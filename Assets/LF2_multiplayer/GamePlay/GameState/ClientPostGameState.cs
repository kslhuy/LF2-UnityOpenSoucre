using System;
using LF2.Gameplay.GameState;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;
using VContainer;


namespace LF2.Client
{
    /// <summary>
    /// Client state-logic for post-game screen. (We don't actually need to do anything here
    /// right now, but we inherit our base-class's OnApplicationQuit() handler.)
    /// </summary>
    public class ClientPostGameState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.PostGame; } }

        [SerializeField]
        NetcodeHooks m_NetcodeHooks;
        [SerializeField] PersistentPlayerRuntimeCollection persistentPlayerRuntimeCollection;

        public WinState winState {private set; get;} 
        protected override void Awake()
        {
            base.Awake();
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;
        }

        private void OnNetworkDespawn()
        {
            m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
        }

        private void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
            }
            Debug.Log("owner client ID" +  m_NetcodeHooks.OwnerClientId); 
            persistentPlayerRuntimeCollection.TryGetPlayer(m_NetcodeHooks.OwnerClientId , out PersistentPlayer persistentPlayer);
            winState = persistentPlayer.GameWinState.Value;
            Debug.Log(winState);
        }

    }
}
