using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class Frezze_DDA_ProjectileLogic :  ProjectileLogic
    {
        [SerializeField] private Animator animator;
        // [SerializeField] private ProjectileBoxType m_boxType = ProjectileBoxType.Blockable; 

        [SerializeField] private ProjectileBoxMask m_ProjectilHurtboxMask = ProjectileBoxMask.Normal; 

        // private int Hit = Animator.StringToHash("Freeze_Ball_DDA_hitting");

        public override void FixedUpdate() {
            base.FixedUpdate();
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
            if (collider.CompareTag("Projectile") || collider.CompareTag("Dummy") ){
                // var projectile = collider.GetComponentInParent<ProjectileLogic>();
                // projectile.ColliderType 

                Coro_Balldp(DestroyAfterHit);

                return;
            }
            if (collider.CompareTag("BlockToRebound")){
                Rebound();
                return;
            }

            if (collider.CompareTag("HurtBox")){

                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if (targetNetObj != null && ( targetNetObj.NetworkObjectId != m_SpawnerId) && targetNetObj.IsDamageable(teamAttacker)){
                    
                    // Debug.Log(targetNetObj);

                    AttackDataSend Atk_data = new AttackDataSend();
                    
                    Atk_data.Amount_injury = ProjectileDamage[0].damageAmount;
                    Atk_data.Effect = (byte)ProjectileDamage[0].Effect;
                    Atk_data.Direction = new Vector3(ProjectileDamage[0].Dirxyz.x * transform.right.x , ProjectileDamage[0].Dirxyz.y,ProjectileDamage[0].Dirxyz.z) ;
                    Atk_data.BDefense_p = ProjectileDamage[0].Bdefend;
                    Atk_data.Fall_p = ProjectileDamage[0].fall;
                    
                    targetNetObj.ReceiveHP(Atk_data);
                    CanMove = false;
                    animator.Play(EndAnimation);
                    if (ProjectileDamage[0].SoundHit.Length > 0 )PlayAudio(ProjectileDamage[0].SoundHit[0], transform.position);
                    m_Started = false;
                    Coro_Balldp(DestroyAfterHit);

                    
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

