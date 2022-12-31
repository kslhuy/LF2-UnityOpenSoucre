using LF2.Gameplay.GameState;
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

        // protected override void OnNetworkSpawn()
        // {
        //     // if (!IsClient)
        //     // {
        //     //     enabled = false;
        //     // }
        // }

    }
}
