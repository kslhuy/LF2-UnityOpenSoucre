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
        public bool TryGetBackGround(Guid guid, out BackGroundGame backgroundValue)
        {
            backgroundValue = Array.Find(m_BackGrounds, background => background.Guid == guid);

            return backgroundValue != null;
        }
        public BackGroundGame TryGetBackGround(BackGroundEnum nameBackground )
        {
            BackGroundGame backgroundValue;
            backgroundValue = Array.Find(m_BackGrounds, background => background.NameBackGround == nameBackground);

            return backgroundValue ;
        }
        public BackGroundGame GetRandomBackGround()
        {
            if (m_BackGrounds == null || m_BackGrounds.Length == 0)
            {
                return null;
            }

            return m_BackGrounds[UnityEngine.Random.Range(0, m_BackGrounds.Length)];
            // return m_BackGrounds[0];
        }


    }
}
