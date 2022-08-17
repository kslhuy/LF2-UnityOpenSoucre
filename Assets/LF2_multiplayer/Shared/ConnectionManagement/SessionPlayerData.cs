using UnityEngine;
using Unity.Multiplayer.Samples.BossRoom;
namespace LF2
{
    // USe full in the case some want lose conection , and he want to go in later . 
    // So in this case we have all information in the server
    // PersistentPlayer not dedicate for this case , So that why we created this
    public struct SessionPlayerData : ISessionPlayerData
    {
        public string PlayerName;
        public int PlayerNumber;
        public Vector3 PlayerPosition;
        public Quaternion PlayerRotation;
        public NetworkGuid AvatarNetworkGuid;
        public int CurrentHitPoints;
        public bool HasCharacterSpawned;
        public TeamType PlayerTeam;

        public SessionPlayerData(ulong clientID, string name, NetworkGuid avatarNetworkGuid,TeamType teamType = 0, int currentHitPoints = 0, bool isConnected = false, bool hasCharacterSpawned = false)
        {
            ClientID = clientID;
            PlayerName = name;
            PlayerTeam = teamType;
            PlayerNumber = -1;
            PlayerPosition = Vector3.zero;
            PlayerRotation = Quaternion.identity;
            AvatarNetworkGuid = avatarNetworkGuid;
            CurrentHitPoints = currentHitPoints;
            IsConnected = isConnected;
            HasCharacterSpawned = hasCharacterSpawned;
        }

        public bool IsConnected { get; set; }
        public ulong ClientID { get; set; }

        public void Reinitialize()
        {
            HasCharacterSpawned = false;
        }
    }
}
