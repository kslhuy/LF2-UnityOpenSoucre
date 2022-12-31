using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using System;
using LF2.Utils;

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
        private TeamType teamType;
        // public float m_TimeDelayInput = 0.05f;  // delay 5 ms // 3 frame in 60FPS //
        // private float timeNow;

        [SerializeField]
        [Tooltip("Setting negative value disables destroying object after it is killed.")]
        private float m_KilledDestroyDelaySeconds = 3.0f;


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
            teamType = NetState.TryGetTeamType();
        }


        public override void OnNetworkDespawn()
        {
            if (NetState)
            {
                NetState.OnHPChange -= OnHPChange;
                NetState.OnMPChange -= OnMPChange;            
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


        // For projectil ( Use server authoriza )
        public void ReceiveHP(AttackDataSend attackData)
        {
            // Debug.Log(attackData);
            NetState.RecvHPClientRPC(attackData);
            if (NetState.HPPoints <= 0 ) {

                if (m_KilledDestroyDelaySeconds >= 0.0f && NetState.LifeState != LifeState.Dead)
                {
                    StartCoroutine(KilledDestroyProcess());
                }

                NetState.LifeState = LifeState.Dead;
                
            }
            
        }

        // /// <summary>
        // /// Receive a Life State change that brings Fainted characters back to Alive state.
        // /// </summary>
        // /// <param name="inflicter">Person reviving the character.</param>
        // /// <param name="HP">The HP to set to a newly revived character.</param>
        // public void Revive(ServerCharacter inflicter, int HP)
        // {
        //     if (NetState.LifeState == LifeState.Fainted)
        //     {
        //         NetState.HPPoints = Mathf.Clamp(HP, 0, CharacterClass.BaseHP.Value);
        //         NetState.LifeState.Value = LifeState.Alive;
        //     }
        // }

        IEnumerator KilledDestroyProcess()
        {
            yield return new WaitForSeconds(m_KilledDestroyDelaySeconds);

            if (NetworkObject != null)
            {
                NetworkObject.Despawn(true);
            }
        }


        //  ( Use Client authoriza )
        // Call by Owner client 

        private void OnHPChange(int HP)
        {
            m_NetworkCharacterState.HPPoints = Mathf.Min(m_NetworkCharacterState.CharacterClass.BaseHP.Value, m_NetworkCharacterState.HPPoints+ HP);

            //we can't currently heal a dead character back to Alive state.
            //that's handled by a separate function.
            if (m_NetworkCharacterState.HPPoints <= 0)
            {

                if (m_KilledDestroyDelaySeconds >= 0.0f && NetState.LifeState != LifeState.Dead)
                {
                    StartCoroutine(KilledDestroyProcess());
                }

                NetState.LifeState = LifeState.Dead;            
            }        
        }
        private void OnMPChange(int MP)
        {
            m_NetworkCharacterState.MPPoints = Mathf.Min(m_NetworkCharacterState.CharacterClass.BaseHP.Value, m_NetworkCharacterState.MPPoints + MP);
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
