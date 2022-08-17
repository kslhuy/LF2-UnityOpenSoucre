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
        [SerializeField] private AnimationCurve Noise_y;
        
        private Vector3 _Direction = new Vector3(1,1,0);
        [SerializeField] private Animator animator;
        private int Henry_Arrow_Normal_2 = Animator.StringToHash("Henry_Arrow_Normal_2");
        private float _time;
        private Vector3 startPosition;
        private Vector3 endPosition;
        public Vector3 _Distance;

        public float Amplitude;

        [SerializeField] private float _speed;
        public override void FixedUpdate() {
            if (IsServer){

                if (m_Started){
                    startPosition = transform.position;
                    endPosition = startPosition + new Vector3 (m_facing*_Distance.x,_Distance.y,_Distance.z);
                    m_Started = false;
                    // _rigidbody.AddForce(_Distance,ForceMode.Impulse);
                }

                // _rigidbody.velocity = new Vector3(spped_x.Evaluate(_time) , spped_y.Evaluate(_time),0);
                // _rigidbody.MovePosition(transform.position + new Vector3(spped_x.Evaluate(_time) , spped_y.Evaluate(_time),0));
                if (_time < 1){
                    transform.position = Vector3.Lerp(startPosition,endPosition , _time) + new Vector3(0, Amplitude*Noise_y.Evaluate(_time), 0) ;
                    _time += Time.deltaTime * _speed;
                }

                
                
                if (IsGounded()){
                    NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                    networkObject.Despawn();
                }
            }
            if(IsClient){
                if (IsGounded()){
                    gameObject.SetActive(false);
                }
            }
            
        }


        public bool IsGounded(){
            // var _isGround = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, _grounderOffset), _grounderRadius, _ground, k_GroundLayerMask) > 0;
            bool _isGround = Physics.Raycast(m_OurCollider.bounds.center,Vector3.down ,m_OurCollider.bounds.extents.y+0.1f,k_GroundLayerMask);
            // Color rayColor;
            // if (!hit_ground){
            //     rayColor = Color.green;
            // }else {
            //     rayColor = Color.red;
            // }
            // Debug.DrawRay(m_BoxCollider.bounds.center , Vector3.down * (m_BoxCollider.bounds.extents.y),rayColor);
            if (_isGround )   _rigidbody.velocity = Vector3.zero;
 
            return  _isGround;
        }    
    
        public override void Rebound()
        {
            animator.Play(Henry_Arrow_Normal_2);
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
                        
                        // PlayAudio(m_ProjectileInfo.Sounds, transform.position);

                        if (!IsServer) return;
                            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                            networkObject.Despawn();
                    
                    }
                }
            }

        }

    }
}

