using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
namespace LF2.Client{

    enum BisCuitState{
        FirtsTimeSpawn,
        Disappear,
        Fly,
        Appear,
        Hitting,
        Chase,
        Wait,
    }
    public class DUJ_Biscuit_Projectil_Logic : ProjectileLogic
    {
        
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] Animator _animator;
        
        [SerializeField] int  distanceXPositionWhenAppear;

        [SerializeField] float  DelayApp;

        [SerializeField] float DelayChase;

        [SerializeField] float PeriodeTimeToDissAndAppearAgain;

        [SerializeField] AnimationCurve _NoiseCurveChase;
        // [SerializeField] AnimationCurve _NoiseCurveFly;



        ClientCharacterVisualization enemyLock;
        private Transform enemyTransform;
        private Coroutine Coro_Appear;
         float _timeNow;


        List<ClientCharacterVisualization> nbCharacterViz = new List<ClientCharacterVisualization>();

        [ SerializeField] private BisCuitState bisCuitState;
        [SerializeField] private float timerToFinishAppearAnimation;
        [SerializeField] private float timerToFinishDissapearAnimation;


        [SerializeField] float MinNoise = 3;
        [SerializeField] float MaxNoise = 4;

        public float yOffset ;

        public float SpeedChase;

        public float SpeedFly ;
        private int NbTentativeChase;
        [SerializeField] private int NbLimiteToStopChase = 3;



        public override void Initialize(ulong creatorsNetworkObjectId,TeamType teamType, Vector3 dir_ToMove,in SkillsDescription.ProjectileInfo projectileInfo)
        {
            base.Initialize(creatorsNetworkObjectId ,teamType, dir_ToMove , projectileInfo);
            // var nbPlayer = GetNetworkObject(creatorsNetworkObjectId).GetComponent<NbPlayer>();
            // nbCharacterViz = NbPlayer.GetPlayer();
            foreach (ClientCharacterVisualization viz in NbPlayer.GetPlayer()){
                if (viz.NetworkObjectId != creatorsNetworkObjectId ){
                    nbCharacterViz.Add(viz);
                }
            }
            int nbRamdom = Random.Range(0, nbCharacterViz.Count);

            enemyLock = nbCharacterViz[nbRamdom];
            enemyTransform = enemyLock.PhysicsWrapper.transform;

            StartCoroutine(SpawnFirstTime(timerToFinishDissapearAnimation));
        }



        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            bisCuitState = BisCuitState.FirtsTimeSpawn;
            _timeNow = Time.time;
        
        }



        IEnumerator SpawnFirstTime(float timerToFinishAppearAnimation){

            yield return new WaitForSeconds(timerToFinishAppearAnimation);
            StartCoroutine(Disappear(timerToFinishDissapearAnimation)) ;

        }

        IEnumerator Disappear(float delayToDisappear){
            // First time to wait to =>>>  disappear 
            _animator.Play("john_biscuit3");

            yield return new WaitForSeconds(delayToDisappear);

            m_OurCollider.enabled = false;
            bisCuitState = BisCuitState.Disappear;
            m_OurCollider.enabled = true;
          
            Coro_Appear =  StartCoroutine( Appear( timerToFinishAppearAnimation ));
        }
        IEnumerator Appear (float delayToAppear ){

            // bisCuitState = BisCuitState.Wait;
            int ramdomXposition = (2 * (Random.Range(0, 2)) - 1 )* distanceXPositionWhenAppear;
            transform.position = enemyTransform.position + new Vector3(ramdomXposition, m_ProjectileInfo.pivot.y, 0);
            m_facing = (int)(enemyTransform.position.x - transform.position.x);
            m_facing = m_facing/Mathf.Abs(m_facing); // To get -1 or 1
            if (!IsServer)
            {
                Debug.Log("Client");
            }
            yield return new WaitForSeconds(delayToAppear);
            _animator.Play("john_biscuit");
            
 
            StartCoroutine(BiscuitChase(DelayChase));
        }


        IEnumerator BiscuitChase(float delayToChase){
            NbTentativeChase += 1 ; 
            yield return new WaitForSeconds(delayToChase);

            bisCuitState = BisCuitState.Chase;  
            Vector3 startPosition = transform.position;
            Vector3 endPosition = enemyTransform.position + new Vector3(m_facing*distanceXPositionWhenAppear, yOffset, 0);;

            float NoisePositionChase = 0;
            float time = 0;
            while (time < 1)
            {
                
                NoisePositionChase = _NoiseCurveChase.Evaluate(time);
                transform.position = Vector3.Lerp(startPosition,endPosition , time) +new Vector3(0, -yOffset*NoisePositionChase, 0) ;

                time += Time.deltaTime * SpeedChase;

                yield return null;
            }
            bisCuitState = BisCuitState.Fly;  
            if (NbTentativeChase <= NbLimiteToStopChase){
                StartCoroutine(BiscuitFly());
            }else{
                StartCoroutine(BiscuitFinish(timerToFinishAppearAnimation + timerToFinishDissapearAnimation));

            }

  
        }

        // Try to following Enemy
        IEnumerator BiscuitFly(){
            _timeNow = Time.time;
            while ( Time.time - _timeNow < PeriodeTimeToDissAndAppearAgain){
                Vector3 startPosition = transform.position;
                float Noise_z = Random.Range(MinNoise, MaxNoise);
                m_facing = (int)(enemyTransform.position.x - transform.position.x);
                m_facing = m_facing/Mathf.Abs(m_facing); // To get -1 or 1
                Vector3 endPosition = enemyTransform.position + new Vector3(m_facing*distanceXPositionWhenAppear, yOffset, 0);;
                Vector3 Anchor = new Vector3(startPosition.x ,endPosition.y,endPosition.z );

                float time = 0;
                while (time < 1)
                {                
                    transform.position = QuadraticLerp(startPosition , Anchor , endPosition , time);

                    time += Time.deltaTime * SpeedFly;

                    yield return null;
                }
            }              
            bisCuitState = BisCuitState.Disappear;
            StartCoroutine( Disappear(timerToFinishDissapearAnimation));
            
        }

        IEnumerator BiscuitFinish(float delayToFinish){

            float timer = Time.time;
            while (Time.time - timer < delayToFinish)
            {
                _rigidbody.MovePosition(transform.position + new Vector3(m_facing*m_ProjectileInfo.Speed_m_s , 0 , 0)*Time.deltaTime);
                yield return null;
            }
  
            // yield return new WaitForSeconds(delayToDisappear);

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();   
        }
            
        private Vector3 QuadraticLerp(Vector3 a , Vector3 b , Vector3 c , float t ){
            Vector3 ab = Vector3.Lerp(a , b , t);
            Vector3 bc = Vector3.Lerp(b , c , t);
            return Vector3.Lerp(ab , bc , t);
        }


        //  protected override void OnTriggerEnter(Collider collider) {
        //     if (collider.CompareTag("HitBox")){
        //         // canHitCreator = true;
        //         Rebound();
        //         return;
        //     }

        


    }
}
