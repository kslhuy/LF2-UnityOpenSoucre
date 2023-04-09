using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class Freeze_DUA_Logic :  ProjectileLogic
    {
        [SerializeField] private Animator animator;
        // private int Hit = Animator.StringToHash("DUA_Hit_Jonh_anim");

        public override void FixedUpdate() {
            base.FixedUpdate();
        }

        public override void Rebound()
        {
            _DirToMove = new Vector3 (_DirToMove.x*-1 ,0,0 ); 
        }

        protected override void OnTriggerEnter(Collider collider) {

            if (collider.CompareTag("HurtBox")){
                // Debug.Log("Hurt Box collider");
                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if (targetNetObj != null && targetNetObj.NetworkObjectId != m_SpawnerId && targetNetObj.IsDamageable(teamAttacker) ){
                    Debug.Log(targetNetObj);

                    // m_NetState.RecvHitEnemyClientRPC(targetNetObj.NetworkObjectId);
                    AttackDataSend Atk_data = new AttackDataSend();
                    Atk_data.Amount_injury = ProjectileDamage[0].damageAmount;
                    Atk_data.Effect = (byte)ProjectileDamage[0].Effect;
                    Atk_data.Direction = new Vector3(ProjectileDamage[0].Dirxyz.x * transform.right.x , ProjectileDamage[0].Dirxyz.y,ProjectileDamage[0].Dirxyz.z) ;
                    Atk_data.BDefense_p = ProjectileDamage[0].Bdefend;
                    Atk_data.Fall_p = ProjectileDamage[0].fall;

                    targetNetObj.ReceiveHP(Atk_data);
                    
                    if (ProjectileDamage[0].SoundHit.Length > 0 )PlayAudio(ProjectileDamage[0].SoundHit[0], transform.position);
                    CanMove = false;
                
                }
            }

        }


    }
}

