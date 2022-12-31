using Unity.Multiplayer.Infrastructure;
using UnityEngine;

namespace LF2
{
    /// <summary>
    /// A runtime list of <see cref="PersistentPlayer"/> objects that is populated both on clients and server.
    /// </summary>
    [CreateAssetMenu(menuName = "Collection/PersistentBackGround")]

    public class PersistentBackGround : ScriptableObject {
        public NetworkGuid NetworkBackGroundGuid ;
    }
}
