using System;
using System.Collections.Generic;
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
        
        [SerializeField] PersistentPlayerRuntimeCollection m_PersistentPlayerCollection;

         
        
        NetworkTimer networkTimer;



        // Netcode general
        public NetworkTimer NetworkTimer;
        private bool gameStart;
        const float k_serverTickRate = 60f; // 60 FPS
        const int k_bufferSize = 1024;

                        

        



        
        protected override void Awake()
        {
            EndGameEventLisener.OnEventRaised += EndGame;
        }


        protected override void Start()
        {
            networkTimer = new NetworkTimer(k_serverTickRate);


            // var persistentPlayerExists = m_PersistentPlayerCollection.TryGetPlayer(0 ,out PersistentPlayer persistentPlayer);

            // for (int b = 0; b < persistentPlayer.NumberBOTsInGame(); b++){

            //     clientStateBuffer = new CircularBuffer<StatePackage>(k_bufferSize);
            //     clientInputBuffer = new CircularBuffer<InputPackage>(k_bufferSize);

            // }

        }

        private void Update(){
            if (gameStart){
                networkTimer.Update(Time.deltaTime);
                if (networkTimer.ShouldTick()){
                    // Debug.Log(NbPlayer.GetCharacter().Count); 
                    for ( int i = 0 ; i < NbPlayer.GetCharacter().Count ; i++ ){
                        NbPlayer.GetCharacter()[i].HandleRollbacks(); 
                        NbPlayer.GetCharacter()[i].SaveGameState(networkTimer.CurrentTick , local : true);
                        NbPlayer.GetCharacter()[i].Tick(networkTimer.CurrentTick);

                    }
                }
            }

        }


        // Server confirm that all characters have been spawns 
        [ClientRpc]
        public void StartPlay(){
            gameStart = true;
        } 



        public void EndGame(){
            _GameEndUI.SetActive(true);
        }

    }

}
