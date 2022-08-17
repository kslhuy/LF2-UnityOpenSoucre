using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class John_DDA_ProjectileLogic :  ProjectileLogic
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float delayDp;
        private int Hit = Animator.StringToHash("DDA_Hit_Jonh_anim");

        public override void FixedUpdate() {
            if (IsServer){

                if (m_Started){
                    Vector3 displacement = m_ProjectileInfo.Speed_m_s*Time.deltaTime*_DirToMove ;
                    // _rigidbody.MovePosition(transform.position + displacement);
                    transform.position += displacement; 
                }

            }

        }


        public override void Rebound()
        {
            _DirToMove = new Vector3 (_DirToMove.x*-1 ,0,0 ); 
        }

        protected override void OnTriggerEnter(Collider collider) {
         
            if (collider.CompareTag("HitBox")){
                // var targetNetObj = collider.GetComponentInParent<IHurtBox>();
                var targetNetObj = collider.GetComponentInParent<IHurtBox>();
                if (targetNetObj.NetworkObjectId != m_SpawnerId )
                    Rebound();
                return;
            }
            if (collider.CompareTag("Projectile")){
                Rebound();
                return;
            }

            if (collider.CompareTag("HurtBox")){

                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if (targetNetObj != null ){
                    if( ( targetNetObj.NetworkObjectId != m_SpawnerId) )
                    {
                        Debug.Log(targetNetObj);

                        m_NetState.RecvHitEnemyClientRPC(targetNetObj.NetworkObjectId);
                        AttackDataSend Atk_data = new AttackDataSend();
                        Atk_data.Direction = m_ProjectileInfo.ProjectileDamage[0].Dirxyz;
                        Atk_data.BDefense_p = m_ProjectileInfo.ProjectileDamage[0].Bdefend;
                        Atk_data.Fall_p = m_ProjectileInfo.ProjectileDamage[0].fall;
                        Atk_data.Facing = m_facing;
                        targetNetObj.ReceiveHP(Atk_data);

                        PlayAudio(m_ProjectileInfo.ProjectileDamage[0].SoundHit[0], transform.position);
                        m_Started = false;
                        if (!IsServer) return;
                        Coro_Balldp(delayDp);

                    }
                }
            }

        }

        IEnumerator  Coro_Balldp(float delay)
        {
            yield return new WaitForSeconds(delay);
            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();
        }
    }
}

