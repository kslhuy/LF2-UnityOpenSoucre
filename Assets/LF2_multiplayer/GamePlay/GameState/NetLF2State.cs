using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Infrastructure;

namespace LF2
{
    /// <summary>
    /// Common data and RPCs for the GameLoop LF2 stage.
    /// </summary>
    public class NetLF2State : NetworkBehaviour
    {

        // [SerializeField]
        // private BackGroundGameRegistry m_BackGroundResigtry ;
        // [SerializeField]
        // private Transform BackGroundSpwanPoint;

        // // [SerializeField] TextMeshProUGUI Text_GameEnd;
        // // [SerializeField] GameObject SummaryTable;


        // public Action GameEnd;
        
        // [ServerRpc]
        // public void EndGameServerRPC(){
        //     GameEnd?.Invoke();
        // }
        
        // [ServerRpc]
        // public void SpawnBackGroundServerRPC(NetworkGuid networkGuid){
        //     m_BackGroundResigtry.TryGetBackGround(networkGuid.ToGuid(), out BackGroundGame backGroundGame);
        //         backGroundGame.BackGroundPreFab.InstantiateAsync(BackGroundSpwanPoint).Completed += (handle) =>
        //         {
        //             var backgroundOBject = handle.Result;
        //             // Debug.Log(backgroundOBject);
        //             // backgroundOBject.GetComponent<NetworkObject>().Spawn(true);
        //             // if (m_NetworkGameState.NetworkGameMode.gameMode.Value == GameMode.Stage){
        //             //     stageManager = backgroundOBject.GetComponent<StageManager>();
        //             //     stageManager.StageFinishEvent += OnStageEndEventMessage;
        //             // }
        //         };
        // }
    }

        

    
}