using System;
using LF2.Client;
using Unity.Netcode;
using VContainer;

namespace LF2.Server
{
    public class NetworkWinState : NetworkBehaviour
    {
        public NetworkVariable<WinState> WinState = new NetworkVariable<WinState>();

        // [Inject]
        // public void Construct(PersistentGameState persistentGameState)
        // {
        //     if (NetworkManager.Singleton.IsServer)
        //     {
        //         WinState.Value = persistentGameState.WinState;
        //     }
        // }
    }
}
