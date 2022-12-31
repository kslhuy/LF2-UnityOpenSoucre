using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class Henry_attack_ProjectileLogic :  ProjectileLogic
    {

        [SerializeField] private Animator animator;

        [SerializeField] private float _gravity = 9.8f;
        [SerializeField] GameObject arrowCacked;

        public override void FixedUpdate() {
            if (!m_cachedIsServer || !m_Started) return;

            if (DestroyAfterSec + timerDestroy < Time.time)
            {
                // Time to return to the pool from whence it came.
                var networkObject = gameObject.GetComponent<NetworkObject>();
                networkObject.Despawn();
                return;
            }
            
            if (CanMove){
                
                Vector3 displacement = Speed_m_s*Time.deltaTime*(transform.right + new Vector3(0,0,0.5f*_DirToMove.z)) + Time.deltaTime*new Vector3(0,-_gravity,0);

                transform.position += displacement;
            }
                
            
        }


        public override void Rebound()
        {
            // arrowCacked.SetActive(true);
            animator.Play(EndAnimation);
            m_Started = false;
            
        }

        protected override void OnTriggerEnter(Collider collider) {
            if (collider.CompareTag("Ground")){
                animator.Play(EndAnimation);
                NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                networkObject.Despawn();
                return;
            }
            if (collider.CompareTag("HitBox")){
                // var targetNetObj = collider.GetComponentInParent<IHurtBox>();
                m_SpawnerId = collider.GetComponent<ProjectileLogic>().NetworkObjectId;
                Rebound();
                return;
            }

            if (collider.CompareTag("BlockToRebound")){
                m_SpawnerId = collider.GetComponent<ProjectileLogic>().NetworkObjectId;
                Rebound();
                return;
            }
        
            if (collider.CompareTag("HurtBox")){

                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if (targetNetObj != null ){
                    if( ( targetNetObj.NetworkObjectId != m_SpawnerId) )
                    {
                        Debug.Log(targetNetObj);

                        AttackDataSend Atk_data = new AttackDataSend();
                        Atk_data.Direction = new Vector3(ProjectileDamage[0].Dirxyz.x * transform.right.x , ProjectileDamage[0].Dirxyz.y,ProjectileDamage[0].Dirxyz.z) ;
                        Atk_data.Amount_injury = ProjectileDamage[0].damageAmount;
                        Atk_data.BDefense_p = ProjectileDamage[0].Bdefend;
                        Atk_data.Fall_p = ProjectileDamage[0].fall;
                        Atk_data.Effect = (byte)ProjectileDamage[0].Effect;

                        targetNetObj.ReceiveHP(Atk_data);
                        
                        // PlayAudio(Sounds, transform.position);

                        if (!IsServer) return;
                            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                            networkObject.Despawn();
                    
                    }
                }
            }

        }

    }
}

