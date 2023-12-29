using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using System;
using LF2.Utils;
using LF2.Client;
using System.Collections.Generic;

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


        public StateMachineNew MStateMachinePlayer{ get; private set; }


        [SerializeField]
        PhysicsWrapper m_PhysicsWrapper;

        public PhysicsWrapper physicsWrapper => m_PhysicsWrapper;

        // Debug State
        public TextMeshPro textMesh;
        private StateType m_lastState;
        private TeamType teamType;
        // public float m_TimeDelayInput = 0.05f;  // delay 5 ms // 3 frame in 60FPS //
        // private float timeNow;

        [SerializeField]
        [Tooltip("Setting negative value disables destroying object after it is killed.")]
        private int m_KilledDestroyDelaySeconds = 3;

        [SerializeField] ClientCharacterVisualization clientCharacterVisualization;

        // Netcode server specific
        CircularBuffer<StatePackage> serverStateBuffer;
        List<InputPackage> serverInputQueue;

        const int k_bufferSize = 1024;

        InputPackage lastinputRecv;

        Queue<int> FrameServerPredict;



        



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
                                    // For Debug
            m_lastState = StateType.Idle;
            textMesh.text = m_lastState.ToString();


            teamType = NetState.TryGetTeamType();



            MStateMachinePlayer = new StateMachineNew( clientCharacterVisualization,NetState.CharacterStateSO );

            serverInputQueue = new List<InputPackage>();
            NetState.ServerSaveInput += ServerSaveInput;

            serverStateBuffer = new CircularBuffer<StatePackage>(k_bufferSize);

            lastinputRecv = new InputPackage();


        }


        public override void OnNetworkDespawn()
        {
            if (NetState)
            {
                NetState.OnHPChange -= OnHPChange;
                NetState.OnMPChange -= OnMPChange;            
            }

        }


        private void ServerSaveInput(InputPackage inputpackage)
        {


            // ServerSaveInput  (for simulation in Server side) 
                      
            // var bufferIndex = inputpackage.tick % 1024;
            // m_LastRemoteInputRecieved = inputpackage; 
            // RemoteInputBuffer.Add(inputpackage ,bufferIndex);

            // Debug.Log("REcevie input");

            serverInputQueue.Add(inputpackage);
        }



        public void Tick(int frameNb) {

            // MStateMachinePlayer.OnUpdate();
             
            var bufferIndex = -1;
            InputPackage inputPayload = default;
            
            // Debug.Log("tick");

            //reconciliation
            if (serverInputQueue.Count > 0){
                inputPayload = serverInputQueue[0];
                bufferIndex = inputPayload.tick % k_bufferSize;
                TickServer(inputPayload, doAnticipateState : true);
                SaveGameState(inputPayload.tick );
                serverInputQueue.RemoveAt(0);

            }else{
                inputPayload = lastinputRecv;
                inputPayload.tick = lastinputRecv.tick + 1;
                // Save this predict frame . To Use for Reconcilliation  
                FrameServerPredict.Enqueue(frameNb);

                // Only do Check Should Aniticipate State for some State that may be frequenly do , and this state not impact the game to much
                // Like Move , players tend to clicked move frequely . 
                // Or may be Attack
                if (inputPayload.buttonID == StateType.Move) {
                    inputPayload.buttonID = StateType.Move;
                    TickServer(inputPayload ,doAnticipateState : true);
                }
                else {
                    inputPayload.buttonID = StateType.Idle;
                    TickServer(inputPayload , doAnticipateState : true);
                } 
                SaveGameState(inputPayload.tick );
            }


            
        
            
            // if (bufferIndex == -1) return;
            // HandleExtrapolation(serverStateBuffer.Get(bufferIndex), CalculateLatencyInMillis(inputPayload));
            NetState.SendServerStateToClientRpc(serverStateBuffer.Get(inputPayload.tick));
        
            // update last input from clients
            lastinputRecv = inputPayload;

        }



        public void TickServer(InputPackage inputThisFrame , bool doAnticipateState ) {
            
            (sbyte inputX, sbyte inputZ) = InputSerialization.ConvertDirectionalInputToAxis(inputThisFrame.dir);
            MStateMachinePlayer.SaveMoveInput(inputX , inputZ);
            // TODO : Aniticipate in Server , So don't need send ClientRPC  
            MStateMachinePlayer.ShouldChangeState(ref inputThisFrame.buttonID);

            MStateMachinePlayer.OnUpdate();
            // if (LastStateViz !=  MStateMachinePlayer.CurrentStateViz.GetId()){
            //     LastStateViz = MStateMachinePlayer.CurrentStateViz.GetId();
            //     textMesh.text = LastStateViz.ToString();
            //     // textMesh.text = coreMovement.GetFacingDirection().ToString();
            // } 

            // Need move this code to above or below MStateMachinePlayer.OnUpdate();
            // Dont decide yet 


        }


        public void SaveGameState(int currentTick )
        {
            var bufferIndex = currentTick % 1024;
            StatePackage stateThisFrame = new StatePackage() {
                tick = currentTick,
                StateTypeEnum = MStateMachinePlayer.CurrentStateViz.GetId(),
                Position = transform.position,
                Velocity = clientCharacterVisualization.coreMovement.RB.velocity,
                Rotation_Y = (sbyte)(clientCharacterVisualization.coreMovement.FacingDirection)
            };

            serverStateBuffer.Add(stateThisFrame , bufferIndex);
        }


        
        // if(m_lastState != MStateMachinePlayer.CurrentState.GetId()){
        //     m_lastState = MStateMachinePlayer.CurrentState.GetId();
        //     textMesh.text = m_lastState.ToString();
        // }

    


        // For projectil ( Use server authoriza )
        public void ReceiveHP(AttackDataSend attackData)
        {
            // Debug.Log(attackData);
            NetState.RecvHPClientRPC(attackData);
            OnHPChange(attackData.Amount_injury);

            
        }

        /// <summary>
        /// Receive a Life State change that brings Fainted characters back to Alive state.
        /// </summary>
        /// <param name="inflicter">Person reviving the character.</param>
        /// <param name="HP">The HP to set to a newly revived character.</param>
        public void Revive(ServerCharacter inflicter, int HP)
        {
            if (NetState.LifeState == LifeState.Fainted)
            {
                NetState.HPPoints = Mathf.Clamp(HP, 0, NetState.CharacterClass.BaseHP.Value);
                NetState.LifeState = LifeState.Alive;
            }
        }

        IEnumerator KilledDestroyProcess()
        {
            yield return new WaitForSeconds(m_KilledDestroyDelaySeconds);

            if (NetworkObject != null)
            {
                NetworkObject.Despawn(true);
            }
        }


        //  ( Use Client authoriza )
        // Call by Owner client or server Projectile 

        private void OnHPChange(int HP)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (HP < 0 && m_NetworkCharacterState.NetworkLifeState.IsGodMode.Value ){
            // Don't apply damage if god mode is on
                return;
            }
#endif
            m_NetworkCharacterState.HPPoints = Mathf.Min(m_NetworkCharacterState.CharacterClass.BaseHP.Value, m_NetworkCharacterState.HPPoints+ HP);

            //we can't currently heal a dead character back to Alive state.
            //that's handled by a separate function.
            if (m_NetworkCharacterState.HPPoints <= 0)
            {
                if (IsNpc)
                {
                    if (m_KilledDestroyDelaySeconds >= 0.0f && NetState.LifeState != LifeState.Dead)
                    {
                        StartCoroutine(KilledDestroyProcess());
                    }
                }

                NetState.LifeState = LifeState.Dead;            
            }        
        }
        private void OnMPChange(int MP)
        {
            m_NetworkCharacterState.MPPoints = Mathf.Clamp(m_NetworkCharacterState.MPPoints - MP,0 ,m_NetworkCharacterState.CharacterClass.BaseHP.Value );
        }



        public bool IsDamageable(TeamType teamAttacker)
        {
            if  (NetState.NetworkLifeState.LifeState.Value != LifeState.Alive ) return false; 
            else {
                if (teamType == TeamType.INDEPENDANT || teamType != teamAttacker ) return true; 
                return false;
            }

        }
    }
}
