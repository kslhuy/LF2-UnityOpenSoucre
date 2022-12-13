using Unity.Netcode;
using UnityEngine;
using Unity.Multiplayer.Samples.BossRoom;
namespace LF2
{
    /// <summary>
    /// NetworkBehaviour that represents a player connection and is the "Default Player Prefab" inside Netcode for
    /// GameObjects' NetworkManager. This NetworkBehaviour will contain several other NetworkBehaviours that should
    /// persist throughout the duration of this connection, meaning it will persist between scenes.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class PersistentPlayer : NetworkBehaviour
    {
        [SerializeField]
        PersistentPlayerRuntimeCollection m_PersistentPlayerRuntimeCollection;
        [SerializeField]
        private PersistentBOTRuntimeCollection m_PersistentBOTRuntimeCollection; 

        public PersistentBOTRuntimeCollection PersistentBOT => m_PersistentBOTRuntimeCollection;

        public PersistentBackGround PersistentBackGround;

        
        [SerializeField]
        NetworkNameState m_NetworkNameState;

        [SerializeField]
        NetworkAvatarGuidState m_NetworkAvatarGuidState;

        // [SerializeField]
        // NetworkBackGroundGuidState m_NetworkBackGroundGuidState;

        public NetworkNameState NetworkNameState => m_NetworkNameState;

        public NetworkAvatarGuidState NetworkAvatarGuidState => m_NetworkAvatarGuidState;
        // public NetworkBackGroundGuidState NetworkBackGroundGuidState => m_NetworkBackGroundGuidState;


        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public override void OnNetworkSpawn()
        {
            
            gameObject.name = "PersistentPlayer " + OwnerClientId;

            // Note that this is done here on OnNetworkSpawn in case this NetworkBehaviour's properties are accessed
            // when this element is added to the runtime collection. If this was done in OnEnable() there is a chance
            // that OwnerClientID could be its default value (0).
            m_PersistentPlayerRuntimeCollection.Add(this);
            if (IsServer)
            {
                
                var sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(OwnerClientId);
                if (sessionPlayerData.HasValue)
                {
                    m_NetworkNameState.Name.Value = sessionPlayerData.Value.PlayerName;
                    // Debug.Log("sessionPlayerData Name :" + sessionPlayerData.Value.PlayerName);
                    m_NetworkNameState.Team.Value = sessionPlayerData.Value.PlayerTeam;
                    m_NetworkAvatarGuidState.AvatarGuid.Value = sessionPlayerData.Value.AvatarNetworkGuid;
                }
            }
        }

        public void AddPersistentBotData(PersistentBOT dataBOT){
            m_PersistentBOTRuntimeCollection.Add(dataBOT);
        }

        public int HowManyBOTData(){
            return m_PersistentBOTRuntimeCollection.Items.Count  ;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemovePersistentPlayer();
        }

        public override void OnNetworkDespawn()
        {
            RemovePersistentPlayer();
        }

        void RemovePersistentPlayer()
        {
            m_PersistentPlayerRuntimeCollection.Remove(this);
            m_PersistentBOTRuntimeCollection.RemoveALL();
            PersistentBackGround.NetworkBackGroundGuid = new NetworkGuid();
            // Double Set Data for ensure we have this 
            if (IsServer)
            {
                var sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(OwnerClientId);
                if (sessionPlayerData.HasValue)
                {
                    var playerData = sessionPlayerData.Value;
                    playerData.PlayerName = m_NetworkNameState.Name.Value;
                    // playerData.PlayerTeam = m_NetworkNameState.Team.Value;
                    playerData.AvatarNetworkGuid = m_NetworkAvatarGuidState.AvatarGuid.Value;
                    SessionManager<SessionPlayerData>.Instance.SetPlayerData(OwnerClientId, playerData);
                }
            }
        }
    }
}
