using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class Henry_Flacon_projectile :  ProjectileLogic
    {
        [SerializeField] private AnimationCurve Noise_y;
        
        private Vector3 _Direction = new Vector3(1,1,0);
        [SerializeField] private Animator animator;
        private int Henry_Flacon_Attack = Animator.StringToHash("Henry_Flacon_2");
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
                }
                if (_time < 1){
                    transform.position = Vector3.Lerp(startPosition,endPosition , _time) + new Vector3(0, Amplitude*Noise_y.Evaluate(_time), 0) ;
                    _time += Time.deltaTime * _speed;
                }                
                else {
                    NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                    networkObject.Despawn();

                }
            
            }
            
        }


        protected override void OnTriggerEnter(Collider collider) {
         

            if (collider.CompareTag("Projectile")){
            // Dissapear when collide with shield John 
                return;
            }

            if (collider.CompareTag("HurtBox")){

                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if( ( targetNetObj != null && targetNetObj.NetworkObjectId != m_SpawnerId) )
                {
                    Debug.Log(targetNetObj);

                    AttackDataSend Atk_data = new AttackDataSend();
                    Atk_data.Amount_injury = ProjectileDamage[0].damageAmount;
                    Atk_data.Direction = new Vector3(ProjectileDamage[0].Dirxyz.x * transform.right.x , ProjectileDamage[0].Dirxyz.y,ProjectileDamage[0].Dirxyz.z) ;
                    Atk_data.BDefense_p = ProjectileDamage[0].Bdefend;
                    Atk_data.Fall_p = ProjectileDamage[0].fall;
                    Atk_data.Effect = (byte)ProjectileDamage[0].Effect;

                    targetNetObj.ReceiveHP(Atk_data);
                    
                    // PlayAudio(Sounds, transform.position);

                    if (!IsServer) return;
                        PlayAudio(Sounds[0]);
                        animator.Play(Henry_Flacon_Attack);
                        StartCoroutine(RunAfterEffet(DestroyAfterHit));                  

                
                }
            
            }

        }


    }
}

