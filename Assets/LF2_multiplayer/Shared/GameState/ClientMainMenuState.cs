using System;
using BossRoom.Scripts.Shared.Net.UnityServices.Auth;
using LF2.Shared;
using Unity.Multiplayer.Samples.BossRoom.Shared.Infrastructure;
using Unity.Multiplayer.Samples.BossRoom.Shared.Net.UnityServices.Lobbies;
using LF2.Visual;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace LF2.Client
{
    /// <summary>
    /// Game Logic that runs when sitting at the MainMenu. This is likely to be "nothing", as no game has been started. But it is
    /// nonetheless important to have a game state, as the GameStateBehaviour system requires that all scenes have states.
    /// </summary>
    /// <remarks> OnNetworkSpawn() won't ever run, because there is no network connection at the main menu screen.
    /// Fortunately we know you are a client, because all players are clients when sitting at the main menu screen.
    /// </remarks>
    public class ClientMainMenuState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.MainMenu; } }

        // [SerializeField] NameGenerationData m_NameGenerationData;
        [SerializeField] LobbyUIMediator m_LobbyUIMediator;
        [SerializeField] IPUIMediator m_IPUIMediator;
        [SerializeField] Button m_LobbyButton;
        [SerializeField] GameObject m_SignInSpinner;
        [SerializeField] UIProfileSelector m_UIProfileSelector;
        [SerializeField] UITooltipDetector m_UGSSetupTooltipDetector;

        AuthenticationServiceFacade m_AuthServiceFacade;
        LocalLobbyUser m_LocalUser;
        LocalLobby m_LocalLobby;
        ProfileManager m_ProfileManager;

        protected override void Awake()
        {
            m_LobbyButton.interactable = false;
            m_LobbyUIMediator.Hide();
            base.Awake();
        }

        protected override void InitializeScope()
        {
            // Scope.BindInstanceAsSingle(m_NameGenerationData);
            Scope.BindInstanceAsSingle(m_LobbyUIMediator);
            Scope.BindInstanceAsSingle(m_IPUIMediator);
        }

        [Inject]
        async void InjectDependenciesAndInitialize(AuthenticationServiceFacade authServiceFacade, LocalLobbyUser localUser, LocalLobby localLobby, ProfileManager profileManager)
        {
            m_AuthServiceFacade = authServiceFacade;
            m_LocalUser = localUser;
            m_LocalLobby = localLobby;
            m_ProfileManager = profileManager;


            // Debug.Log(m_ProfileManager.AvailableProfiles.Count); 
            if (string.IsNullOrEmpty(Application.cloudProjectId) )
            {
                OnSignInFailed();
                return;
            }


            try
            {
                var unityAuthenticationInitOptions = new InitializationOptions();
                var profile = m_ProfileManager.Profile;
                if (profile.Length > 0)
                {
                    Debug.Log("SetProfile" + profile);
                    unityAuthenticationInitOptions.SetProfile(profile);
                }

                await m_AuthServiceFacade.InitializeAndSignInAsync(unityAuthenticationInitOptions);
                m_ProfileManager.onProfileChanged += OnProfileChanged;
                if (!ProfileAvailable()){
                    return;
                }
                OnAuthSignIn();
                // OnProfileChanged_Asyn();
            }
            catch (Exception)
            {
                OnSignInFailed();
            }

            // void OnAuthSignIn()
            // {
            //     m_LobbyButton.interactable = true;
            //     m_UGSSetupTooltipDetector.enabled = false;
            //     m_SignInSpinner.SetActive(false);

            //     Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");
                
            //     m_LocalUser.ID = AuthenticationService.Instance.PlayerId;
            //     // The local LobbyUser object will be hooked into UI before the LocalLobby is populated during lobby join, so the LocalLobby must know about it already when that happens.
            //     m_LocalLobby.AddUser(m_LocalUser);
            // }


        }

        bool ProfileAvailable(){
            if ( m_ProfileManager.AvailableProfiles.Count == 0){
                Debug.Log("Not have Profile");
                m_UGSSetupTooltipDetector.SetText("You need to Create a profile first");
                OnSignInFailed();
                return false;
            }
            return true;
        }
        void OnSignInFailed()
        {
            if (m_LobbyButton)
            {
                m_LobbyButton.interactable = false;
                m_UGSSetupTooltipDetector.enabled = true;
            }
            if (m_SignInSpinner)
            {
                m_SignInSpinner.SetActive(false);
            }
        }

        public override void OnDestroy()
        {
            m_ProfileManager.onProfileChanged -= OnProfileChanged;
            base.OnDestroy();
        }

        void OnProfileChanged(){
            // if (!ProfileAvailable()){
            //     return;
            // }
            OnProfileChanged_Asyn();
        }

        async void OnAuthSignIn()
        {
            m_LobbyButton.interactable = false;
            m_SignInSpinner.SetActive(true);
            await m_AuthServiceFacade.SwitchProfileAndReSignInAsync(m_ProfileManager.Profile);

            m_LobbyButton.interactable = true;
            m_UGSSetupTooltipDetector.enabled = false;
            m_SignInSpinner.SetActive(false);

            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");
            
            m_LocalUser.ID = AuthenticationService.Instance.PlayerId;
            // The local LobbyUser object will be hooked into UI before the LocalLobby is populated during lobby join, so the LocalLobby must know about it already when that happens.
            m_LocalLobby.AddUser(m_LocalUser);
        }

        async void OnProfileChanged_Asyn()
        {
            m_LobbyButton.interactable = false;
            m_SignInSpinner.SetActive(true);
            await m_AuthServiceFacade.SwitchProfileAndReSignInAsync(m_ProfileManager.Profile);

            m_LobbyButton.interactable = true;
            m_SignInSpinner.SetActive(false);

            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");

            // Updating LocalUser and LocalLobby
            if (m_LocalUser != null){
                m_LocalLobby.RemoveUser(m_LocalUser);
            }
            m_LocalUser.ID = AuthenticationService.Instance.PlayerId;
            m_LocalLobby.AddUser(m_LocalUser);
        }

        // UI Button will trigger that function below 
        // Lobby
        public void OnStartClicked()
        {
            m_LobbyUIMediator.ToggleJoinLobbyUI();
            m_LobbyUIMediator.Show();
        }
        // IP
        public void OnDirectIPClicked()
        {
            m_LobbyUIMediator.Hide();
            m_IPUIMediator.Show();
        }
        // Profile
        public void OnChangeProfileClicked()
        {
            m_UIProfileSelector.Show();
        }
    }
}
