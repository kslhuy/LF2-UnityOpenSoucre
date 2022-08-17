using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class Firen_DDA_ProjectileLogic :  ProjectileLogic
    {
       
        [SerializeField] private Animator animator;
        private int Henry_Arrow_Normal_2 = Animator.StringToHash("Fire_Ball_2");

        public override void FixedUpdate() {
            if (IsServer){

                if (m_Started){
                    m_Started = false;
                }
                Vector3 displacement = m_ProjectileInfo.Speed_m_s*Time.deltaTime*_DirToMove ;
                _rigidbody.MovePosition(transform.position + displacement);
            }

        }


        public override void Rebound()
        {
            animator.Play(Henry_Arrow_Normal_2);
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
                    if( ( targetNetObj.NetworkObjectId != m_SpawnerId) && targetNetObj.IsDamageable(teamAttacker))
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

                        if (!IsServer) return;
                            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                            networkObject.Despawn();
                    
                    }
                }
            }

        }

    }
}

