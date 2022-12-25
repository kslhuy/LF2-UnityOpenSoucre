using System;
using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    public enum GameMode : byte
    {
        VS,
        Stage,
        
    }

    /// <summary>
    /// MonoBehaviour containing only one NetworkVariableBool to represent the game session's win state.
    /// </summary>
    public class NetworkGameMode : NetworkBehaviour
    {
        public NetworkVariable<GameMode> gameMode = new NetworkVariable<GameMode>(GameMode.VS);
    }
}
