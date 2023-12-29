using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace LF2.Client
{
    /// <summary>
    /// Attached to the player-characters' prefab, this maintains a list of active ClientCharacterVisualization objects for players.
    /// </summary>
    /// <remarks>
    /// This is an optimization. In Client code you can already get a list of players' ClientCharacterVisualization by
    /// iterating over the active connections and calling GetComponent() on their PlayerObject. But we need
    /// to iterate over all players quite often -- the monsters' IdleAIState does so in every Update() --
    /// and all those GetComponent() calls add up! So this optimization lets us iterate without calling
    /// GetComponent().

    // TODO : This will be refactored with a ScriptableObject-based approach on player collection.
    /// </remarks>
    // [RequireComponent(typeof(ServerCharacter))]
    public class NbPlayer : NetworkBehaviour{

        static List<ClientCharacterVisualization> s_ActiveCharacter = new List<ClientCharacterVisualization>();
        static List<ClientCharacterVisualization> s_ActivePlayers = new List<ClientCharacterVisualization>();

        [SerializeField]
        ClientCharacterVisualization m_CachedClientCharacter;

        public override void OnNetworkSpawn()
        {

            s_ActiveCharacter.Add(m_CachedClientCharacter);
            if (!m_CachedClientCharacter.IsNPC){
                s_ActivePlayers.Add(m_CachedClientCharacter);
            }

        }

        void OnDisable()
        {
            s_ActiveCharacter.Remove(m_CachedClientCharacter);
            if (!m_CachedClientCharacter.IsNPC){
                s_ActivePlayers.Remove(m_CachedClientCharacter);
            }
        }

        /// <summary>
        /// Returns a list of all active Chararater including AI bot ' ServerCharacters. Treat the list as read-only!
        /// The list will be empty on the client.
        /// </summary>
        public static List<ClientCharacterVisualization> GetCharacter()
        {

            return s_ActiveCharacter;
        }

        /// <summary>
        /// Returns a list of all active "Players" . Treat the list as read-only!
        /// The list will be empty on the client.
        /// </summary>
        public static List<ClientCharacterVisualization> GetPlayers()
        {

            return s_ActivePlayers;
        }
    }
}