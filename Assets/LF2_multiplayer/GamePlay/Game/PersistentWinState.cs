using System.Collections.Generic;
using UnityEngine;

namespace LF2.Client
{
    /// <summary>
    /// A runtime list of <see cref="PersistentPlayer"/> objects that is populated both on clients and server.
    /// </summary>
    [CreateAssetMenu(menuName = "Collection/PersistentWinState")]

    public class PersistentWinState : ScriptableObject
    {
        public WinState winState {get;private set;}
        public void SetWinState(WinState winState )
        {
            this.winState = winState; // 
        }

        public void Reset()
        {
            winState = WinState.Invalid; // 

        }


    }
}
