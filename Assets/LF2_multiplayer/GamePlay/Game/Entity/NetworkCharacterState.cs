using System;
using LF2.Client;
using LF2.Utils;
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

        // /// <summary>
        // /// The active target of this character.
        // /// </summary>
        // public NetworkVariable<ulong> TargetId { get; } = new NetworkVariable<ulong>();

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

        public CharacterStateSOs CharacterStateSO => m_CharacterClassContainer.CharacterStateSOs;




        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            HPPoints = CharacterClass.BaseHP.Value;
            MPPoints = CharacterClass.BaseMP.Value;
        }

        public TeamType TryGetTeamType(){
            if (TryGetComponent(out NetworkNameState networkNameState))
                return networkNameState.Team.Value;
            return TeamType.INDEPENDANT;
        }



        // ACTION SYSTEM

        public event Action<StateType > StateDataSync;
        // public event Action<byte> InnerStateDataSync;

        public event Action<StateType> SyncEndAnimation;      

        public event Action<AttackDataSend> RecvHPClient;





        [ClientRpc]
        public void RecvHPClientRPC(AttackDataSend attackDataSend)
        {
            RecvHPClient?.Invoke(attackDataSend);
        }



        // Play Sync State 
        


        [ServerRpc]
        public void AddPredictState_and_SyncServerRpc(StateType state )
        {
            AddPredictState_and_SyncClientRPC(state  );
        }

        [ClientRpc]
        public void AddPredictState_and_SyncClientRPC(StateType state )
        {
            StateDataSync?.Invoke(state );
        }
        // Play Sync State 


        // Play Sync Inner State , that for old systeme
        // [ServerRpc]
        // public void AddPredict_InnerStateServerRpc(byte indexState)
        // {
        //     AddPredict_InnerStateClientRPC(indexState);
        // }


        // [ClientRpc]
        // public void AddPredict_InnerStateClientRPC(byte indexState)
        // {
        //     InnerStateDataSync.Invoke(indexState);
        // }
        // Play Sync Inner State , that for old systeme


        // Play Sync Animation End  , that for some state that have animation loop

        [ServerRpc]
        public void PlayEndAniamtion_SyncServerRpc(StateType state)
        {
            PlayEndAniamtion_SyncClientRPC(state);
        }


        [ClientRpc]
        public void PlayEndAniamtion_SyncClientRPC(StateType state)
        {
            SyncEndAnimation?.Invoke(state);
        }
        // Play Sync Animation End  , that for some state that have animation loop






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





    }
}
