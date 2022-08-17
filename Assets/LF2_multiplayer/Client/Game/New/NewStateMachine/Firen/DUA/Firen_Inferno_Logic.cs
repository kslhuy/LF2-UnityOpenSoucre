using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class Firen_Inferno_Logic :  MonoBehaviour
    {
       

        private void OnTriggerEnter(Collider collider) {

            if (collider.CompareTag("HurtBox")){

                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if (targetNetObj != null ){
                    // if( ( targetNetObj.NetworkObjectId != m_SpawnerId) )
                    // {
                    //     Debug.Log(targetNetObj);

                        // m_NetState.RecvHitEnemyClientRPC(targetNetObj.NetworkObjectId);
                        // AttackDataSend Atk_data = new AttackDataSend();
                        // Atk_data.Direction = m_ProjectileInfo.ProjectileDamage[0].Dirxyz;
                        // Atk_data.BDefense_p = m_ProjectileInfo.ProjectileDamage[0].Bdefend;
                        // Atk_data.Fall_p = m_ProjectileInfo.ProjectileDamage[0].fall;
                        // Atk_data.Facing = m_facing;
                        // targetNetObj.ReceiveHP(Atk_data);
                        
                        // PlayAudio(m_ProjectileInfo.ProjectileDamage[0].SoundHit[0], transform.position);
                    // }
                }
            }

        }

    }
}

