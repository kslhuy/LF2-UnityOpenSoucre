using LF2.Client;
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
                
                if (m_CharacterClass == null ){
                    if(gameObject.layer == LayerMask.NameToLayer("PCs") )
                    {
                        // Debug.Log(gameObject.layer +" = "+ LayerMask.NameToLayer("PCs")); 
                        // Debug.Log(m_State.RegisteredAvatar.CharacterClass);
                        m_CharacterClass = m_State.RegisteredAvatar.CharacterClass;
                        // Debug.Log(m_CharacterClass);
                    }else{

                        m_CharacterClass = m_State.RegisteredAvatar.CharacterClassNPC;
                    }
                }
                // Debug.Log(m_CharacterClass);
                return m_CharacterClass;
            }
        }
        [SerializeField] CharacterStateSOs m_CharacterStateSOs;
        public CharacterStateSOs CharacterStateSOs
        {
            get
            {
                if (m_CharacterStateSOs == null ){   
                    m_CharacterStateSOs = m_State.RegisteredAvatar.CharacterStateSOs;                    
                }
                return m_CharacterStateSOs;
            }
        }

        private NetworkAvatarGuidState m_State;

        private void Awake()
        {
            m_State = GetComponent<NetworkAvatarGuidState>();
            // Debug.Log(m_State);
        }

        // set only in Server side 
        // so if want to work propety need to set NetworkVaribale also 
        public void SetCharacterClass(CharacterClass characterClass , CharacterStateSOs characterStateSOs)
        {
            // Debug.Log("Set already");
            m_CharacterStateSOs = characterStateSOs;
            m_CharacterClass = characterClass;
            // Debug.Log(m_CharacterClass);
        }
    }
}
