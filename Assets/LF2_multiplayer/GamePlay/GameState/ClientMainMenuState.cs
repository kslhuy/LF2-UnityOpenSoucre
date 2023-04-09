using System;
using LF2.UnityServices.Auth;
using Unity.Multiplayer.Infrastructure;
using Unity.Multiplayer.Lobbies;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityGamingServicesUseCases;
using System.Threading.Tasks;
using LF2.Gameplay.GameState;
using VContainer;
using VContainer.Unity;
using LF2.Gameplay.UI;
using LF2.Utils;
using TMPro;

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
        [Header("----------UI Component---------")]
        [SerializeField] LobbyUIMediator m_LobbyUIMediator;
        [SerializeField] IPUIMediator m_IPUIMediator;
        [SerializeField] Button m_LobbyButton;
        [SerializeField] GameObject m_SignInSpinner;
        [SerializeField] UIProfileSelector m_UIProfileSelector;
        [SerializeField] UITooltipDetector m_UGSSetupTooltipDetector;

        [SerializeField] UIHeroInventory m_UIHeroInventory;

        [SerializeField] MainMenuView m_MainMenuView;

        [Header("-------Unity Game Services Component----------")]

        [SerializeField] EconomyManager economyManager;
        
        //
        // public

        // 

        [Inject] AuthenticationServiceFacade m_AuthServiceFacade;
        [Inject] LocalLobbyUser m_LocalUser;
        [Inject] LocalLobby m_LocalLobby;
        [Inject] ProfileManager m_ProfileManager;


        protected override void Awake()
        {
            base.Awake();

            m_LobbyButton.interactable = false;
            m_LobbyUIMediator.Hide();

            if (string.IsNullOrEmpty(Application.cloudProjectId))
            {
                OnSignInFailed();
                return;
            }
            TryToSignIn();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(m_LobbyUIMediator);
            builder.RegisterComponent(m_IPUIMediator);
        }

        private async void TryToSignIn(){
            // Debug.Log(m_ProfileManager.AvailableProfiles.Count); 


            m_ProfileManager.onProfileChanged += OnProfileChanged;
            await SignIn();


            async Task SignIn()
            {
                try
                {
                    // Debug.Log("InjectDependenciesAndInitialize");

                    // m_ProfileManager.onProfileChanged += OnProfileChanged;
                    // If have already profile in the game 
                    var unityAuthenticationInitOptions = new InitializationOptions();
                    Debug.Log("Initialization and signin processing ....");

                    if (ProfileAvailable()) {
                        var profile = m_ProfileManager.Profile;
                        Debug.Log("SetProfile : " + profile);
                        if (profile.Length > 0)
                        {
                            unityAuthenticationInitOptions.SetProfile(profile);
                        }
                        await m_AuthServiceFacade.InitializeAndSignInAsync(unityAuthenticationInitOptions);
                        Debug.Log("Initialization and signin Complet !!! ");

                        m_LocalUser.ID = AuthenticationService.Instance.PlayerId;
                        m_LocalLobby.AddUser(m_LocalUser);
                        Debug.Log("Load Data processing ...");
                        await LoadDataFromServices();
                        Debug.Log("Load Data Complet !!!");
                        m_LobbyButton.interactable = true;
                        m_UGSSetupTooltipDetector.enabled = false;
                        m_SignInSpinner.SetActive(false);

                    }
                    // First Time play game
                    else {
                        Debug.Log("First Time play + Initialization and signin processing ....");

                        // await m_AuthServiceFacade.InitializeAndSignInAsync(unityAuthenticationInitOptions);
                        // Debug.Log("First Time Sign In.");
                        // await LoadDataFromServices();
                        m_UIProfileSelector.ShowFirstTime();
                    }
                }
                catch (Exception)
                {
                    Debug.Log("error");
                    OnSignInFailed();
                }


            async Task LoadDataFromServices(){
                if (this == null) return;
                Debug.Log($" Player id: {AuthenticationService.Instance.PlayerId}");

                // Economy configuration should be refreshed every time the app initializes.
                // Doing so updates the cached configuration data and initializes for this player any items or
                // currencies that were recently published.
                // 
                // It's important to do this update before making any other calls to the Economy or Remote Config
                // APIs as both use the cached data list. (Though it wouldn't be necessary to do if only using Remote
                // Config in your project and not Economy.)
                // Debug.Log("RefreshEconomyConfiguration");
                if (economyManager == null ) return;
                // await economyManager.RefreshEconomyConfiguration(); // Uncomment that if some day need use cached data
                // Debug.Log("RefreshEconomyConfiguration Compleyt !!!!!");

                // Debug.Log("LoadServicesData processing  ....");

                await LoadServicesData();
                // Debug.Log("LoadServicesData Compleyt !!!!!");
                UpdateSceneViewAfterSignIn();

            }

            bool ProfileAvailable()
            {
                if (m_ProfileManager.AvailableProfiles.Count == 0)
                {
                    // Debug.Log("Not have Profile");
                    m_UGSSetupTooltipDetector.SetText("You need to Create a profile first");
                    // OnSignInFailed();
                    return false;
                }
                return true;
            }
            }
        }

        async Task LoadServicesData()
        {
            await Task.WhenAll(
                CloudSaveManager.instance.LoadAndCacheData(),
                economyManager.RefreshCurrencyBalances()
                // RemoteConfigManager.instance.FetchConfigs()
            );
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

        protected override void OnDestroy()
        {
            m_ProfileManager.onProfileChanged -= OnProfileChanged;
            base.OnDestroy();
        }

        async void OnProfileChanged(){

            Debug.Log("Change Profile");
            await OnProfileChanged_Asyn();
        }

        void UpdateSceneViewAfterSignIn()
        {
            m_MainMenuView.OnSignedIn();
            m_MainMenuView.EnableAndUpdate();
        }



        async Task OnProfileChanged_Asyn()
        {
            m_LobbyButton.interactable = false;
            m_SignInSpinner.SetActive(true);
            await m_AuthServiceFacade.SwitchProfileAndReSignInAsync(m_ProfileManager.Profile);


            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");

            // Economy configuration should be refreshed every time the app initializes.
            // Doing so updates the cached configuration data and initializes for this player any items or
            // currencies that were recently published.
            // 
            // It's important to do this update before making any other calls to the Economy or Remote Config
            // APIs as both use the cached data list. (Though it wouldn't be necessary to do if only using Remote
            // Config in your project and not Economy.)
            await economyManager.RefreshEconomyConfiguration();
            if (this == null) return;

            await LoadServicesData();
            if (this == null) return;

            // Updating LocalUser and LocalLobby
            if (m_LocalUser != null){
                // Debug.Log(m_LocalUser);
                m_LocalLobby.RemoveUser(m_LocalUser);
            }
            m_LocalUser.ID = AuthenticationService.Instance.PlayerId;
            m_LocalLobby.AddUser(m_LocalUser);

            m_UGSSetupTooltipDetector.enabled = false;
            m_LobbyButton.interactable = true;
            m_SignInSpinner.SetActive(false);
        }




        // void UpdatePlayerLevel()
        // {
        //     playerLevel.text = CloudSaveManager.instance.playerLevel.ToString();
        // }
        // void UpdateProgressBar()
        // {
        //     progressBar.maxValue = RemoteConfigManager.instance.levelUpXPNeeded;
        //     progressBar.value = CloudSaveManager.instance.playerXP;
        //     playerXPProgressText.text = $"{CloudSaveManager.instance.playerXP}/{RemoteConfigManager.instance.levelUpXPNeeded}";
        // }



        // UI Button will trigger that function below 
        // Lobby
        public void OnStartLobbyClicked()
        {
            m_LobbyUIMediator.ToggleJoinLobbyUI();
            m_IPUIMediator.Hide();
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


        public void OnHerosClicked()
        {
            m_UIHeroInventory.Show();
        }



    }
}
