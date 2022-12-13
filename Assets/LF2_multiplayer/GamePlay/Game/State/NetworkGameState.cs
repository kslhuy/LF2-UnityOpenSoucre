using System;
using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    public class NetworkGameState : NetworkBehaviour
    {
        [SerializeField]
        TransformVariable m_GameStateTransformVariable;

        [SerializeField]
        WinState m_NetworkWinState;

        public WinState NetworkWinState => m_NetworkWinState;

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
