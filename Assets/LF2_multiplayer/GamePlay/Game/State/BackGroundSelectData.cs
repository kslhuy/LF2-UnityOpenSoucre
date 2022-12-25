using System;
using UnityEngine;
using Unity.Netcode;

namespace LF2
{
    /// <summary>
    /// Common data and RPCs for the CharSelect stage.
    /// </summary>
    public class BackGroundSelectData : NetworkBehaviour
    {


        public BackGroundGameRegistry backGroundGameRegistry;


        [HideInInspector]
        public NetworkVariable<int> BackGroundNumber = new NetworkVariable<int>(0);
        public NetworkVariable<bool> IsStateChooseBackGround { get; } = new NetworkVariable<bool>(false);

            // Huy Add new here 
        /// <summary>
        /// When this becomes true, the lobby is closed and in process of terminating (switching to gameplay).
        /// </summary>
        public NetworkVariable<LobbyMode> LobbyModeChange { get; } = new NetworkVariable<LobbyMode>(LobbyMode.ChooseBackGround);
        public NetworkVariable<int> NumberBot { get; } = new NetworkVariable<int>();

        
        public event Action< bool> OnHostChangedBackGround;
        public event Action<int> OnHostClickedReady;


        // HOST = player 1




        [ServerRpc]
        public void ChangeBackGroundServerRpc( bool nextLeft)
        {
            OnHostChangedBackGround?.Invoke( nextLeft);
        }

        // // HOST = player 1
        // [ServerRpc]
        // public void StartGameServerRpc()
        // {
        //     OnHostStartGame?.Invoke();
        // }

        [ServerRpc]
        public void HostGameReadyServerRpc(int nbBot)
        {
            OnHostClickedReady?.Invoke(nbBot);
        }

        [ServerRpc]
        public void HostChangeNumberBOTServerRpc(int nbBot)
        {
            NumberBot.Value = nbBot;
        }
    }
}