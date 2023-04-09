using UnityEngine;
using TMPro;
using Unity.Multiplayer.Samples.BossRoom.ApplicationLifecycle.Messages;
using Unity.Multiplayer.Infrastructure;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using VContainer;
using LF2.Client;
using LF2.Server;
using System;
using System.Threading.Tasks;
using Unity.Services.Samples.RewardedAds;

namespace LF2.Gameplay.UI
{
    /// <summary>
    /// Provides backing logic for all of the UI that runs in the PostGame stage.
    /// </summary>
    public class PostGameUI : MonoBehaviour
    {

        [SerializeField]
        private TextMeshProUGUI m_WinEndMessage;

        [SerializeField]
        private TextMeshProUGUI m_LoseGameMessage;

        [SerializeField]
        private GameObject m_ReplayButton;

        [SerializeField]
        private GameObject m_WaitOnHostMsg;


        // [SerializeField]
        // private Color m_WinLightColor;

        // [SerializeField]
        // private Color m_LoseLightColor;

        ServerPostGameState m_PostGameState;
        [SerializeField] UlongVariable OnwerClientID;


        [SerializeField] PersistentPlayerRuntimeCollection persistentPlayerRuntimeCollection;

        [Inject]
        void Inject(ServerPostGameState postGameState )
        {
            m_PostGameState = postGameState;

            // only hosts can restart the game, other players see a wait message
            if (NetworkManager.Singleton.IsHost)
            {
                m_ReplayButton.SetActive(true);
                m_WaitOnHostMsg.SetActive(false);
            }
            else
            {
                m_ReplayButton.SetActive(false);
                m_WaitOnHostMsg.SetActive(true);
            }

            
            // if (m_NetworkGameStateTransform && m_NetworkGameStateTransform.Value &&
            //     m_NetworkGameStateTransform.Value.TryGetComponent(out NetworkGameState networkGameState))
            // {
            // }
            

        }
        void Start()
        {
            
            persistentPlayerRuntimeCollection.TryGetPlayer(OnwerClientID.Value, out PersistentPlayer persistentPlayer);
            SetPostGameUI(persistentPlayer.GameWinState.Value);

        //     PersistentPlayer persistentPlayer;
        //     // Debug.Log(persistentPlayer.name);
        //     // if (persistentPlayer.IsOwner)
        //     // {
        //     //     SetPostGameUI(persistentPlayer.WinState);
        //     // }
        //     // m_PostGameState.NetworkPostGame.WinState.OnValueChanged += OnWinStateChanged;
        //     // SetPostGameUI(persistentPlayer.WinState);
        }

        // private void PersistentPlayerRemoved(PersistentPlayer persistentPlayer)
        // {
        //     SetPostGameUI(persistentPlayer.WinState);
        // }

        // private void PersistentPlayerAdded(PersistentPlayer persistentPlayer)
        // {
        //     if (persistentPlayer.IsOwner)
        //     {
        //         SetPostGameUI(persistentPlayer.WinState);
        //     }
        //     // else
        //     // {
        //     //     SetAllyData(clientPlayerAvatar);
        //     // }
        // }

        // void OnWinStateChanged(WinState previousValue, WinState newValue)
        // {
        //     SetPostGameUI(newValue);
        // }

        void SetPostGameUI(WinState winState)
        {
            Debug.Log("Game Win State" + winState);
            // Set end message and background color based last game outcome
            if (winState == WinState.Win)
            {
                m_WinEndMessage.gameObject.SetActive(true);
                m_LoseGameMessage.gameObject.SetActive(false);
            }
            else if (winState == WinState.Loss)
            {
                m_WinEndMessage.gameObject.SetActive(false);
                m_LoseGameMessage.gameObject.SetActive(true);
            }
        }



        public void OnPlayAgainClicked()
        {
            // this should only ever be called by the Host - so just go ahead and switch scenes
            SceneLoaderWrapper.Instance.LoadScene("CharSelect", useNetworkSceneManager: true);

            // FUTURE: could be improved to better support a dedicated server architecture
        }

        public void OnMainMenuClicked()
        {
            m_PostGameState.GoToMainMenu();
        }

        public async void OnClaimLevelEndRewardsButtonPressed()
        {
            try
            {
                // sceneView.SetInteractable(false);
                await DistributeBaseRewards(false);
                if (this == null) return;

                // TODO : disable reward Button 
                // sceneView.CloseCompleteLevelPopup();
            }
            catch (Exception e)
            {
                Debug.Log("A problem occurred while trying to distribute level end rewards: " + e);
            }
            finally
            {
                // sceneView.SetInteractable(true);
            }
        }

        async Task DistributeBaseRewards(bool waitForSecondRewardDistribution)
        {
            await CloudCodeManager.instance.CallGrantLevelEndRewardsEndpoint(waitForSecondRewardDistribution);
        }
    }
}

