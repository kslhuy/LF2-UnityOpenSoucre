using System;
using System.Collections.Generic;
using LF2.Utils;
using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    /// <summary>
    /// Common data and RPCs for the CharSelect stage.
    /// </summary>
    public class CharSelectData : NetworkBehaviour
    {
        public enum SeatState : byte
        {
            Inactive,
            Active,
            LockedIn,
        }

        

        /// <summary>
        /// Describes one of the players in the lobby, and their current character-select status.
        /// </summary>
        public struct LobbyPlayerState : INetworkSerializable, IEquatable<LobbyPlayerState>
        {
            public ulong ClientId;

            private FixedPlayerName m_PlayerName; // I'm sad there's no 256Bytes fixed list :(

            public int PlayerNumber; // this player's assigned "P#". (0=P1, 1=P2, etc.)
            public CharacterTypeEnum PlayerChamp; // the latest seat they were in. -1 means none
            // public float LastChangeTime;

            public SeatState SeatState;
            public TeamType PlayerTeam;

            

            // Constructeur 
            public LobbyPlayerState(ulong clientId, string name, int playerNum, SeatState state,TeamType teamType = 0, CharacterTypeEnum champ = 0, float lastChangeTime = 0)
            {
                ClientId = clientId;
                PlayerNumber = playerNum;
                SeatState = state;
                PlayerChamp = champ;
                PlayerTeam = teamType;
                // LastChangeTime = lastChangeTime;
                m_PlayerName = new FixedPlayerName();
                PlayerName = name;
            }

            public string PlayerName
            {
                get => m_PlayerName;
                private set => m_PlayerName = value;
            }


            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref m_PlayerName);
                serializer.SerializeValue(ref PlayerNumber);
                serializer.SerializeValue(ref PlayerTeam);
                serializer.SerializeValue(ref SeatState);
                serializer.SerializeValue(ref PlayerChamp);
                // serializer.SerializeValue(ref LastChangeTime);
            }

            public bool Equals(LobbyPlayerState other)
            {
                return ClientId == other.ClientId &&
                       m_PlayerName.Equals(other.m_PlayerName) &&
                       PlayerNumber == other.PlayerNumber &&
                       PlayerTeam == other.PlayerTeam &&
                       PlayerChamp == other.PlayerChamp &&
                    //    LastChangeTime.Equals(other.LastChangeTime) &&
                       SeatState == other.SeatState;
            }
        }

        private NetworkList<LobbyPlayerState> m_LobbyPlayers;
        private NetworkList<LobbyPlayerState> m_LobbyBOTs;


        public AvatarRegistry AvatarRegistry;

        private Dictionary<CharacterTypeEnum, Avatar> m_AvatarByHero;

        public Dictionary<CharacterTypeEnum, Avatar> AvatarByHero{
            get
                {
                    if( m_AvatarByHero == null )
                    {
                        m_AvatarByHero = new Dictionary<CharacterTypeEnum, Avatar>();
                        // Hoi bi rac roi cach viet
                        // co 1 list SkillsDescription o tren , lay tung cai 1 .
                        foreach (Avatar avatar in AvatarRegistry.m_Avatars)
                        {
                            if (m_AvatarByHero.ContainsKey(avatar.CharacterClass.CharacterType))
                            {
                                throw new System.Exception($"Duplicate PlayerChamp definition detected: {avatar.CharacterClass.CharacterType}");
                            }
                            m_AvatarByHero[avatar.CharacterClass.CharacterType] = avatar;
                        }
                    }
                    return m_AvatarByHero;
                }
        }



        private void Awake()
        {
            m_LobbyPlayers = new NetworkList<LobbyPlayerState>();
            m_LobbyBOTs = new NetworkList<LobbyPlayerState>();
        }

        /// <summary>
        /// Current state of all players in the lobby.
        /// </summary>
        public NetworkList<LobbyPlayerState> LobbyPlayers => m_LobbyPlayers;
        public NetworkList<LobbyPlayerState> LobbyBOTs => m_LobbyBOTs;

        public NetworkVariable<int> IndexBOT ;



        /// <summary>
        /// When this becomes true, the lobby is closed and in process of terminating (switching to gameplay).
        /// </summary>
        public NetworkVariable<bool> IsLobbyClosed { get; } = new NetworkVariable<bool>(false);

        /// <summary>
        /// Server notification when a client requests a different lobby-seat, or locks in their seat choice
        /// </summary>
        public event Action<ulong, CharacterTypeEnum ,TeamType,bool> OnClientChangedSomeThing;

        /// <summary>
        /// RPC to notify the server that a client has chosen a seat or change team .
        /// </summary>

        [ServerRpc(RequireOwnership = false)]
        public void ChangeSomeThingServerRpc(ulong clientId, CharacterTypeEnum ChampId, TeamType teamType, bool lockedIn)
        {
            OnClientChangedSomeThing?.Invoke(clientId,ChampId,teamType, lockedIn);
        }

    }
}