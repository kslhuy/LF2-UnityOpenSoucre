// using System;
// using Unity.Netcode;
// using UnityEngine;

// namespace LF2
// {
//     public enum WinState : byte
//     {
//         Invalid,
//         Win,
//         Loss
//     }

//     /// <summary>
//     /// MonoBehaviour containing only one NetworkVariableBool to represent the game session's win state.
//     /// </summary>
//     public class NetworkWinState : NetworkBehaviour
//     {
//         public NetworkVariable<WinState> winState = new NetworkVariable<WinState>(WinState.Invalid);
//     }
// }
