using TMPro;
using LF2.Shared;
using Unity.Multiplayer.Samples.BossRoom.Shared.Infrastructure;
using UnityEngine;

namespace LF2.Visual
{
    public class ProfileListItemUI : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_ProfileNameText;

        ProfileManager m_ProfileManager;

        [Inject]
        void InjectDependency(ProfileManager profileManager)
        {
            m_ProfileManager = profileManager;
        }

        public void SetProfileName(string profileName)
        {
            m_ProfileNameText.text = profileName;
        }

        public void OnSelectClick()
        {
            Debug.Log(m_ProfileNameText.text);
            m_ProfileManager.Profile = m_ProfileNameText.text;
            // m_ProfileManager.CurrentProfileName =  m_ProfileNameText.text;
        }

        public void OnDeleteClick()
        {
            m_ProfileManager.DeleteProfile(m_ProfileNameText.text);
        }
    }
}
