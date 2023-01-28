using System;
using Unity.Netcode;
using UnityEngine;

namespace LF2.Client
{
    public enum WinState
    {
        Invalid,
        Win,
        Loss
    }

    /// <summary>
    /// Class containing some data that needs to be passed between ServerBossRoomState and PostGameState to represent the game session's win state.
    /// </summary>
    public class ServerWinStatePostGame : NetworkBehaviour
    {
        // [SerializeField] PersistentPlayerRuntimeCollection persistentPlayerRuntimeCollection; 
        // [SerializeField] NetworkLifeState m_NetworkLifeState;
        // // [field:SerializeField] public WinState WinState { get; private set; }

        // public override void OnNetworkSpawn(){
        //     if (!IsServer) enabled = false;
        //     m_NetworkLifeState.LifeState.OnValueChanged += OnLifeStateChanged;
        // }

        // private void OnLifeStateChanged(LifeState previousValue, LifeState newValue)
        // {

        //     // var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerInfo.ClientId);


        // }

        // public void SetWinState(WinState winState)
        // {
        //     WinState = winState;
        // }

        // public void Reset()
        // {
        //     WinState = WinState.Invalid;
        // }
    }
}
