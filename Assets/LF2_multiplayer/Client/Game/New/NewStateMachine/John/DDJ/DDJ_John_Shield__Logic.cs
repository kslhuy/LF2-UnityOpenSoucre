using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
namespace LF2.Client{


    public class DDJ_John_Shield__Logic : ProjectileLogic
    {
        
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] Animator _animator;
        


        // public override void Initialize(ulong creatorsNetworkObjectId, int facing,in SkillsDescription.ProjectileInfo projectileInfo)
        // {
        //     base.Initialize(creatorsNetworkObjectId , facing , projectileInfo);

        // }



        // public override void OnNetworkSpawn()
        // {
        //     base.OnNetworkSpawn();        
        // }

        protected override void OnTriggerEnter(Collider collider) {

            if (collider.CompareTag("Projectile")){
                collider.GetComponent<IRebound>().Rebound();
                return;
            }

            if (collider.CompareTag("HurtBox")){

                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if (targetNetObj != null ){
                    if( targetNetObj.NetworkObjectId != m_SpawnerId )
                    {
                        Debug.Log(targetNetObj);

                        m_NetState.RecvHitEnemyClientRPC(targetNetObj.NetworkObjectId);
                        AttackDataSend Atk_data = new AttackDataSend();
                        Atk_data.Direction = m_ProjectileInfo.ProjectileDamage[0].Dirxyz;
                        Atk_data.BDefense_p = m_ProjectileInfo.ProjectileDamage[0].Bdefend;
                        Atk_data.Fall_p = m_ProjectileInfo.ProjectileDamage[0].fall;
                        Atk_data.Facing = m_facing;
                        targetNetObj.ReceiveHP(Atk_data);
                        
                        StartCoroutine(RunAfterEffet(m_ProjectileInfo.DestroyAfterSec));                  
                        // PlayAudio(m_ProjectileInfo.Sounds, transform.position);

                    }
                }
            }
        }

        public override IEnumerator RunAfterEffet(float delay){
                // Time to return to the pool from whence it came.

            _animator.Play("Shiled3_John_anim");
            base.RunAfterEffet(delay);
            return null;
                 
        }





        


    }
}
