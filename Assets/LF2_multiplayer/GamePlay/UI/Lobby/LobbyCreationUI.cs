using Unity.Multiplayer.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace LF2.Gameplay.UI
{
    public class LobbyCreationUI : MonoBehaviour
    {
        [SerializeField] InputField m_LobbyNameInputField;
        [SerializeField] GameObject m_LoadingIndicatorObject;
        [SerializeField] Toggle m_IsPrivate;
        [SerializeField] CanvasGroup m_CanvasGroup;
        LobbyUIMediator m_LobbyUIMediator;

        void Awake()
        {
            EnableUnityRelayUI();
        }

        [Inject]
        void InjectDependencies(LobbyUIMediator lobbyUIMediator)
        {
            // Debug.Log("inject Lobby Creation");
            m_LobbyUIMediator = lobbyUIMediator;
        }

        void EnableUnityRelayUI()
        {
            m_LoadingIndicatorObject.SetActive(false);
        }

        public void OnCreateClick()
        {
            m_LobbyUIMediator.CreateLobbyRequest(m_LobbyNameInputField.text, m_IsPrivate.isOn);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            m_CanvasGroup.alpha = 1f;
            m_CanvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            // m_CanvasGroup.alpha = 0f;
            m_CanvasGroup.blocksRaycasts = false;
        }
    }
}
