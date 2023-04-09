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
                    SendHitData(targetNetObj);
                    
                    CanMove = false;
                    Instantiate(m_OnHitParticlePrefab , transform.position ,Quaternion.identity);
                    // animator.Play(EndAnimation);
                    
                    PlayAudio();
                    
                    if (!IsServer) return;
                    NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                    networkObject.Despawn();

                    
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

