using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
namespace LF2.Client{


    public class DDJ_John_Shield__Logic : ProjectileLogic
    {
        
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] Animator _animator;
        
        private float timerSounds = 0.5f;

        private float timeNow;
        private float timeStart;

        private bool finish;


        public override void Initialize(ulong creatorsNetworkObjectId, TeamType team ,Vector3 dir_ToMove, Vector3 rotation = default )
        {
            base.Initialize(creatorsNetworkObjectId, team ,dir_ToMove );
            timeStart = timeNow = Time.time;
            finish = false;
        }
        public override void FixedUpdate() {
            if (Time.time - timeStart > DestroyAfterSec && !finish  ){
                finish = true;
                StartCoroutine(RunAfterEffet(DestroyAfterHit));                  
                return;        
            }
            if (Time.time - timeNow > timerSounds){
                PlayAudio(Sounds[1]);

                timeNow = Time.time;
            }
        }

        public override void Rebound(){
        }

        protected override void OnTriggerEnter(Collider collider) {

            if (collider.CompareTag("Projectile")){
                collider.GetComponent<ProjectileLogic>().Rebound();
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
                        Atk_data.Direction = new Vector3(ProjectileDamage[0].Dirxyz.x * transform.right.x , ProjectileDamage[0].Dirxyz.y,ProjectileDamage[0].Dirxyz.z) ;
                        Atk_data.Amount_injury = ProjectileDamage[0].damageAmount;

                        Atk_data.BDefense_p = ProjectileDamage[0].Bdefend;
                        Atk_data.Fall_p = ProjectileDamage[0].fall;
                        Atk_data.Effect = (byte)ProjectileDamage[0].Effect;

                        targetNetObj.ReceiveHP(Atk_data);
                        if (ProjectileDamage[0].SoundHit.Length > 0)   PlayAudio(ProjectileDamage[0].SoundHit[0], transform.position);

                        StartCoroutine(RunAfterEffet(DestroyAfterHit));                  

                    }
                }
            }
        }


        public override IEnumerator RunAfterEffet(float delay){
            // Time to return to the pool from whence it came.
            CanMove = false;
            _animator.Play("Shiled3_John_anim");
            if (Sounds.Length > 0)   PlayAudio(Sounds[0]);
            yield return new WaitForSeconds(delay);
            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();
            // Destroy(this);
                 
        }





        


    }
}
