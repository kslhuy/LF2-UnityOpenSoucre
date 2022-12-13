using Unity.Multiplayer.Samples.BossRoom;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;
using LF2;
using TMPro;
using LF2.Server;
using UnityEngine.Assertions;
using LF2.Client;

namespace Unity.BossRoom.DebugCheats
{
    /// <summary>
    /// Handles debug cheat events, applies them on the server and logs them on all clients. This class is only
    /// available in the editor or for development builds
    /// </summary>
    public class DebugCheatsManager : NetworkBehaviour
    {
        [SerializeField]
        GameObject m_DebugCheatsPanel;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField]
        [Tooltip("Enemy to spawn. Make sure this is included in the NetworkManager's list of prefabs!")]
        NetworkObject m_AIPrefab;

        [SerializeField] NetworkObject m_PlayerPrefab;


        [SerializeField] AvatarRegistry avatarRegistry;
        [SerializeField] Transform spawnPoint;

        [SerializeField] KeyCode m_OpenWindowKeyCode = KeyCode.Slash;

        
        private CharacterTypeEnum BotTypeToSpawn = CharacterTypeEnum.Deep;
        private TeamType BotTeamTypeToSpawn = TeamType.INDEPENDANT;

        const int k_NbTouchesToOpenWindow = 4;

        bool m_DestroyPortalsOnNextToggle = true;

        [Header(" DEBUG ")]
        [SerializeField] BoolEventChannelSO Debug_Toggle_SpwanWave;
        [SerializeField] BoolEventChannelSO Debug_ToggleAiBrain;
                



        // [Inject]
        // IPublisher<CheatUsedMessage> m_CheatUsedMessagePublisher;

        // void Update()
        // {
        //     if (Input.touchCount == k_NbTouchesToOpenWindow && AnyTouchDown() ||
        //         m_OpenWindowKeyCode != KeyCode.None && Input.GetKeyDown(m_OpenWindowKeyCode))
        //     {
        //         m_DebugCheatsPanel.SetActive(!m_DebugCheatsPanel.activeSelf);
        //     }
        // }

        static bool AnyTouchDown()
        {
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    return true;
                }
            }
            return false;
        }

        public void SpawnEnemy()
        {
            SpawnEnemyServerRpc();
        }

        public void SpawnPlayer()
        {
            SpawnPlayerServerRpc();
        }

        public void OnBotTypeChanged(TMP_Dropdown dropdown){
            BotTypeToSpawn =  (CharacterTypeEnum)dropdown.value;
        }
        public void OnBotTeamTypeChanged(TMP_Dropdown dropdown){
            BotTeamTypeToSpawn =  (TeamType)dropdown.value;
        }




        public void KillAllEnemies()
        {
            KillAllEnemiesServerRpc();
        }

        // public void ToggleGodMode()
        // {
        //     ToggleGodModeServerRpc();
        // }

        // public void HealPlayer()
        // {
        //     HealPlayerServerRpc();
        // }

        public void ToggleAIBrain()
        {
            ToggleAIBrainServerRpc();
        }

        public void ToggleSpawnWaves()
        {
            ToggleSpawnWavesServerRpc();
            
        }





        public void GoToPostGame()
        {
            GoToPostGameServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        void SpawnEnemyServerRpc(ServerRpcParams serverRpcParams = default)
        {


            var botNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

            var persistentPlayerExists = botNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer);
            Assert.IsTrue(persistentPlayerExists,
                $"Matching persistent PersistentPlayer for Bot not found!");


            // Find a spawn point 
            // Instaite a Bot Object
            var newBOT = Instantiate(m_AIPrefab);
            var newBOTCharacter = newBOT.GetComponent<ServerCharacter>();

            var physicsTransform = newBOTCharacter.physicsWrapper.Transform;

            //     // Set position and rotation to the Bot Object in the scene
                physicsTransform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

            // Check if the Bot Object have Componenet NetworkAvatarGuidState  
            var networkAvatarGuidStateExists =
                newBOT.TryGetComponent(out NetworkAvatarGuidState networkAvatarGuidState);

            Assert.IsTrue(networkAvatarGuidStateExists,
                $"NetworkCharacterGuidState not found on player avatar!");

            LF2.Avatar _botavatarValue;
            avatarRegistry.TryGetAvatar(BotTypeToSpawn, out _botavatarValue);
            networkAvatarGuidState.RegisterAvatar(_botavatarValue);

            // pass name , team type from persistent player to avatar
            if (newBOT.TryGetComponent(out NetworkNameState networkNameState))
            {
                networkNameState.Name.Value = _botavatarValue.CharacterClass.CharacterType.ToString();
                networkNameState.Team.Value = BotTeamTypeToSpawn;
            }

        
            // newBOT.SpawnWithOwnership(NetworkManager.Singleton.LocalClientId, true);
            newBOT.Spawn( true);

            // PublishCheatUsedMessage(serverRpcParams.Receive.SenderClientId, "SpawnEnemy");

        }

        [ServerRpc(RequireOwnership = false)]
        void SpawnPlayerServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            // Destroy the current player
            var playerServerCharacter = PlayerServerCharacter.GetPlayerServerCharacter(clientId);
            Destroy(playerServerCharacter.gameObject);


            // Spwan new player
            var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);

            var newPlayer = Instantiate(m_PlayerPrefab, Vector3.zero, Quaternion.identity);

            var newPlayerCharacter = newPlayer.GetComponent<ServerCharacter>();

            var physicsTransform = newPlayerCharacter.physicsWrapper.Transform;

            physicsTransform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

            var persistentPlayerExists = playerNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer);
            Assert.IsTrue(persistentPlayerExists,
                $"Matching persistent PersistentPlayer for client {clientId} not found!");

            // pass character type from persistent player to avatar
            var networkAvatarGuidStateExists =
                newPlayer.TryGetComponent(out NetworkAvatarGuidState networkAvatarGuidState);

            Assert.IsTrue(networkAvatarGuidStateExists,
                $"NetworkCharacterGuidState not found on player avatar!");

            LF2.Avatar _PlayeravatarValue;
            avatarRegistry.TryGetAvatar(BotTypeToSpawn, out _PlayeravatarValue);
            networkAvatarGuidState.RegisterAvatar(_PlayeravatarValue);

            // pass name from persistent player to avatar
            if (newPlayer.TryGetComponent(out NetworkNameState networkNameState))
            {
                // Debug.Log("Network Name " + persistentPlayer.NetworkNameState.Name.Value);
                networkNameState.Name.Value = "player";
                // Debug.Log("Network Team " + persistentPlayer.NetworkNameState.Team.Value);
                networkNameState.Team.Value = persistentPlayer.NetworkNameState.Team.Value;

            }

            // spawn players characters with destroyWithScene = true
            newPlayer.SpawnWithOwnership(clientId, true);
        
        }

        // [ServerRpc(RequireOwnership = false)]
        // void SpawnBossServerRpc(ServerRpcParams serverRpcParams = default)
        // {
        //     var newEnemy = Instantiate(m_BossPrefab);
        //     newEnemy.SpawnWithOwnership(NetworkManager.Singleton.LocalClientId, true);
        //     // PublishCheatUsedMessage(serverRpcParams.Receive.SenderClientId, "SpawnBoss");
        // }



        [ServerRpc(RequireOwnership = false)]
        void KillAllEnemiesServerRpc(ServerRpcParams serverRpcParams = default)
        {
            foreach (var serverCharacter in FindObjectsOfType<ServerCharacter>())
            {
                if (serverCharacter.IsNpc && serverCharacter.NetState.LifeState == LifeState.Alive)
                {
                    if (serverCharacter.gameObject.TryGetComponent(out IHurtBox damageable))
                    {
                        AttackDataSend Atk_data = new AttackDataSend();
                        Atk_data.Amount_injury = -serverCharacter.NetState.HPPoints;
   
                        damageable.ReceiveHP(Atk_data);
                    }
                }
            }
            // PublishCheatUsedMessage(serverRpcParams.Receive.SenderClientId, "KillAllEnemies");
        }

        // [ServerRpc(RequireOwnership = false)]
        // void ToggleGodModeServerRpc(ServerRpcParams serverRpcParams = default)
        // {
        //     var clientId = serverRpcParams.Receive.SenderClientId;
        //     var playerServerCharacter = PlayerServerCharacter.GetPlayerServerCharacter(clientId);
        //     if (playerServerCharacter != null)
        //     {
        //         playerServerCharacter.NetLifeState.IsGodMode.Value = !playerServerCharacter.NetLifeState.IsGodMode.Value;
        //         PublishCheatUsedMessage(clientId, "ToggleGodMode");
        //     }
        // }

        // [ServerRpc(RequireOwnership = false)]
        // void HealPlayerServerRpc(ServerRpcParams serverRpcParams = default)
        // {
        //     var clientId = serverRpcParams.Receive.SenderClientId;
        //     var playerServerCharacter = PlayerServerCharacter.GetPlayerServerCharacter(clientId);
        //     if (playerServerCharacter != null)
        //     {
        //         var baseHp = playerServerCharacter.CharacterClass.BaseHP.Value;
        //         if (playerServerCharacter.LifeState == LifeState.Fainted)
        //         {
        //             playerServerCharacter.Revive(null, baseHp);
        //         }
        //         else
        //         {
        //             if (playerServerCharacter.gameObject.TryGetComponent(out IDamageable damageable))
        //             {
        //                 damageable.ReceiveHP(null, baseHp);
        //             }
        //         }
        //         PublishCheatUsedMessage(serverRpcParams.Receive.SenderClientId, "HealPlayer");
        //     }
        // }

        [ServerRpc(RequireOwnership = false)]
        void ToggleAIBrainServerRpc(ServerRpcParams serverRpcParams = default)
        {
            Debug_ToggleAiBrain.RaiseEvent(true);
            // var clientId = serverRpcParams.Receive.SenderClientId;
            // foreach (var playerServerCharacter in NbPlayer.GetPlayer())
            // {
            //     if (playerServerCharacter.OwnerClientId == clientId)
            //     {
            //         playerServerCharacter.Movement.SpeedCheatActivated = !playerServerCharacter.Movement.SpeedCheatActivated;
            //         break;
            //     }
            // }
            // PublishCheatUsedMessage(clientId, "ToggleSuperSpeed");
        }

        [ServerRpc(RequireOwnership = false)]
        void ToggleSpawnWavesServerRpc(ServerRpcParams serverRpcParams = default)
        {
            Debug_Toggle_SpwanWave.RaiseEvent(true) ;

            // PublishCheatUsedMessage(clientId, "ToggleSuperSpeed");
        }

        // [ServerRpc(RequireOwnership = false)]
        // void ToggleTeleportModeServerRpc(ServerRpcParams serverRpcParams = default)
        // {
        //     var clientId = serverRpcParams.Receive.SenderClientId;
        //     foreach (var playerServerCharacter in PlayerServerCharacter.GetPlayerServerCharacters())
        //     {
        //         if (playerServerCharacter.OwnerClientId == clientId)
        //         {
        //             playerServerCharacter.Movement.TeleportModeActivated = !playerServerCharacter.Movement.TeleportModeActivated;
        //             break;
        //         }
        //     }
        //     PublishCheatUsedMessage(serverRpcParams.Receive.SenderClientId, "ToggleTeleportMode");
        // }

        // [ServerRpc(RequireOwnership = false)]
        // void ToggleDoorServerRpc(ServerRpcParams serverRpcParams = default)
        // {
        //     if (SwitchedDoor != null)
        //     {
        //         SwitchedDoor.ForceOpen = !SwitchedDoor.ForceOpen;
        //         PublishCheatUsedMessage(serverRpcParams.Receive.SenderClientId, "ToggleDoor");
        //     }
        //     else
        //     {
        //         UnityEngine.Debug.Log("Could not activate ToggleDoor cheat. Door not found.");
        //     }
        // }

        // [ServerRpc(RequireOwnership = false)]
        // void TogglePortalsServerRpc(ServerRpcParams serverRpcParams = default)
        // {
        //     foreach (var portal in FindObjectsOfType<EnemyPortal>())
        //     {
        //         if (m_DestroyPortalsOnNextToggle)
        //         {
        //             // This will only affect portals that are currently active in a scene and are currently loaded.
        //             // Portals that are already destroyed will not be affected by this, and won't have their cooldown
        //             // reinitialized.
        //             portal.ForceDestroy();
        //         }
        //         else
        //         {
        //             portal.ForceRestart();
        //         }
        //     }

        //     m_DestroyPortalsOnNextToggle = !m_DestroyPortalsOnNextToggle;
        //     PublishCheatUsedMessage(serverRpcParams.Receive.SenderClientId, "TogglePortals");
        // }

        [ServerRpc(RequireOwnership = false)]
        void GoToPostGameServerRpc(ServerRpcParams serverRpcParams = default)
        {
            SceneLoaderWrapper.Instance.LoadScene("PostGame", useNetworkSceneManager: true);
            // PublishCheatUsedMessage(serverRpcParams.Receive.SenderClientId, "GoToPostGame");
        }

        // void PublishCheatUsedMessage(ulong clientId, string cheatUsed)
        // {
        //     var playerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);
        //     if (playerData.HasValue)
        //     {
        //         m_CheatUsedMessagePublisher.Publish(new CheatUsedMessage(cheatUsed, playerData.Value.PlayerName));
        //     }
        // }

        static void LogCheatNotImplemented(string cheat)
        {
            UnityEngine.Debug.Log($"Cheat {cheat} not implemented");
        }

#else
        void Awake()
        {
            m_DebugCheatsPanel.SetActive(false);
        }
#endif
    }
}
