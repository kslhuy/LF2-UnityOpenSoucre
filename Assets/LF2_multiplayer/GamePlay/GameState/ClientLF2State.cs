using System;
using LF2.Gameplay.GameState;
using TMPro;
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

        [SerializeField]
        private NetLF2State _NetLF2State;
        
        public Transform TransformCamera;
        
        [SerializeField]
        private Transform BackGroundSpwanPoint;
        [SerializeField]
        private BackGroundGameRegistry m_BackGroundResigtry ;

        [SerializeField] TextMeshProUGUI Text_GameEnd;
        [SerializeField] GameObject SummaryTable;



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
