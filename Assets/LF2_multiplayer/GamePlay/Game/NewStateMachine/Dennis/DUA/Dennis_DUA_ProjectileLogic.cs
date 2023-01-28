using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Collections;
using LF2.Utils;

namespace LF2.Client
{
    // BAll Chase
    public class Dennis_DUA_ProjectileLogic :  ProjectileLogic
    {
        [SerializeField] private Animator animator;
        private int Hit = Animator.StringToHash("DDA_Hit_Jonh_anim");
        List<ClientCharacterVisualization> nbCharacterViz = new List<ClientCharacterVisualization>();

        ClientCharacterVisualization enemyLock;
        private Vector3 enemyPosition;
        private bool haveTarget;
        private float interpolationAmount;
        private Vector3 ourPosition;
        [SerializeField] Vector3 AnchorBFromA;

        public override void Initialize(ulong creatorsNetworkObjectId,TeamType teamType, Vector3 dir_ToMove , Vector3 rotation = default )
        {
            base.Initialize(creatorsNetworkObjectId ,teamType, dir_ToMove );
            // var nbPlayer = GetNetworkObject(creatorsNetworkObjectId).GetComponent<NbPlayer>();
            // nbCharacterViz = NbPlayer.GetPlayer();
            interpolationAmount = 0f;
            foreach (ClientCharacterVisualization viz in NbPlayer.GetCharacter()){
                if (viz.NetworkObjectId != creatorsNetworkObjectId ){
                    Debug.Log("clientCharacter" + viz);
                    nbCharacterViz.Add(viz);
                }
            }
            if (nbCharacterViz.Count > 0){
                int nbRamdom = UnityEngine.Random.Range(0, nbCharacterViz.Count);

                enemyLock = nbCharacterViz[nbRamdom];
                enemyPosition = enemyLock.PhysicsWrapper.transform.position;
                haveTarget = true;
            }else{
                haveTarget = false;
            }
            ourPosition = transform.position;
        }

        public override void FixedUpdate() {
            
            // Debug.Log($"interpolation Amount {interpolationAmount}" );
            if (haveTarget && interpolationAmount < 1){
                if (enemyLock == null ||enemyLock.m_NetState.LifeState == LifeState.Dead ) {
                    haveTarget = false;
                    return;
                }
                interpolationAmount = (interpolationAmount + Time.deltaTime) ;
                transform.position = QuadraticLerp(ourPosition ,AnchorBFromA + ourPosition , enemyLock.PhysicsWrapper.transform.position , interpolationAmount );
            } 
            else base.FixedUpdate();
        }

        private Vector3 QuadraticLerp(Vector3 a , Vector3 b , Vector3 c , float t ){
            Vector3 ab = Vector3.Lerp(a , b , t);
            Vector3 bc = Vector3.Lerp(b , c , t);
            return Vector3.Lerp(ab , bc , t);
        }



        public override void Rebound()
        {
            _DirToMove = new Vector3 (_DirToMove.x*-1 ,0,0 ); 
        }

        protected override void OnTriggerEnter(Collider collider) {
            base.OnTriggerEnter(collider);
            // if (collider.CompareTag("HitBox")){
            //     // var targetNetObj = collider.GetComponentInParent<IHurtBox>();
            //     var targetNetObj = collider.GetComponentInParent<IHurtBox>();
            //     if (targetNetObj.NetworkObjectId != m_SpawnerId )
            //         Rebound();
            //     return;
            // }
            // if (collider.CompareTag("Projectile")){
            //     Rebound();
            //     return;
            // }
            // if (collider.CompareTag("BlockToRebound")){
            //     Rebound();
            //     return;
            // }

            // if (collider.CompareTag("HurtBox")){

            //     var targetNetObj = collider.GetComponentInParent<IHurtBox>();

            //     if (targetNetObj != null ){
            //         if( ( targetNetObj.NetworkObjectId != m_SpawnerId) )
            //         {
            //             Debug.Log(targetNetObj);

            //             m_NetState.RecvHitEnemyClientRPC(targetNetObj.NetworkObjectId);
            //             AttackDataSend Atk_data = new AttackDataSend();
            //             Atk_data.Direction = new Vector3(ProjectileDamage[0].Dirxyz.x * transform.right.x , ProjectileDamage[0].Dirxyz.y,ProjectileDamage[0].Dirxyz.z) ;
            //             Atk_data.BDefense_p = ProjectileDamage[0].Bdefend;
            //             Atk_data.Fall_p = ProjectileDamage[0].fall;
            //             targetNetObj.ReceiveHP(Atk_data);
            //             CanMove = false;
            //             animator.Play(Hit);
            //             if (ProjectileDamage[0].SoundHit.Length > 0 )PlayAudio(ProjectileDamage[0].SoundHit[0], transform.position);
            //             m_Started = false;
            //             Coro_Balldp(DestroyAfterHit);

            //         }
            //     }
            // }

        }

        IEnumerator  Coro_Balldp(float delay)
        {
            yield return new WaitForSeconds(delay);
            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();
        }
    }
}

