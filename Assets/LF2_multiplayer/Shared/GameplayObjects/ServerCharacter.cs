using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using System;

namespace LF2.Server
{
    // [RequireComponent(typeof(ServerCharacterMovement), typeof(NetworkCharacterState))]
    public class ServerCharacter : NetworkBehaviour 
    {


        [SerializeField]
        NetworkCharacterState m_NetworkCharacterState;

        public NetworkCharacterState NetState => m_NetworkCharacterState;

        /// <summary>
        /// Returns true if this Character is an NPC.
        /// </summary>
        public bool IsNpc
        {
            get { return NetState.IsNpc; }
        }




        
        // [SerializeField]
        // Dame m_DamageReceiver;

        // [SerializeField]
        // ServerCharacterMovement m_Movement;

        // public ServerCharacterMovement Movement => m_Movement;

        [SerializeField]
        PhysicsWrapper m_PhysicsWrapper;

        public PhysicsWrapper physicsWrapper => m_PhysicsWrapper;

        // Debug State
        public TextMeshPro textMesh;
        private StateType m_lastState;
        // public float m_TimeDelayInput = 0.05f;  // delay 5 ms // 3 frame in 60FPS //
        // private float timeNow;




        // Start Here because the GameObject being intantied when we go to the Scence GamePlay LF2
        private void Start()
        {
            
            // MStateMachinePlayer = new StateMachineServer(this ,m_NetworkCharacterState.CharacterType);
                        // For Debug
            m_lastState = StateType.Idle;
            textMesh.text = m_lastState.ToString();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) { enabled = false; }
            else
            {
                // NetState.NetworkLifeState.LifeState.OnValueChanged += OnLifeStateChanged;
                NetState.OnHPChange += OnHPChange;
                NetState.OnMPChange += OnMPChange;


                // Debug.Log(NetState.HPPoints);
                NetState.HPPoints = NetState.CharacterClass.BaseHP.Value;
                NetState.MPPoints = NetState.CharacterClass.BaseMP.Value;



            }
        }


        public override void OnNetworkDespawn()
        {
            if (NetState)
            {
                // NetState.NetworkLifeState.LifeState.OnValueChanged -= OnLifeStateChanged;
            }

        }



        // private void OnLifeStateChanged(LifeState prevLifeState, LifeState lifeState)
        // {
        //     if (lifeState == LifeState.Fainted)
        //     {
        //         StartCoroutine(FaintedTimer());
        //     }
        // }

        // private IEnumerator FaintedTimer()
        // {
        //     yield return new WaitForSeconds(1);
        //     NetState.LifeState = LifeState.Alive;
        // }



        /// <summary>
        /// Receive an HP change from somewhere. Could be healing or damage.
        /// </summary>
        /// <param name="inflicter">Person dishing out this damage/healing. Can be null. </param>
        /// <param name="HP">The HP to receive. Positive value is healing. Negative is damage.  </param>
        public void OnGameplayActivity(InputPackage stateRequestData, int HP)
        {

        }


        void Update()
        {

            // MStateMachinePlayer.Update();
            // if(m_lastState != MStateMachinePlayer.CurrentState.GetId()){
            //     m_lastState = MStateMachinePlayer.CurrentState.GetId();
            //     textMesh.text = m_lastState.ToString();
            // }

        }

        public void ReceiveHP(InputPackage stateRequestData, int HP)
        {

        }
        private void OnHPChange(int HP)
        {
            m_NetworkCharacterState.HPPoints = Mathf.Min(m_NetworkCharacterState.CharacterClass.BaseHP.Value, m_NetworkCharacterState.HPPoints+ HP);

            //we can't currently heal a dead character back to Alive state.
            //that's handled by a separate function.
            if (m_NetworkCharacterState.HPPoints <= 0)
            {
                m_NetworkCharacterState.LifeState = LifeState.Dead;
            }        
        }
        private void OnMPChange(int MP)
        {
            m_NetworkCharacterState.MPPoints = Mathf.Min(m_NetworkCharacterState.CharacterClass.BaseHP.Value, m_NetworkCharacterState.MPPoints + MP);
        }



        public bool IsDamageable()
        {
            return NetState.NetworkLifeState.LifeState.Value == LifeState.Alive;
        }
    }
}
