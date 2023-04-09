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
        [SerializeField] private string animationName;
        [SerializeField] private int _Nbloop;
        private int _loopTime;
        private List<IHurtBox> _Listdamagable = new List<IHurtBox>();
        private float timeNow;
        AttackDataSend Atk_data;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            PlayAudio(Sounds[0]);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

        }

        protected override void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("HurtBox"))
            {
                // Debug.Log("hurt one s");
                IHurtBox targetNetObj = GetTargetObject(collider , out bool isOKtoUse);
                if(isOKtoUse){
                    AttackDataSend Atk_data = new AttackDataSend();
                    Atk_data.Amount_injury = ProjectileDamage[0].damageAmount;
                    Atk_data.Effect = (byte)ProjectileDamage[0].Effect;
                    Atk_data.Direction = new Vector3(ProjectileDamage[0].Dirxyz.x * transform.right.x, ProjectileDamage[0].Dirxyz.y, ProjectileDamage[0].Dirxyz.z);
                    Atk_data.BDefense_p = ProjectileDamage[0].Bdefend;
                    Atk_data.Fall_p = ProjectileDamage[0].fall;
                    targetNetObj.ReceiveHP(Atk_data);
                    if (ProjectileDamage[0].SoundHit.Length > 0) {
                        PlayAudio(ProjectileDamage[0].SoundHit[0], transform.position);
                    }
                }
            }
            
        }

        public override void OnAnimationEvent(int i)
        {
            _loopTime ++;
            if (_loopTime < _Nbloop){
                animator.Play( animationName , 0,0.333f);
            }
        }



        // IEnumerator Coro_Balldp(float delay)
        // {
        //     yield return new WaitForSeconds(delay);
        //     NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
        //     networkObject.Despawn();
        // }
    }

}

