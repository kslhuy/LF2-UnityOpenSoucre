using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    /// <summary>
    /// NetworkBehaviour containing only one NetworkVariableString which represents this object's name.
    /// </summary>
    public class NetworkTeamState : NetworkBehaviour
    {
        [HideInInspector]
        public NetworkVariable<TeamType> Team = new NetworkVariable<TeamType>();
    }

 
}
