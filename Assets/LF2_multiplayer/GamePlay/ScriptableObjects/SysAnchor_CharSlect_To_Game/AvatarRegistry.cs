using System;
using UnityEngine;

namespace LF2
{
    /// <summary>
    /// This ScriptableObject will be the container for all possible Avatars inside BossRoom.
    /// <see cref="Avatar"/>
    /// </summary>
    [CreateAssetMenu(menuName = "Collection/AvatarRegistry", order = 4)]
    public class AvatarRegistry : ScriptableObject
    {
        [SerializeField]
        public Avatar[] m_Avatars;

        public bool TryGetAvatar(Guid guid, out Avatar avatarValue)
        {
            avatarValue = Array.Find(m_Avatars, avatar => avatar.Guid == guid);

            return avatarValue != null;
        }

        public bool TryGetAvatar(CharacterTypeEnum characterType , out Avatar avatarValue ){
            avatarValue = Array.Find(m_Avatars, avatar => avatar.CharacterClass.CharacterType == characterType);
            return avatarValue != null;
        }

        public Avatar GetAvatar(CharacterTypeEnum characterType){
            return  Array.Find(m_Avatars, avatar => avatar.CharacterClass.CharacterType == characterType) ;
        }

        public Avatar GetRandomAvatar()
        {
            if (m_Avatars == null || m_Avatars.Length == 0)
            {
                return null;
            }

            return m_Avatars[UnityEngine.Random.Range(0, m_Avatars.Length)];
            // return m_Avatars[0];
        }
        public Avatar GetSpecifiqueAvatar(int index)
        {
            if (m_Avatars == null || m_Avatars.Length == 0)
            {
                return null;
            }

            // return m_Avatars[UnityEngine.Random.Range(0, m_Avatars.Length)];
            return m_Avatars[index];
        }

    }
}
