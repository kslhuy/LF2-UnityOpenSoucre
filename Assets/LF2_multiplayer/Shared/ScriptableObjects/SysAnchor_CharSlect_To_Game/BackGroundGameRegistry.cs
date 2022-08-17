using System;
using UnityEngine;

namespace LF2
{
    /// <summary>
    /// This ScriptableObject defines a Player Character for BossRoom. It defines its CharacterClass field for
    /// associated game-specific properties, as well as its graphics representation.
    /// </summary>
    [CreateAssetMenu(menuName = "Collection/BackGroundRegistry")]
    [Serializable]
    public class BackGroundGameRegistry : ScriptableObject
    {
        [SerializeField]
        public BackGroundGame[] m_BackGrounds ;

    }
}
