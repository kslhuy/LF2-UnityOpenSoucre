// using System;
// using Unity.Netcode;

// namespace LF2
// {
//     /// <summary>
//     /// Common data and RPCs for the CharSelect stage.
//     /// </summary>
//     public class COMAISelectData : NetworkBehaviour
//     {
//         public struct LobbyCOMState : INetworkSerializable, IEquatable<LobbyPlayerState>
//         {

//             private FixedPlayerName m_PlayerName; // I'm sad there's no 256Bytes fixed list :(

//             public int PlayerNumber; // this player's assigned "P#". (0=P1, 1=P2, etc.)
//             public CharacterTypeEnum PlayerChamp; // the latest seat they were in. -1 means none

//             public SeatState SeatState;
//             public TeamType PlayerTeam;

//                        // Constructeur 
//             public LobbyCOMState(string name, int playerNum, SeatState state,TeamType teamType = 0, CharacterTypeEnum champ = 0)
//             {
//                 PlayerNumber = playerNum;
//                 SeatState = state;
//                 PlayerChamp = champ;
//                 PlayerTeam = teamType;
//                 m_PlayerName = new FixedPlayerName();
//                 PlayerName = name;
//             }

//             public string PlayerName
//             {
//                 get => m_PlayerName;
//                 private set => m_PlayerName = value;
//             }


//             public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//             {
//                 serializer.SerializeValue(ref m_PlayerName);
//                 serializer.SerializeValue(ref PlayerNumber);
//                 serializer.SerializeValue(ref PlayerTeam);
//                 serializer.SerializeValue(ref SeatState);
//                 serializer.SerializeValue(ref PlayerChamp);
//                 // serializer.SerializeValue(ref LastChangeTime);
//             }

//             public bool Equals(LobbyPlayerState other)
//             {
//                 return m_PlayerName.Equals(other.m_PlayerName) &&
//                        PlayerNumber == other.PlayerNumber &&
//                        PlayerTeam == other.PlayerTeam &&
//                        PlayerChamp == other.PlayerChamp &&
//                     //    LastChangeTime.Equals(other.LastChangeTime) &&
//                        SeatState == other.SeatState;
//             }
//         }
    
//         public AvatarRegistry AvatarAIRegistry;


//     }
// }