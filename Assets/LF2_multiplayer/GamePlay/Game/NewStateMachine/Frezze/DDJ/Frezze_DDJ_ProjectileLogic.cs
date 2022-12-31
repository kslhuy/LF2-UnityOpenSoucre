using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class Frezze_DDJ_ProjectileLogic :  ProjectileLogic
    {
        [SerializeField] private Animator animator;
        // private int Hit = Animator.StringToHash("DDA_Hit_Jonh_anim");

        public override void Rebound()
        {
            if (m_OnHitParticlePrefab != null ) {
                Instantiate(m_OnHitParticlePrefab , transform.position , Quaternion.identity);
                PlayAudio(Sounds[0]);
            }
            StartCoroutine(Coro_Balldp(DestroyAfterHit));
        }
        protected override void OnTriggerEnter(Collider collider) {

            if (collider.CompareTag("Projectile")){
                Rebound();
                return;
            }
            if (collider.CompareTag("HitBox")){
                Rebound();
                return;
            }

            if (collider.CompareTag("HurtBox")){

                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if (targetNetObj != null ){
                    if( ( targetNetObj.NetworkObjectId != m_SpawnerId) )
                    {
                        // Debug.Log(targetNetObj);

                        AttackDataSend Atk_data = new AttackDataSend();
                        Atk_data.Amount_injury = ProjectileDamage[0].damageAmount;
                        Atk_data.Effect = (byte)ProjectileDamage[0].Effect;
                        Atk_data.Direction = new Vector3(ProjectileDamage[0].Dirxyz.x * transform.right.x , ProjectileDamage[0].Dirxyz.y,ProjectileDamage[0].Dirxyz.z) ;
                        Atk_data.BDefense_p = ProjectileDamage[0].Bdefend;
                        Atk_data.Fall_p = ProjectileDamage[0].fall;
                        targetNetObj.ReceiveHP(Atk_data);
                        // CanMove = false;
                        // animator.Play(Hit);
                        if (ProjectileDamage[0].SoundHit.Length > 0 )PlayAudio(ProjectileDamage[0].SoundHit[0], transform.position);
                        // m_Started = false;
                        StartCoroutine(Coro_Balldp(DestroyAfterHit));

                    }
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

