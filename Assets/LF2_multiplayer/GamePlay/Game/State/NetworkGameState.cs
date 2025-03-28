using System;
using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    public class NetworkGameState : NetworkBehaviour
    {
        [SerializeField]
        TransformVariable m_GameStateTransformVariable;

        // [SerializeField]
        // NetworkWinState m_NetworkWinState;

        // public NetworkWinState NetworkWinState => m_NetworkWinState;

        public NetworkGameMode NetworkGameMode;
        
        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public override void OnNetworkSpawn()
        {
            gameObject.name = "NetworkGameState";

            m_GameStateTransformVariable.Value = transform;
        }

        public override void OnNetworkDespawn()
        {
            m_GameStateTransformVariable.Value = null;
        }
    }
}
