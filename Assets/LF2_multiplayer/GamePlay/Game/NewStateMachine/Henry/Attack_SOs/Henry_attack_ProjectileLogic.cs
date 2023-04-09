using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;
using LF2.Utils;

namespace LF2.Client
{

    public class Henry_attack_ProjectileLogic :  ProjectileLogic
    {

        [SerializeField] private Animator animator;

        [SerializeField] private AnimationCurve gravityCurve ;
        private float _gravitySpeed;
        // [SerializeField] GameObject arrowCacked;
        private RaycastHit[] GroundHit = new RaycastHit[1];
        float gravityTime ;

        public override void Initialize(ulong creatorsNetworkObjectId, TeamType team, Vector3 dir_ToMove, Vector3 rotation = default)
        {
            base.Initialize(creatorsNetworkObjectId, team, dir_ToMove, rotation);
            gravityTime = 0;
            // _gravitySpeed =  gravityCurve.Evaluate(gravityTime);
        }
        public override void FixedUpdate() {
            if (!m_cachedIsServer || !m_Started) return;
            // this logic is Server Only
            if (DestroyAfterSec + timerDestroy < Time.time)
            {
                // Time to return to the pool from whence it came.
                var networkObject = gameObject.GetComponent<NetworkObject>();
                networkObject.Despawn();
                return;
            }
            
            if (CanMove){
                gravityTime += Time.fixedDeltaTime;
                _gravitySpeed =  gravityCurve.Evaluate(gravityTime) ;

                Vector3 displacement = Speed_m_s*Time.deltaTime*(transform.right + new Vector3(0,0,0.5f*_DirToMove.z)) + Time.deltaTime*new Vector3(0,-_gravitySpeed,0);

                transform.position += displacement;
            }
            if (IsGounded()){
                PlayAudio(Sounds[0],transform.position);
                if (!IsServer) return;
                NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                networkObject.Despawn();
            }
                
            
        }


        public override void Rebound()
        {
            // arrowCacked.SetActive(true);
            animator.Play(EndAnimation);
            // m_OurCollider.enabled = false;
            PlayAudio(Sounds[0],transform.position);
            m_Started = false;
            
            

        }

        public bool IsGounded()
        {
            // var _isGround = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, _grounderOffset), _grounderRadius, _ground, k_GroundLayerMask) > 0;
            int number = Physics.RaycastNonAlloc(m_OurCollider.bounds.center, Vector3.down, GroundHit, m_OurCollider.bounds.extents.y + 1f, k_GroundLayerMask);

            if (number == 0) return false;
            // if is ground 

            return true;

        }

        protected override void OnTriggerEnter(Collider collider) {
            // Debug.Log(collider);
            // if (collider.CompareTag("Ground")){
            //     // animator.Play(EndAnimation);
            //     PlayAudio(Sounds[0]);
            //     if (!IsServer) return;
            //     NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            //     networkObject.Despawn();
                
            // }
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
                // Debug.Log("hurt");
                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if (targetNetObj != null ){
                    if( ( targetNetObj.NetworkObjectId != m_SpawnerId) )
                    {
                        // Debug.Log(targetNetObj);
                        SendHitData(targetNetObj);
                        PlayAudio(Sounds[0] , transform.position);

                        if (!IsServer) return;
                        NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                        networkObject.Despawn();
                    
                    }
                }
            }

        }

    }
}

