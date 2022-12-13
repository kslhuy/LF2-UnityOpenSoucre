using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

        [SerializeField] public TextMeshProUGUI Text_GameEnd;


        protected override void Awake() {
            _NetLF2State.BackGroundGUID.OnValueChanged += InstantiateBackGround;
            _NetLF2State.GameEnd += GameStateEnd;
        }

        private void GameStateEnd()
        {
            Text_GameEnd.gameObject.SetActive(true);
        }

        public override void OnNetworkSpawn()
        {
            if( !IsClient ) { this.enabled = false; }
        }

        public override void OnNetworkDespawn()
        {
            if( !IsClient ) { 
                _NetLF2State.BackGroundGUID.OnValueChanged -= InstantiateBackGround;
                _NetLF2State.GameEnd -= GameStateEnd; 
            }
        }

        private void InstantiateBackGround(NetworkGuid previousValue, NetworkGuid newValue)
        {
            m_BackGroundResigtry.TryGetBackGround(newValue.ToGuid() , out BackGroundGame backGroundGame); 

            backGroundGame.BackGroundPreFab.InstantiateAsync(BackGroundSpwanPoint);
            
        }
    }

}
