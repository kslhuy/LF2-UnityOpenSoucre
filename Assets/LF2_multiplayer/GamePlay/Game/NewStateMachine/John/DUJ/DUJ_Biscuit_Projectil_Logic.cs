using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LF2.Utils;
using Unity.Netcode;
using UnityEngine;
namespace LF2.Client{

    enum BisCuitState{
        borning,
        flying,
        chase,
        teleport,
        rebounding,
        hit,
        hitting,
        despawn
    }
    public class DUJ_Biscuit_Projectil_Logic : ProjectileLogic
    {
        
        [SerializeField] Animator _animator;


        [SerializeField] float NumberTeleport;

        // [SerializeField] AnimationCurve _NoiseCurveFly;



        ClientCharacterVisualization enemyLock;
        private Transform enemyTransform;


        
        
        private float _timeNow;

        [Header("--------- Timer State --------------- ")]
        [SerializeField] float timerWaitToCuvre;

        [SerializeField] private float timerBorning;
        [SerializeField] private float m_timerTeleport_dessappear;
        [SerializeField] private float m_timerTeleport_appear;

        [SerializeField] private float timeToFlying;

        [SerializeField] private float timeToFinish;


        [ SerializeField] private BisCuitState bisCuitState;


        [Header("--------- Noise --------------- ")]
        [SerializeField] AnimationCurve _NoiseCurveChase;
        [SerializeField] float MinNoise = 3;
        [SerializeField] float MaxNoise = 4;

        [SerializeField] float  xOffset;

        [SerializeField] float yOffset ;

        [Header("--------- Speed --------------- ")]

        public float SpeedChase;
        public float SpeedFly ;

        [SerializeField] private int NbLimiteToStopChase = 3;
        List<ClientCharacterVisualization> nbCharacterViz = new List<ClientCharacterVisualization>();



        private int NbTentativeChase;
        private bool haveTarget ; 


        public override void Initialize(ulong creatorsNetworkObjectId,TeamType teamType, Vector3 dir_ToMove , Vector3 rotation = default )
        {
            base.Initialize(creatorsNetworkObjectId ,teamType, dir_ToMove );


                        // base.Initialize(creatorsNetworkObjectId ,teamType, dir_ToMove );
            // var nbPlayer = GetNetworkObject(creatorsNetworkObjectId).GetComponent<NbPlayer>();
            // nbCharacterViz = NbPlayer.GetPlayer();
            foreach (ClientCharacterVisualization viz in NbPlayer.GetCharacter()){
                if (viz.NetworkObjectId != creatorsNetworkObjectId ){
                    Debug.Log("clientCharacter" + viz);
                    nbCharacterViz.Add(viz);
                }
            }
            if (nbCharacterViz.Count > 0){
                int nbRamdom = Random.Range(0, nbCharacterViz.Count);

                enemyLock = nbCharacterViz[nbRamdom];
                enemyTransform = enemyLock.PhysicsWrapper.transform;

                haveTarget = true;
            }else{
                haveTarget = false;
                // enemyTransform = dir_ToMove.x*1000*Vector3.right ;
            }


            // var nbPlayer = GetNetworkObject(creatorsNetworkObjectId).GetComponent<NbPlayer>();
            // nbCharacterViz = NbPlayer.GetPlayer();
            // foreach (ClientCharacterVisualization viz in NbPlayer.GetPlayer()){
            //     if (viz.NetworkObjectId != creatorsNetworkObjectId ){
            //         nbCharacterViz.Add(viz);
            //     }
            // }
            // // int nbRamdom = Random.Range(0, nbCharacterViz.Count);

            // enemyLock = nbCharacterViz[nbRamdom];
            // enemyTransform = enemyLock.PhysicsWrapper.transform;

            StartCoroutine(Borning(timerBorning));
        }



        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _timeNow = Time.time;
        
        }

        // need overide , not remove that  
        public override void FixedUpdate()
        {}



        IEnumerator Borning(float _timerBorning){
            _animator.Play("john_biscuit_borning");
            bisCuitState = BisCuitState.borning;

            yield return new WaitForSeconds(_timerBorning);

            if (haveTarget){
                StartCoroutine(Tele_Disappear_Then_Appear(m_timerTeleport_dessappear ,m_timerTeleport_appear )) ;

            }else{
                StartCoroutine(BiscuitFlyNoTarget());
            }

        }

        

        IEnumerator Tele_Disappear_Then_Appear(float timerTeleportDisappear , float timerTeleportAppear  ){
            // borning =>>>  teleport  
            _animator.Play("john_biscuit_teleport_desapear");
            bisCuitState = BisCuitState.teleport;

            m_OurCollider.enabled = false;
            yield return new WaitForSeconds(timerTeleportDisappear);
            m_OurCollider.enabled = true;
          

            // Seach ennemy phase 
            float ramdomXposition = (2 * (Random.Range(0, 2)) - 1 )* xOffset;
            // Debug.Log(enemyTransform.position);

            transform.position = enemyTransform.position + new Vector3(ramdomXposition, 80, 0);
            m_facing = (int)(enemyTransform.position.x - transform.position.x) + 1;
            m_facing = m_facing/Mathf.Abs(m_facing); // To get -1 or 1
 
            _animator.Play("john_biscuit_teleport_appear");
            yield return new WaitForSeconds(timerTeleportAppear);

            StartCoroutine(BiscuitCuvre(timerWaitToCuvre));
        }



        IEnumerator BiscuitCuvre(float delayToChase){
            NbTentativeChase += 1 ; 
            bisCuitState = BisCuitState.chase;

            yield return new WaitForSeconds(delayToChase);

            if(enemyTransform == null) {
                StartCoroutine(BiscuitFinish(timeToFinish));
                yield return null;
            }else{
                Vector3 startPosition = transform.position;
                Vector3 endPosition = enemyTransform.position + new Vector3(m_facing*xOffset, yOffset, 0);;

                float NoisePositionChase = 0;
                float time = 0;
                while (time < 1)
                {
                    
                    NoisePositionChase = _NoiseCurveChase.Evaluate(time);
                    transform.position = Vector3.Lerp(startPosition,endPosition , time) +new Vector3(0, -yOffset*NoisePositionChase, 0) ;

                    time += Time.deltaTime * SpeedChase;

                    yield return null;
                }


                if (NbTentativeChase <= NbLimiteToStopChase){
                    
                    StartCoroutine(BiscuitFly());
                }else{
                    StartCoroutine(BiscuitFinish(timeToFinish));

                }
            }

  
        }

        // Try to following Enemy
        IEnumerator BiscuitFly(){

            bisCuitState = BisCuitState.flying;

            _timeNow = Time.time;
            while ( Time.time - _timeNow < timeToFlying){
                Vector3 startPosition = transform.position;
                float Noise_z = Random.Range(MinNoise, MaxNoise);
                m_facing = (int)(enemyTransform.position.x - transform.position.x) + 1 ;
                m_facing = m_facing/Mathf.Abs(m_facing); // To get -1 or 1
                Vector3 endPosition = enemyTransform.position + new Vector3(m_facing*xOffset, yOffset, 0);
                Vector3 Anchor = new Vector3(startPosition.x ,endPosition.y,endPosition.z );

                float time = 0;
                while (time < 1)
                {                
                    transform.position = QuadraticLerp(startPosition , Anchor , endPosition , time);

                    time += Time.deltaTime * SpeedFly;

                    yield return null;
                }
            }              
            StartCoroutine( Tele_Disappear_Then_Appear(m_timerTeleport_dessappear ,m_timerTeleport_appear  ));
            
        }
        IEnumerator BiscuitFinish(float delayToFinish){
            bisCuitState = BisCuitState.despawn;

            float timer = Time.time;
            while (Time.time - timer < delayToFinish)
            {
                // Debug.Log(m_facing);
                transform.position +=  new Vector3(m_facing*Speed_m_s , 0 , 0)*Time.deltaTime;
                yield return null;
            }

            
  
            // yield return new WaitForSeconds(delayToDisappear);

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();   
        }

        IEnumerator BiscuitFlyNoTarget(){

            bisCuitState = BisCuitState.flying;

            timerDestroy = Time.time;
            while ( DestroyAfterSec + timerDestroy > Time.time){

                transform.position += Speed_m_s*Time.deltaTime*transform.right;
                yield return null;
            }              
            StartCoroutine(BiscuitFinish(timeToFinish)); 
            
        }
            
        private Vector3 QuadraticLerp(Vector3 a , Vector3 b , Vector3 c , float t ){
            Vector3 ab = Vector3.Lerp(a , b , t);
            Vector3 bc = Vector3.Lerp(b , c , t);
            return Vector3.Lerp(ab , bc , t);
        }






    }
}
