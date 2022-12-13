using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class DUJ_BiscuitVizClient : ClienVizProjectileLogic 
    {

        [SerializeField]
        private NetworkProjectileState m_NetState;

        [SerializeField] Animator _animator;
  


        public override void OnNetworkSpawn()
        {
            m_NetState.HitEnemyEvent += RunAfterEffet; 

            // m_NPCLayer = LayerMask.NameToLayer("NPCs");
        }

    

        private void RunAfterEffet(ulong IdTaget)
        {
                // Time to return to the pool from whence it came.
                _animator.SetTrigger("AfterEffet");
                // NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                // networkObject.Despawn();    
        }

    }
}

