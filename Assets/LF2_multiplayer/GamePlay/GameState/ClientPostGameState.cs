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




    }
}
