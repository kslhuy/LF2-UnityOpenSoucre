using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{
    public class Freeze_DUJ_whirlwind : ProjectileLogic
    {
        [SerializeField] private Animator animator;

        private List<IHurtBox> _Listdamagable = new List<IHurtBox>();
        private float timeNow;
        AttackDataSend Atk_data;

        // private int Hit = Animator.StringToHash("DDA_Hit_Jonh_anim");

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            // if (IsServer){
            // if (Time.time - timeNow > ProjectileDamage[0].vrest)
            // {
            //     foreach (IHurtBox damagable in _Listdamagable)
            //     {
            //         if (damagable != null && damagable.IsDamageable(teamAttacker))
            //         {
            //             damagable.ReceiveHP(Atk_data);
            //         }
            //     }
            //     timeNow = Time.time;
            // }
            // }
        }

        protected override void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("HurtBox"))
            {
                var targetNetObj = collider.GetComponentInParent<IHurtBox>();
                if (targetNetObj != null)
                {
                    if ((targetNetObj.NetworkObjectId != m_SpawnerId))
                    {
                        AttackDataSend Atk_data = new AttackDataSend();
                        Atk_data.Amount_injury = ProjectileDamage[0].damageAmount;
                        Atk_data.Effect = (byte)ProjectileDamage[0].Effect;
                        Atk_data.Direction = new Vector3(ProjectileDamage[0].Dirxyz.x * transform.right.x, ProjectileDamage[0].Dirxyz.y, ProjectileDamage[0].Dirxyz.z);
                        Atk_data.BDefense_p = ProjectileDamage[0].Bdefend;
                        Atk_data.Fall_p = ProjectileDamage[0].fall;
                        targetNetObj.ReceiveHP(Atk_data);
                        if (ProjectileDamage[0].SoundHit.Length > 0) PlayAudio(ProjectileDamage[0].SoundHit[0], transform.position);

                    }
                }
            }
        }

        // protected override void OnTriggerExit(Collider collider)
        // {
        //     IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();
        //     if (damagable != null)
        //     {
        //         _Listdamagable.Remove(damagable);
        //     }
        // }

        // IEnumerator Coro_Balldp(float delay)
        // {
        //     yield return new WaitForSeconds(delay);
        //     NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
        //     networkObject.Despawn();
        // }
    }

}

