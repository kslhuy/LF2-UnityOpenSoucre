using Unity.Netcode;
using UnityEngine;
using Unity.Multiplayer.Samples.BossRoom;
using System;
using Unity.Multiplayer.Infrastructure;
using LF2.Utils;

namespace LF2
{
    /// <summary>
    /// NetworkBehaviour that represents a player connection and is the "Default Player Prefab" inside Netcode for
    /// GameObjects' NetworkManager. This NetworkBehaviour will contain several other NetworkBehaviours that should
    /// persist throughout the duration of this connection, meaning it will persist between scenes.
    /// </summary>
    [Serializable]
    public struct PersistentBOT 
    {
        public string Name ;
        public TeamType TeamType ;

        public CharacterTypeEnum CharacterType ;
        public PersistentBOT(string name, TeamType teamType , CharacterTypeEnum characterType)
        {
            Name = name;
            TeamType = teamType;
            CharacterType = characterType;
        }

    }
}
