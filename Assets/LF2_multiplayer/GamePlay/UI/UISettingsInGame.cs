using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Multiplayer.Infrastructure;
using Unity.Multiplayer.Samples.BossRoom.ApplicationLifecycle.Messages;
using VContainer;

namespace LF2.Gameplay.UI
{
    /// <summary>
    /// Controls the special Canvas that has the settings icon and the settings window.
    /// The window itself is controlled by UISettingsPanel; the button is controlled here.
    /// </summary>
    public class UISettingsInGame : MonoBehaviour
    {
        [SerializeField] GameObject MasterObject; 
        [SerializeField]
        private GameObject m_SettingsPanelRoot;

        [SerializeField]
        private GameObject m_QuitPanelRoot;


        void Awake()
        {
            // hide the settings window at startup (this is just to handle the common case where an artist forgets to disable the window in the prefab)
            DisablePanels();
        }



        void DisablePanels()
        {
            m_SettingsPanelRoot.SetActive(false);
            m_QuitPanelRoot.SetActive(false);
        }

        /// <summary>
        /// Called directly by the settings button in the UI prefab
        /// </summary>
        public void OnClickSettingsButton()
        {
            m_SettingsPanelRoot.SetActive(!m_SettingsPanelRoot.activeSelf);
            m_QuitPanelRoot.SetActive(false);
        }

        /// <summary>
        /// Called directly by the quit button in the UI prefab
        /// </summary>
        public void OnClickQuitButton()
        {
            m_QuitPanelRoot.SetActive(!m_QuitPanelRoot.activeSelf);
            m_SettingsPanelRoot.SetActive(false);
        }

        public void OnClickControlSetting()
        {
            MasterObject.SetActive(false);
            SceneManager.LoadSceneAsync("DebugUIScene" ,LoadSceneMode.Additive);

        }



    }
}
