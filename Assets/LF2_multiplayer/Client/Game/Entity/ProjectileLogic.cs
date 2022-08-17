using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class ProjectileLogic : NetworkBehaviour , IRebound
    {
        protected bool m_Started = false;

        [SerializeField]
        protected NetworkProjectileState m_NetState;

        [SerializeField]
        protected BoxCollider m_OurCollider;

        [SerializeField]
        protected Rigidbody _rigidbody;
        

        // [SerializeField] Animator _animator;
        /// <summary>
        /// The character that created us. Can be 0 to signal that we were created generically by the server.
        /// </summary>
        protected ulong m_SpawnerId;
        protected TeamType teamAttacker;
        /// <summary>
        /// The data for our projectile. Indicates speed, damage, etc.
        /// </summary>
        protected SkillsDescription.ProjectileInfo m_ProjectileInfo;

        private const int k_MaxCollisions = 4;
        private const float k_WallLingerSec = 2f; //time in seconds that arrows linger after hitting a target.
        private const float k_EnemyLingerSec = 0.2f; //time after hitting an enemy that we persist.
        private Collider[] m_CollisionCache = new Collider[k_MaxCollisions];

        /// <summary>
        /// Time when we should destroy this arrow, in Time.time seconds.
        /// </summary>
        private float m_DestroyAtSec;
        // public LayerMask[] CollisionMask;
        // public LayerMask[] BlockerMask;    //physics mask for things that block the arrow's flight.


        private int m_CollisionMask;  //mask containing everything we test for while moving
        private int m_BlockerMask;    //physics mask for things that block the arrow's flight.
        [HideInInspector]
        public int k_GroundLayerMask;    //physics mask for things that block the arrow's flight.


        private bool canHitCreator;
        private int nbTime_Rebound;

        protected Vector3 _DirToMove;



        /// <summary>
        /// Are we done moving?
        /// </summary>
        protected int m_facing =  1;


        
        [Header("----- Audio Event ------")]

        [SerializeField] private AudioCueEventChannelSO _sfxEventChannel = default;
        [SerializeField] private AudioConfigurationSO _audioConfig = default;
        protected bool CanMove;

        /// <summary>
        /// Set everything up based on provided projectile information.
        /// (Note that this is called before OnNetworkSpawn(), so don't try to do any network stuff here.)
        /// </summary>
        public virtual void Initialize(ulong creatorsNetworkObjectId, TeamType team ,Vector3 dir_ToMove ,in SkillsDescription.ProjectileInfo projectileInfo)
        {
            _DirToMove = dir_ToMove;
            m_facing = (int)dir_ToMove.x;
            CanMove = true;
            canHitCreator = false;
            teamAttacker =  team;
            m_SpawnerId = creatorsNetworkObjectId;
            m_ProjectileInfo = projectileInfo;

        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }
            m_Started = true;

            // m_DestroyAtSec = Time.fixedTime + 3f;

            m_CollisionMask = LayerMask.GetMask(new[] { "HurtBox" });
            m_BlockerMask = LayerMask.GetMask(new[] { "HitBox" });
            k_GroundLayerMask = LayerMask.GetMask(new[] { "Ground" });
            // m_NPCLayer = LayerMask.NameToLayer("NPCs");
        }

        public virtual void FixedUpdate() {

            
            if (!m_Started) { return; } //don't do anything before OnNetworkSpawn has run.
            // Debug.Log(m_facing);
            if (CanMove){
                Vector3 displacement = m_ProjectileInfo.Speed_m_s*Time.deltaTime*m_facing*Vector3.right ;
                _rigidbody.MovePosition(transform.position + displacement);

            }
        }
        // private void Update() {
            
        // }

        public void PlayAudio(AudioCueSO audioCueSO , Vector3 pos = default){
            _sfxEventChannel.RaisePlayEvent(audioCueSO , _audioConfig,pos);
        }
    
        public virtual void Rebound()
        {
            canHitCreator = true;
            m_facing = m_facing*-1;
            transform.Rotate(0.0f,180.0f,0.0f);
        }

        protected virtual void OnTriggerEnter(Collider collider) {
            if (!IsServer)
            {
                return;
            }            
            if (collider.CompareTag("HitBox")){
                // canHitCreator = true;
                Rebound();
                return;
            }
            if (collider.CompareTag("Projectile")){
                Debug.Log("collisio, projectile");
                StartCoroutine(RunAfterEffet(0.1f));
                return;
            }

            if (collider.CompareTag("HurtBox")){

                var targetNetObj = collider.GetComponentInParent<IHurtBox>();

                if (targetNetObj != null ){
                    if( (canHitCreator || targetNetObj.NetworkObjectId != m_SpawnerId) )
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
                        nbTime_Rebound += 1;

                        if (nbTime_Rebound >= m_ProjectileInfo.MaxVictims){
                            if (!IsServer) return;
                            StartCoroutine(RunAfterEffet(m_ProjectileInfo.DestroyAfterSec));                  
                        }
                    }
                }
            }

        }

        protected virtual void OnTriggerExit(Collider collider) {}


        public virtual IEnumerator RunAfterEffet(float delay){
            
            CanMove = false;
            yield return new WaitForSeconds(delay);

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();
                 
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

            //         if (m_HitTargets.Count >= m_ProjectileInfo.MaxVictims)
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
            //             targetNetObj.ReceiveHP(stateRequestData, -m_ProjectileInfo.Damage);                  
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

