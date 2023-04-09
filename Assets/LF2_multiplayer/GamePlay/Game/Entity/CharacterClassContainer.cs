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
                        // Debug.Log("PCs");
                        // Debug.Log(gameObject.layer +" = "+ LayerMask.NameToLayer("PCs")); 
                        // Debug.Log(m_State.RegisteredAvatar.CharacterClass);
                        m_CharacterClass = m_State.RegisteredAvatar.CharacterClass;
                        // Debug.Log(m_CharacterClass);
                    }else{
                        // Debug.Log("get CharacterClass");
                        // Debug.Log(gameObject.layer +" != "+ LayerMask.NameToLayer("PCs"));

                        m_CharacterClass = m_State.RegisteredAvatar.CharacterClassNPC;
                    }
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

        // set only in Server side 
        // so if want to work propety need to set NetworkVaribale also 
        public void SetCharacterClass(CharacterClass characterClass)
        {
            // Debug.Log("Set already");
            m_CharacterClass = characterClass;
            // Debug.Log(m_CharacterClass);
        }
    }
}
