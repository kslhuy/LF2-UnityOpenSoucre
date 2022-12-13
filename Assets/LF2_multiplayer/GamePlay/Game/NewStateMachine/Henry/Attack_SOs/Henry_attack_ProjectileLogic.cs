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

        [SerializeField] private float _fallMultiplier;
        [SerializeField] GameObject arrowCacked;

        public override void FixedUpdate() {
            if (IsServer){
                if (m_Started){
                    // Debug.Log("transform right " + transform.right.x); 

                    base.FixedUpdate();
                    SetFallingDown();
                }
                if (!IsGounded()){
                    SetFallingDown();
                }else {
                    m_Rigidbody.velocity = Vector3.zero;
                    NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                    networkObject.Despawn();
                }
            }
        }

        public void SetFallingDown(){
            m_Rigidbody.velocity += _fallMultiplier * Physics.gravity.y * Time.deltaTime * Vector3.up ;  
        }

        public bool IsGounded(){
            // var _isGround = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, _grounderOffset), _grounderRadius, _ground, k_GroundLayerMask) > 0;
            bool _isGround = Physics.Raycast(m_OurCollider.bounds.center,Vector3.down ,m_OurCollider.bounds.extents.y+0.1f,k_GroundLayerMask);
            if (_isGround )   m_Rigidbody.velocity = Vector3.zero;
            SetFallingDown();
            return  _isGround;
        }    
    
        public override void Rebound()
        {
            // arrowCacked.SetActive(true);
            animator.Play(EndAnimation);
            m_Started = false;
            m_Rigidbody.velocity = Vector3.zero ;
            
        }

        protected override void OnTriggerEnter(Collider collider) {
            if (collider.CompareTag("Ground")){
                animator.Play(EndAnimation);
                m_Rigidbody.velocity = Vector3.zero;
                NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                networkObject.Despawn();
                return;
            }
            if (collider.CompareTag("HitBox")){
                // var targetNetObj = collider.GetComponentInParent<IHurtBox>();
                m_SpawnerId = collider.GetComponent<ProjectileLogic>().NetworkObjectId;
                Rebound();
                m_Rigidbody.AddForce(new Vector3(-10*transform.right.x,10,0) , ForceMode.VelocityChange);
                return;
            }

            if (collider.CompareTag("BlockToRebound")){
                m_SpawnerId = collider.GetComponent<ProjectileLogic>().NetworkObjectId;
                Rebound();
                m_Rigidbody.velocity = (new Vector3(-100*transform.right.x,20,0));
                return;
            }
        
            if (collider.CompareTag("HurtBox")){

                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if (targetNetObj != null ){
                    if( ( targetNetObj.NetworkObjectId != m_SpawnerId) )
                    {
                        Debug.Log(targetNetObj);

                        m_NetState.RecvHitEnemyClientRPC(targetNetObj.NetworkObjectId);
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

