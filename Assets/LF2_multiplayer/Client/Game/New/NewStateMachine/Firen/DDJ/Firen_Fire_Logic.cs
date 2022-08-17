using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class Firen_Fire_Logic :  ProjectileLogic
    {
       
        [SerializeField] private Animator animator;
        private int Firen_Fire_2 = Animator.StringToHash("Fire_2");
        [SerializeField] private float delayFire1;

        [SerializeField] private float delayFire2;

        public override void Initialize(ulong creatorsNetworkObjectId, TeamType teamType,Vector3 dir_ToMove, in SkillsDescription.ProjectileInfo projectileInfo)
        {
            base.Initialize(creatorsNetworkObjectId, teamType,dir_ToMove, projectileInfo);
            StartCoroutine(Coro_Fire_1(delayFire1));
        }
        // First State of the Fire : 
        IEnumerator Coro_Fire_1(float delay){
            yield return new WaitForSeconds(delay);
            StartCoroutine(Coro_Fire_2(delayFire2));

        }
        // Seconde State of the Fire : dissapear after few time 
        IEnumerator Coro_Fire_2(float delay){
            animator.Play(Firen_Fire_2);
            yield return new WaitForSeconds(delay);

            // NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            // networkObject.Despawn();

            Destroy(gameObject);
        
        }


        protected override void OnTriggerEnter(Collider collider) {

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
                    }
                }
            }

        }

    }
}

