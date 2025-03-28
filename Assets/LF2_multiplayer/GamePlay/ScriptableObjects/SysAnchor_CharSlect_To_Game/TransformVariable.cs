using UnityEngine;

namespace LF2
{
    /// <summary>
    /// A ScriptableObject which contains a reference to a Transform component. This can be used to remove dependencies
    /// between scene objects.
    /// </summary>

    [CreateAssetMenu(menuName = "Variable/Transform", order = 2)]    
    public class TransformVariable : DescriptionBaseSO
    {
        public Transform Value;
    }
}
