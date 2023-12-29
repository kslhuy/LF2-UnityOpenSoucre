using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace LF2.Server
{
    /// <summary>
    /// Attached to the player-characters' prefab, this maintains a list of active ServerCharacter objects for players.
    /// </summary>
    /// <remarks>
    /// This is an optimization. In server code you can already get a list of players' ServerCharacters by
    /// iterating over the active connections and calling GetComponent() on their PlayerObject. But we need
    /// to iterate over all players quite often -- the monsters' IdleAIState does so in every Update() --
    /// and all those GetComponent() calls add up! So this optimization lets us iterate without calling
    /// GetComponent(). 
    
    // TODO : This will be refactored with a ScriptableObject-based approach on player collection.

    /// </remarks>
    // [RequireComponent(typeof(ServerCharacter))]
    public class PlayerServerCharacter : NetworkBehaviour
    {
        static List<ServerCharacter> S_ActivePlayers = new List<ServerCharacter>();

        [SerializeField]
        ServerCharacter m_CachedServerCharacter;

        public override void OnNetworkSpawn()
        {
            if( !IsServer )
            {
                enabled = false;
            }
            S_ActivePlayers.Add(m_CachedServerCharacter);

        }

        void OnDisable()
        {
            S_ActivePlayers.Remove(m_CachedServerCharacter);
        }

        /// <summary>
        /// Returns a list of all active players' ServerCharacters. Treat the list as read-only!
        /// The list will be empty on the client.
        /// </summary>
        public static List<ServerCharacter> GetPlayerServerCharacters()
        {

            return S_ActivePlayers;
        }
        public static ServerCharacter GetPlayerServerCharacter(ulong ownerClientId)
        {
            foreach (var playerServerCharacter in S_ActivePlayers)
            {
                if (playerServerCharacter.OwnerClientId == ownerClientId)
                {
                    return playerServerCharacter;
                }
            }
            return null;
        }
    }
}
