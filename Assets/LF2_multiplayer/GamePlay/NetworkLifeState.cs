using System;
using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    /// <summary>
    /// MonoBehaviour containing only one NetworkVariable of type LifeState which represents this object's life state.
    /// </summary>
    public class NetworkLifeState : NetworkBehaviour
    {
        [SerializeField]
        NetworkVariable<LifeState> m_LifeState = new NetworkVariable<LifeState>(LF2.LifeState.Alive);

        public NetworkVariable<LifeState> LifeState => m_LifeState;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        /// <summary>
        /// Indicates whether this character is in "god mode" (cannot be damaged).
        /// </summary>
        public NetworkVariable<bool> IsGodMode { get; } = new NetworkVariable<bool>(false);
#endif
    }
}
