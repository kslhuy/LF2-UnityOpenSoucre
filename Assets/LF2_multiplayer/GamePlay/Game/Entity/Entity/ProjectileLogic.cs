using Unity.Netcode;
using System.Collections.Generic;
using System.IO;
using LF2.Server;

using UnityEngine;
using System.Collections;
using System;
using LF2.Utils;

namespace LF2.Client
{

    public class ProjectileLogic : NetworkBehaviour , IRebound 
    {
        protected bool m_Started = false;


        [Header("----- Physics ------")]


        [SerializeField]
        protected BoxCollider m_OurCollider;


        /// <summary>
        /// The character that created us. Can be 0 to signal that we were created generically by the server.
        /// </summary>
        protected ulong m_SpawnerId;
        protected TeamType teamAttacker;
        /// <summary>
        /// The data for our projectile. Indicates speed, damage, etc.
        /// </summary>
        protected SkillsDescription.ProjectileInfo m_ProjectileInfo;


        private int m_CollisionMask;  //mask containing everything we test for while moving
        private int m_BlockerMask;    //physics mask for things that block the arrow's flight.
        [HideInInspector]
        public int k_GroundLayerMask;    //physics mask for things that block the arrow's flight.

        private bool canHitCreator;
        private int hitNumer;

        protected Vector3 _DirToMove;




        /// <summary>
        /// Are we done moving?
        /// </summary>
        protected int m_facing =  1;
        protected bool m_cachedIsServer {get ; private set;}

        [Tooltip("Projectile's speed in meters/second")]
        public DamageDetails[] ProjectileDamage;

        public float Speed_m_s;

        [Tooltip("Maximum range of the Projectile")]
        public float Range;
        [Tooltip("Max number of enemies this projectile can hit before disappearing")]
        public int MaxVictims;

        // Effect affter hit , or rebouding 
        public GameObject m_OnHitParticlePrefab;





        
        [Header("----- Audio Event ------")]

        [SerializeField] private AudioCueEventChannelSO _sfxEventChannel = default;
        [SerializeField] private AudioConfigurationSO _audioConfig = default;
        public AudioCueSO[] Sounds;


        [Header("----- Aniamtion ------")]
        // public Animator animator;
        public string EndAnimation ; 
        protected float timerDestroy;

        public float DestroyAfterSec;
        public float DestroyAfterHit; 


        protected bool CanMove;

        [SerializeField] private ProjectileBoxType m_BoxType;
        public ProjectileBoxType ColliderType => m_BoxType;

        /// <summary>
        /// Set everything up based on provided projectile information.
        /// (Note that this is called before OnNetworkSpawn(), so don't try to do any network stuff here.)
        /// </summary>
        public virtual void Initialize(ulong creatorsNetworkObjectId, TeamType team ,Vector3 dir_ToMove , Vector3 rotation = default )
        {
            _DirToMove = dir_ToMove;
            m_facing = (int)dir_ToMove.x;
            // Debug.Log("m_facing" + m_facing);
            CanMove = true;
            canHitCreator = false;
            teamAttacker =  team;
            m_SpawnerId = creatorsNetworkObjectId;
            if (rotation != default){
                transform.rotation = Quaternion.Euler(rotation);
            }

        }

        public virtual void InitializeData(in SkillsDescription.ProjectileInfo projectileInfo)
        {
            m_ProjectileInfo = projectileInfo;
        }

        public override void OnNetworkSpawn()
        {
            m_CollisionMask = LayerMask.GetMask(new[] { "HurtBox" });
            m_BlockerMask = LayerMask.GetMask(new[] { "HitBox" });
            k_GroundLayerMask = LayerMask.GetMask(new[] { "Ground" });

            m_cachedIsServer = IsServer;
            m_Started = true;
            timerDestroy = Time.time;

        }

        public override void OnNetworkDespawn()
        {
            m_Started = false;
        }

        public virtual void FixedUpdate() {
            if (!m_cachedIsServer || !m_Started) return;

            if (DestroyAfterSec + timerDestroy < Time.time)
            {
                // Time to return to the pool from whence it came.
                var networkObject = gameObject.GetComponent<NetworkObject>();
                networkObject.Despawn();
                return;
            }
            
            if (CanMove){
                
                Vector3 displacement = Speed_m_s*Time.deltaTime*(transform.right + new Vector3(0,0,0.5f*_DirToMove.z));
                // m_Rigidbody.MovePosition(transform.position + displacement);
                // var displacement = transform.forward * (m_ProjectileInfo.Speed_m_s * Time.fixedDeltaTime);
                transform.position += displacement;
            }
            
        }


        public void PlayAudio(AudioCueSO audioCueSO , Vector3 pos = default){
            _sfxEventChannel.RaisePlayEvent(audioCueSO , _audioConfig,pos);
        }
    
        public virtual void Rebound()
        {
            // canHitCreator = true;
            // Debug.Log("Rebound");
            m_facing = m_facing*-1;
            transform.Rotate(0.0f,180.0f,0.0f);
        }



        protected virtual void OnTriggerEnter(Collider collider) {
            // Debug.Log(collider);
            if (!m_cachedIsServer) return; 

            if (collider.CompareTag("HitBox")){
                // canHitCreator = true;
                
                m_SpawnerId = collider.GetComponentInParent<ServerCharacter>().NetworkObjectId;
                // transform.position =  collider.bounds.center; 
                Rebound();
                return;
            }

            if (collider.CompareTag("BlockToRebound")){
                m_SpawnerId = collider.GetComponent<ProjectileLogic>().NetworkObjectId;
                // transform.position =  collider.bounds.center; 
                Rebound();
                return;
            }
        
            if (collider.CompareTag("HurtBox")){

                var targetNetObj = collider.GetComponentInParent<ServerCharacter>();
                // Debug.Log("Hit ID " + targetNetObj.NetworkObjectId);
                // Debug.Log("spawner ID " + m_SpawnerId);
                if (targetNetObj != null ){
                    if( ( targetNetObj.NetworkObjectId != m_SpawnerId) &&  targetNetObj.IsDamageable(teamAttacker) )
                    {
                        // Debug.Log("Hit ID " + targetNetObj.NetworkObjectId);
                        // Debug.Log("spawner ID " + m_SpawnerId);
                        AttackDataSend Atk_data = new AttackDataSend();
                        // Debug.Log("Projecile Dirxyz" + ProjectileDamage[0].Dirxyz);
                        Atk_data.Direction = new Vector3(ProjectileDamage[0].Dirxyz.x * transform.right.x , ProjectileDamage[0].Dirxyz.y,ProjectileDamage[0].Dirxyz.z) ;
                        Atk_data.Amount_injury = ProjectileDamage[0].damageAmount;
                        Atk_data.BDefense_p = ProjectileDamage[0].Bdefend;
                        Atk_data.Fall_p = ProjectileDamage[0].fall;
                        Atk_data.Effect = ((byte)ProjectileDamage[0].Effect);

                        targetNetObj.ReceiveHP(Atk_data);
                                        
                        hitNumer += 1;
                        if (hitNumer >= MaxVictims){
                            StartCoroutine(RunAfterEffet(DestroyAfterHit));                  

                        // PlayAudio(Sounds[0], transform.position);

                        }
                    }
                }
            }

        }


        protected virtual void OnTriggerExit(Collider collider) {}


        public virtual IEnumerator RunAfterEffet(float delay){
            
            CanMove = false;
            
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
            if (m_cachedIsServer){
                NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                networkObject.Despawn();

            }
                 
        }

        public virtual void OnAnimationEvent(int i){
        }
        private void DetectCollisions()
        {
            // Vector3 position = transform.localToWorldMatrix.MultiplyPoint(m_OurCollider.center);
            // int numCollisions = Physics.OverlapSphereNonAlloc(position, m_OurCollider.radius, m_CollisionCache, m_CollisionMask);
            // for (int i = 0; i < numCollisions; i++)
            // {
            //     int layerTest = 1 << m_CollisionCache[i].gameObject.layer;
            
            //     if ((layerTest & m_BlockerMask) != 0)
            //     {
            //         //Player succes rebound projectil.
            //         Rebound();
            //         return;
            //     }
            //     if (!m_HitTargets.Contains(m_CollisionCache[i].gameObject))
            //     {
            //         m_HitTargets.Add(m_CollisionCache[i].gameObject);

            //         if (m_HitTargets.Count >= MaxVictims)
            //         {
            //             // we've hit all the enemies we're allowed to! So we're done
            //             m_DestroyAtSec = Time.fixedTime + k_EnemyLingerSec;
            //             m_IsDead = true;
            //         }

            //         // all NPC layer entities should have one of these.
            //         var targetNetObj = m_CollisionCache[i].GetComponent<IHurtBox>();
            //         if (targetNetObj != null)
            //         {
            //             Debug.Log(targetNetObj);
            //             m_NetState.RecvHitEnemyClientRPC(targetNetObj.NetworkObjectId);
            //             InputPackage stateRequestData = new InputPackage();
            //             stateRequestData.StateTypeEnum = StateType.Hurt1;
            //             targetNetObj.ReceiveHP(stateRequestData, -Damage);                  
            //         }

            //         if (m_IsDead)
            //             return; // don't keep examining collisions since we can't damage anybody else
            //     }
            // }
        }

        public bool IsReboundable()
        {
            
            return true;
        }


    }
}

