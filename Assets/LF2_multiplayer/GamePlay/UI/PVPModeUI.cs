using System.Collections;
using System;
using UnityEngine;
using LF2.Client;

namespace LF2.Client
{
    /// <summary>
    /// Provides logic for a Hero Action Bar with attack, skill buttons and a button to open emotes panel
    /// This bar tracks button clicks on hero action buttons for later use by ClientInputSender
    /// </summary>

    public class PVPModeUI : MonoBehaviour
    {
        [SerializeField] GameObject LobbyStart;
        [SerializeField] GameObject IPStart;
        [SerializeField] GameObject ProfileStart; 

        public void ActiveButton(bool enabled){
            LobbyStart.gameObject.SetActive(enabled);
            IPStart.gameObject.SetActive(enabled);
            ProfileStart.gameObject.SetActive(enabled);
        }
    }

}
