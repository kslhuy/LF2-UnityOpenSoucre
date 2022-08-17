using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;

namespace LF2.Server
{

    public class ServerProjectileLogic : NetworkBehaviour 
    {
        private bool m_Started = false;

        [SerializeField]
        private NetworkProjectileState m_NetState;

        [SerializeField]
        private SphereCollider m_OurCollider;

        /// <summary>
        /// The character that created us. Can be 0 to signal that we were created generically by the server.
        /// </summary>
        private ulong m_SpawnerId;

        /// <summary>
        /// The data for our projectile. Indicates speed, damage, etc.
        /// </summary>
        private SkillsDescription.ProjectileInfo m_ProjectileInfo;

        private const int k_MaxCollisions = 4;
        private const float k_WallLingerSec = 2f; //time in seconds that arrows linger after hitting a target.
        private const float k_EnemyLingerSec = 0.2f; //time after hitting an enemy that we persist.
        private Collider[] m_CollisionCache = new Collider[k_MaxCollisions];

        /// <summary>
        /// Time when we should destroy this arrow, in Time.time seconds.
        /// </summary>
        private float m_DestroyAtSec;

        private int m_CollisionMask;  //mask containing everything we test for while moving
        private int m_BlockerMask;    //physics mask for things that block the arrow's flight.

        private bool canHitCreator;

        private List<GameObject> m_HitTargets = new List<GameObject>();

        /// <summary>
        /// Are we done moving?
        /// </summary>
        private bool m_IsDead;
        private int m_facing =  1;

        public float Speed = 0.5f;

        /// <summary>
        /// Set everything up based on provided projectile information.
        /// (Note that this is called before OnNetworkSpawn(), so don't try to do any network stuff here.)
        /// </summary>
        public void Initialize(ulong creatorsNetworkObjectId, in SkillsDescription.ProjectileInfo projectileInfo)
        {
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

            m_HitTargets = new List<GameObject>();
            m_IsDead = false;

            m_DestroyAtSec = Time.fixedTime + 3f;

            m_CollisionMask = LayerMask.GetMask(new[] { "HurtBox" });
            m_BlockerMask = LayerMask.GetMask(new[] { "HitBox" });
            // m_NPCLayer = LayerMask.NameToLayer("NPCs");
        }

        private void FixedUpdate()
        {
            if (!m_Started) { return; } //don't do anything before OnNetworkSpawn has run.
            // Debug.Log(m_facing);
            Vector3 displacement = Speed*Time.fixedDeltaTime*m_facing*transform.right   ;
            transform.position += displacement;

            // DetectCollisions();

            // if (m_DestroyAtSec < Time.fixedTime)
            // {
            //     // Time to return to the pool from whence it came.
            //     NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            //     networkObject.Despawn();
            // }

        }
        public void Rebound()
        {
            canHitCreator = true;
            // m_facing = m_facing*-1;
            transform.Rotate(0.0f,180.0f,0.0f);
        }

        // private void OnTriggerEnter(Collider other) {
        //     var targetNetObj = other.GetComponent<IHurtBox>();
        //     if (targetNetObj != null ){
        //         if( (canHitCreator || targetNetObj.NetworkObjectId != m_SpawnerId) )
        //         {
        //             canHitCreator = true;
        //             // Debug.Log(targetNetObj);
        //             m_NetState.RecvHitEnemyClientRPC(targetNetObj.NetworkObjectId);
        //             InputPackage stateRequestData = new InputPackage();
        //             stateRequestData.StateTypeEnum = StateType.Hurt;
        //             targetNetObj.ReceiveHP(stateRequestData, -m_ProjectileInfo.Damage);                  
        //         }
        //     }

        //     }

        // }

        // private void DetectCollisions()
        // {
        //     Vector3 position = transform.localToWorldMatrix.MultiplyPoint(m_OurCollider.center);
        //     int numCollisions = Physics.OverlapSphereNonAlloc(position, m_OurCollider.radius, m_CollisionCache, m_CollisionMask);
        //     for (int i = 0; i < numCollisions; i++)
        //     {
        //         int layerTest = 1 << m_CollisionCache[i].gameObject.layer;
            
        //         if ((layerTest & m_BlockerMask) != 0)
        //         {
        //             //Player succes rebound projectil.
        //             Rebound();
        //             return;
        //         }
        //         if (!m_HitTargets.Contains(m_CollisionCache[i].gameObject))
        //         {
        //             m_HitTargets.Add(m_CollisionCache[i].gameObject);

        //             if (m_HitTargets.Count >= m_ProjectileInfo.MaxVictims)
        //             {
        //                 // we've hit all the enemies we're allowed to! So we're done
        //                 m_DestroyAtSec = Time.fixedTime + k_EnemyLingerSec;
        //                 m_IsDead = true;
        //             }

        //             // all NPC layer entities should have one of these.
        //             var targetNetObj = m_CollisionCache[i].GetComponent<IHurtBox>();
        //             if (targetNetObj != null)
        //             {
        //                 Debug.Log(targetNetObj);
        //                 m_NetState.RecvHitEnemyClientRPC(targetNetObj.NetworkObjectId);
        //                 InputPackage stateRequestData = new InputPackage();
        //                 stateRequestData.StateTypeEnum = StateType.Hurt1;
        //                 targetNetObj.ReceiveHP(stateRequestData, -m_ProjectileInfo.Damage);                  
        //             }

        //             if (m_IsDead)
        //                 return; // don't keep examining collisions since we can't damage anybody else
        //         }
        //     }
        // }

    }
}

