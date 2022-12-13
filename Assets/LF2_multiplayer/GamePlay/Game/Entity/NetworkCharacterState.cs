using System;
using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    public enum LifeState
    {
        Alive,
        Fainted,
        Dead,
    }



    /// <summary>
    /// Contains all NetworkVariables and RPCs of a character. This component is present on both client and server objects.
    /// </summary>
    [RequireComponent(typeof(NetworkStaticsPoints), typeof(NetworkLifeState))]
    public class NetworkCharacterState : NetworkBehaviour, ITargetable 
    {


        /// <summary>
        /// Indicates whether this character is in "stealth mode" (invisible to monsters and other players).
        /// </summary>
        public NetworkVariable<bool> IsStealthy { get; } = new NetworkVariable<bool>();

        [SerializeField]
        NetworkStaticsPoints m_NetworkStatics;

        public NetworkStaticsPoints Statics
        {
            get
            {
                return m_NetworkStatics;
            }
        }

        /// <summary>
        /// The active target of this character.
        /// </summary>
        public NetworkVariable<ulong> TargetId { get; } = new NetworkVariable<ulong>();

        /// <summary>
        /// Current HP. This value is populated at startup time from CharacterClass data.
        /// </summary>
        public int HPPoints
        {
            get { return m_NetworkStatics.HPPoints.Value; }
            set { m_NetworkStatics.HPPoints.Value = value; }
        }

        public int MPPoints{
            get { return m_NetworkStatics.MPPoints.Value; }
            set { m_NetworkStatics.MPPoints.Value = value; }
        }

        [SerializeField]
        NetworkLifeState m_NetworkLifeState;

        public NetworkLifeState NetworkLifeState => m_NetworkLifeState;


        public LifeState LifeState
        {
            get => m_NetworkLifeState.LifeState.Value;
            set => m_NetworkLifeState.LifeState.Value = value;
        }

        /// <summary>
        /// Returns true if this Character is an NPC.
        /// </summary>
        public bool IsNpc { get { return CharacterClass.IsNpc; } }

        [HideInInspector]
        public bool IsHOST ;

        public bool IsValidTarget => LifeState != LifeState.Dead;

        /// <summary>
        /// Returns true if the Character is currently in a state where it can play actions, false otherwise.
        /// </summary>
        public bool CanPerformActions => LifeState == LifeState.Alive;

        [SerializeField]
        CharacterClassContainer m_CharacterClassContainer;

        /// <summary>
        /// The CharacterData object associated with this Character. This is the static game data that defines its attack skills, HP, etc.
        /// </summary>
        public CharacterClass CharacterClass => m_CharacterClassContainer.CharacterClass;

        /// <summary>
        /// Character Type. This value is populated during character selection.
        /// </summary>
        public CharacterTypeEnum CharacterType => m_CharacterClassContainer.CharacterClass.CharacterType;

        // [SerializeField]
        // private NetworkNameState m_NameState ; 
        public TeamType TeamType ;


        // /// <summary>
        // /// Gets invoked when inputs are received from the client which own this networked character.
        // /// </summary>
        // public event Action<Vector2,float> ReceivedClientInput;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            HPPoints = CharacterClass.BaseHP.Value;
            MPPoints = CharacterClass.BaseMP.Value;
            IsHOST = IsHost;
        }

        public TeamType TryGetTeamType(){
            if (TryGetComponent(out NetworkNameState networkNameState))
                return networkNameState.Team.Value;
            return TeamType.INDEPENDANT;
        }



        // ACTION SYSTEM


        public event Action<StateType> StateDataSync;
        
        public event Action<InputPackage> InputSendBack;      
        public event Action<float , float> InputMoveSendBack;


        public event Action<NetStatePackage> SyncStateEvent;
        public event Action<AttackDataSend> RecvHPClient;






        [ClientRpc]
        public void AddPredictState_and_SyncClientRPC(StateType state)
        {
            StateDataSync?.Invoke(state);
        }

        [ClientRpc]
        public void RecvHPClientRPC(AttackDataSend attackDataSend)
        {
            RecvHPClient?.Invoke(attackDataSend);
        }

        
        [ServerRpc]
        public void AddPredictState_and_SyncServerRpc(StateType state)
        {
            AddPredictState_and_SyncClientRPC(state);
        }






        [ClientRpc]
        public void MoveInputEventClientRPC(int inputX , int inputZ)
        {

            // DoActionEventServer?.Invoke(data);
            InputMoveSendBack?.Invoke(inputX,inputZ);
        }

        /// <summary>
        /// Client->Server RPC that sends a request to play an action.
        /// </summary>
        /// <param name="data">Data about which action to play and its associated details. </param>
        // [ServerRpc(RequireOwnership = false)]
        // public void DoPassiveActionServerRPC(InputPackage data) {
        //     SendPassiveActionServerRPC?.Invoke(data);
        // } 
            

        // UTILITY AND SPECIAL-PURPOSE RPCs

        public event Action<int> OnHPChange;
        public event Action<int> OnMPChange;
 


        [ServerRpc]
        public void HPChangeServerRpc(int HP)
        {
            OnHPChange?.Invoke(HP);
        }

        [ServerRpc]
        public void MPChangeServerRpc(int MP)
        {
            OnMPChange?.Invoke(MP);
        }

        [ServerRpc]
        public void LifeStateChangeServerRpc(LifeState lifeState)
        {
            LifeState = lifeState;
        }




        /// <summary>
        /// Called on server when the character's client decides they have stopped "charging up" an attack.
        /// </summary>
        public event Action OnStopChargingUpServer;

        /// <summary>
        /// Called on all clients when this character has stopped "charging up" an attack.
        /// Provides a value between 0 and 1 inclusive which indicates how "charged up" the attack ended up being.
        /// </summary>
        public event Action<float> OnStopChargingUpClient;

        [ServerRpc]
        public void RecvStopChargingUpServerRpc()
        {
            OnStopChargingUpServer?.Invoke();
        }

        [ClientRpc]
        public void RecvStopChargingUpClientRpc(float percentCharged)
        {
            OnStopChargingUpClient?.Invoke(percentCharged);
        }


    }
}
