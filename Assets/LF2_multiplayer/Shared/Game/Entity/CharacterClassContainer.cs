using UnityEngine;

namespace LF2
{
    /// <summary>
    /// The source of truth for a PC/NPCs CharacterClass.
    /// Player's class is dynamic. NPC's class isn't. This class serves as a single access point for static vs dynamic classes
    /// </summary>
    public class CharacterClassContainer : MonoBehaviour
    {
        [SerializeField]
        CharacterClass m_CharacterClass;

        public CharacterClass CharacterClass
        {
            get
            {
                if (m_CharacterClass == null)
                {
                    // Debug.Log(m_State.RegisteredAvatar.CharacterClass);
                    m_CharacterClass = m_State.RegisteredAvatar.CharacterClass;
                    // Debug.Log(m_CharacterClass);
                }
                // Debug.Log(m_CharacterClass);
                return m_CharacterClass;
            }
        }

        private NetworkAvatarGuidState m_State;

        private void Awake()
        {
            m_State = GetComponent<NetworkAvatarGuidState>();
            // Debug.Log(m_State);
        }

        public void SetCharacterClass(CharacterClass characterClass)
        {
            m_CharacterClass = characterClass;
            // Debug.Log(m_CharacterClass);
        }
    }
}
