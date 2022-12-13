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
                Vector3 displacement = Speed_m_s*Time.deltaTime*m_facing*Vector3.right ;
                m_Rigidbody.MovePosition(transform.position + displacement);

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

                            Atk_data.Direction = new Vector3(ProjectileDamage[0].Dirxyz.x * transform.right.x , ProjectileDamage[0].Dirxyz.y,ProjectileDamage[0].Dirxyz.z) ;
                            Atk_data.BDefense_p = ProjectileDamage[0].Bdefend;
                            Atk_data.Fall_p = ProjectileDamage[0].fall;
                            targetNetObj.ReceiveHP(Atk_data);
                                    
                        // }   
                        // PlayAudio(Sounds, transform.position);
                        nbTime_Hit += 1;

                        if (nbTime_Hit >= MaxVictims){
                            if (!IsServer) 
                                StartCoroutine(RunAfterEffet(DestroyAfterSec));                  
                            else gameObject.SetActive(false);
                        }
                    }
                }
            }

        }

    }
}

