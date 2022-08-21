using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace LF2
{
    /// <summary>
    /// NetworkBehaviour component to send/receive GUIDs from server to clients.
    /// </summary>
    public class NetworkAvatarGuidState : NetworkBehaviour
    {
        [FormerlySerializedAs("AvatarGuidArray")]
        [HideInInspector]
        // Use full , for save GUID and Use in between scene 
        //  From that GUID , we can extrait data from SessionPlayerData 
        public NetworkVariable<NetworkGuid> AvatarGuid = new NetworkVariable<NetworkGuid>();        
            

        CharacterClassContainer m_CharacterClassContainer;

        [SerializeField]
        AvatarRegistry m_AvatarRegistry;

        Avatar m_Avatar;

        public Avatar RegisteredAvatar
        {
            get
            {
                if (m_Avatar == null)
                {

                    RegisterAvatar(AvatarGuid.Value.ToGuid());
                }

                return m_Avatar;
            }
        }

        private void Awake()
        {
            m_CharacterClassContainer = GetComponent<CharacterClassContainer>();
            // Debug.Log(m_CharacterClassContainer);
        }

        public void RegisterAvatar(Guid guid)
        {
            // Debug.Log("RegisteredAvatar");

            if (guid.Equals(Guid.Empty))
            {
                // not a valid Guid
                return;
            }

            // based on the Guid received, Avatar is fetched from AvatarRegistry
            if (!m_AvatarRegistry.TryGetAvatar(guid, out Avatar avatar))
            {
                Debug.LogError("Avatar not found!");
                return;
            }

            if (m_Avatar != null)
            {
                // already set, this is an idempotent call, we don't want to Instantiate twice
                return;
            }

            m_Avatar = avatar;
            if (gameObject.layer == LayerMask.NameToLayer("PCs"))         m_CharacterClassContainer.SetCharacterClass(avatar.CharacterClass);
            else m_CharacterClassContainer.SetCharacterClass(avatar.CharacterClassNPC);
        }
    }
}
