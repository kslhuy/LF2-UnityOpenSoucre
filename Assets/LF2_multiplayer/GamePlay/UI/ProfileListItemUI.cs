using TMPro;
using LF2.Shared;
using LF2.Client;
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
            // Debug.Log(m_ProfileNameText.text);
            ClientMusicPlayer.Instance.PlaySoundOK();
            m_ProfileManager.Profile = m_ProfileNameText.text;
        }

        public void OnDeleteClick()
        {
            m_ProfileManager.DeleteProfile(m_ProfileNameText.text);
        }
    }
}
