using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class Dennis_DDA_ProjectileLogic :  ProjectileLogic
    {
        [SerializeField] private Animator animator;
        private int Hit = Animator.StringToHash("DDA_Hit_Jonh_anim");

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
            if (collider.CompareTag("Projectile")){
                Rebound();
                return;
            }
            if (collider.CompareTag("BlockToRebound")){
                Rebound();
                return;
            }

            if (collider.CompareTag("HurtBox")){

                var targetNetObj = GetTargetObject(collider , out bool isOKtoUse);

                if (isOKtoUse){
                    SendHitData(targetNetObj);
                    PlayAudio();
                    CanMove = false;
                    m_Started = false;

                    Instantiate(m_OnHitParticlePrefab , transform.position ,Quaternion.identity);
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

