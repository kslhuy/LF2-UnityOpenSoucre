using System;
using LF2.Gameplay.GameState;
using LF2.Utils;
using TMPro;
using Unity.Multiplayer.Infrastructure;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace LF2.Client
{

    /// <summary>
    /// Client specialization of core BossRoom game logic.
    /// </summary>
    public class ClientLF2State : GameStateBehaviour
    {
        public override GameState ActiveState {  get { return GameState.LF2_Net; } }

        [SerializeField] GameObject _GameEndUI;
        // [SerializeField] GameObject SummaryTable;

        [SerializeField] EventChannelSO EndGameEventLisener;
        
        protected override void Awake()
        {
            EndGameEventLisener.OnEventRaised += EndGame;
        }
        public void EndGame(){
            _GameEndUI.SetActive(true);
        }

    }

}
