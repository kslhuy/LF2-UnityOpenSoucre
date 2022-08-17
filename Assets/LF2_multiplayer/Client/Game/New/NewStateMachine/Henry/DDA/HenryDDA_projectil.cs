using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class HenryDDA_projectil : ProjectileLogic
    {
        private int nbTime_Hit;

        public override void FixedUpdate() {

            
            if (!m_Started) { return; } //don't do anything before OnNetworkSpawn has run.
            // Debug.Log(m_facing);
            if (CanMove){
                Vector3 displacement = m_ProjectileInfo.Speed_m_s*Time.deltaTime*m_facing*Vector3.right ;
                _rigidbody.MovePosition(transform.position + displacement);

            }
        }


        protected override void OnTriggerEnter(Collider collider) {
         

            if (collider.CompareTag("Projectile")){
                Debug.Log("collisio, projectile");
                StartCoroutine(RunAfterEffet(0.1f));
                return;
            }

            if (collider.CompareTag("HurtBox")){

                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if (targetNetObj != null ){
                    if( (targetNetObj.NetworkObjectId != m_SpawnerId) )
                    {
                        Debug.Log(targetNetObj);

                        // if (IsServer)
                        // {
                            
                            AttackDataSend Atk_data = new AttackDataSend();

                            Atk_data.Direction = m_ProjectileInfo.ProjectileDamage[0].Dirxyz;
                            Atk_data.BDefense_p = m_ProjectileInfo.ProjectileDamage[0].Bdefend;
                            Atk_data.Fall_p = m_ProjectileInfo.ProjectileDamage[0].fall;
                            Atk_data.Facing = m_facing;
                            targetNetObj.ReceiveHP(Atk_data);
                                    
                        // }   
                        // PlayAudio(m_ProjectileInfo.Sounds, transform.position);
                        nbTime_Hit += 1;

                        if (nbTime_Hit >= m_ProjectileInfo.MaxVictims){
                            if (!IsServer) 
                                StartCoroutine(RunAfterEffet(m_ProjectileInfo.DestroyAfterSec));                  
                            else gameObject.SetActive(false);
                        }
                    }
                }
            }

        }

    }
}

