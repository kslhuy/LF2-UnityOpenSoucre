using Unity.Multiplayer.Infrastructure;
using UnityEngine;
using Unity.Netcode;


namespace LF2
{
    /// <summary>
    /// A runtime list of <see cref="PersistentPlayer"/> objects that is populated both on clients and server.
    /// </summary>
    [CreateAssetMenu(menuName = "Collection/PersistentBackGround")]

    public class PersistentBackGround : ScriptableObject {
        // public NetworkGuid NetworkBackGroundGuid ;
        public NetworkVariable<BackGroundEnum> NetworkBackGround = new NetworkVariable<BackGroundEnum>(); 

    }
}
