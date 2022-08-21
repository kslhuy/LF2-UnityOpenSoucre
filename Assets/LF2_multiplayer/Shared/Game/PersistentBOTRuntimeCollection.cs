using UnityEngine;

namespace LF2
{
    /// <summary>
    /// A runtime list of <see cref="PersistentPlayer"/> objects that is populated both on clients and server.
    /// </summary>
    [CreateAssetMenu(menuName = "Collection/PersistentBOTRuntimeCollection", order = 4)]

    public class PersistentBOTRuntimeCollection : RuntimeCollection<PersistentBOT>
    {

    }
}
