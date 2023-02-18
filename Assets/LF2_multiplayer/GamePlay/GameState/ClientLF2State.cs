using System;
using LF2.Gameplay.GameState;
using LF2.Utils;
using TMPro;
using Unity.Multiplayer.Infrastructure;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace LF2.Client
{

    /// <summary>
    /// Client specialization of core BossRoom game logic.
    /// </summary>
    public class ClientLF2State : GameStateBehaviour
    {
        public override GameState ActiveState {  get { return GameState.LF2_Net; } }

        // [SerializeField]
        // private NetLF2State _NetLF2State;
        
        public Transform TransformCamera;

        [SerializeField]
        private BackGroundGameRegistry m_BackGroundResigtry ;
        [SerializeField]
        private Transform BackGroundSpwanPoint;

        // [SerializeField] TextMeshProUGUI Text_GameEnd;
        // [SerializeField] GameObject SummaryTable;
        [SerializeField] PersistentPlayerRuntimeCollection persistentPlayerRuntimeCollection;

        // public Action GameEnd;
        
        [ServerRpc]
        public void EndGameServerRPC(TeamType teamType){
            Debug.Log("cLient Game End ");
            Debug.Log(teamType);
            // Save win state
            foreach (ClientCharacterVisualization player in NbPlayer.GetPlayers()){
                PersistentPlayer persistentPlayer;
                if (!(player.m_NetState.LifeState == LifeState.Alive) || !(player.m_NetState.TryGetTeamType() == teamType)){
                    persistentPlayerRuntimeCollection.TryGetPlayer(player.OwnerClientId , out persistentPlayer);
                    persistentPlayer.SetWinState(WinState.Loss);
                    continue;
                }
                // 3: Find All Players In this Team  
                persistentPlayerRuntimeCollection.TryGetPlayer(player.OwnerClientId , out persistentPlayer);
                persistentPlayer.SetWinState(WinState.Win);
            }
        }
        
        [ClientRpc]
        public void SpawnBackGroundClientRpc(BackGroundEnum backGroundEnum){
            Debug.Log("Client Spwan BackGround");
            m_BackGroundResigtry.TryGetBackGround(backGroundEnum).BackGroundPreFab.InstantiateAsync(BackGroundSpwanPoint);
            // .Completed += (handle) =>
            // {
            //     var backgroundOBject = handle.Result;
            //     // Debug.Log(backgroundOBject);
            //     // backgroundOBject.GetComponent<NetworkObject>().Spawn(true);
            //     // if (m_NetworkGameState.NetworkGameMode.gameMode.Value == GameMode.Stage){
            //     //     stageManager = backgroundOBject.GetComponent<StageManager>();
            //     //     stageManager.StageFinishEvent += OnStageEndEventMessage;
            //     // }
            // };
        }
        







        // protected override void Awake() {
        //     if (!IsClient)
        //         _NetLF2State.BackGroundGUID.OnValueChanged += InstantiateBackGround;
        //     _NetLF2State.GameEnd += GameStateEnd;
        // }

        // private void GameStateEnd()
        // {
        //     Text_GameEnd.gameObject.SetActive(true);
        //     SummaryTable.SetActive(true);
        // }

        // public override void OnNetworkSpawn()
        // {
        //     if( !IsClient ) { this.enabled = false; }
        // }

        // public override void OnNetworkDespawn()
        // {
        //     if (!IsClient)
        //         _NetLF2State.BackGroundGUID.OnValueChanged -= InstantiateBackGround;
        //     _NetLF2State.GameEnd -= GameStateEnd; 
        // }

        // private void InstantiateBackGround(NetworkGuid previousValue, NetworkGuid newValue)
        // {
        //     if(IsClient){
        //         Debug.Log("InstantiateBackGround");
        //         m_BackGroundResigtry.TryGetBackGround(newValue.ToGuid() , out BackGroundGame backGroundGame); 

        //         backGroundGame.BackGroundPreFab.InstantiateAsync(BackGroundSpwanPoint);
        //     }

            
            
        // }
    }

}
