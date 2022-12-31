using TMPro;
using LF2.Client;
using Unity.Multiplayer.Infrastructure;
using UnityEngine;
using LF2.Utils;
using VContainer;

namespace LF2.Gameplay.UI
{
    public class ProfileListItemUI : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_ProfileNameText;

        [Inject] ProfileManager m_ProfileManager;


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
