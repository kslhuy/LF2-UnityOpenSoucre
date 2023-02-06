using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Infrastructure;
using Unity.Multiplayer.Samples.BossRoom;

using Unity.Multiplayer.Samples.Utilities;

using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.AddressableAssets;
using VContainer;
using LF2.Gameplay.GameState;
using LF2.ConnectionManagement;
using LF2.Utils;
using LF2.Client;

namespace LF2.Server
{
    /// <summary>
    /// Server specialization of core BossRoom game logic.
    /// </summary>
    public class ServerLF2State : GameStateBehaviour
    {


        [SerializeField]
        NetcodeHooks m_NetcodeHooks;


        // LF2 exculsive 
        private StageManager stageManager;

        [SerializeField]
        [Tooltip("Make sure this is included in the NetworkManager's list of prefabs!")]
        private NetworkObject m_PlayerPrefab;

        [SerializeField]
        [Tooltip("Make sure this is included in the NetworkManager's list of prefabs!")]
        private NetworkObject m_BOTPrefab;

        [Tooltip("A collection of locations for spawning players")]
        [SerializeField] Transform[] m_PlayerSpawnPoints;

        private List<Transform> m_PlayerSpawnPointsList = null;

        // private List<Transform> m_BOTSpawnPointsList = null;

        [SerializeField]
        private ClientLF2State _ClientLF2State;

        public override GameState ActiveState { get { return GameState.LF2_Net; } }


        // Wait time constants for switching to post game after the game is won or lost
        private const float k_WinDelay = 5f;
        private const float k_LoseDelay = 5f;

        /// <summary>
        /// Has the ServerBossRoomState already hit its initial spawn? (i.e. spawned players following load from character select).
        /// </summary>
        public bool InitialSpawnDone { get; private set; }

        /// <summary>
        /// Keeping the subscriber during this GameState's lifetime to allow disposing of subscription and re-subscribing
        /// when despawning and spawning again.
        /// </summary>
        // [Inject] ISubscriber<LifeStateChangedEventMessage> m_LifeStateChangedEventMessageSubscriber;
        [SerializeField] LifeStateEventChannelSO lifeStateEventChannelSO;

        [Inject] ConnectionManager m_ConnectionManager;
        [SerializeField] PersistentPlayerRuntimeCollection m_PersistentPlayerCollection;

        private int _nbPlayer;


        // [SerializeField]
        // private BackGroundGameRegistry m_BackGroundResigtry;

        // [SerializeField]
        // private Transform BackGroundSpwanPoint;

        // private Dictionary<TeamType, List<ServerCharacter>> _allPlayerByTeam = new Dictionary<TeamType, List<ServerCharacter>>();
        // List<ServerCharacter> _allPlayer;
        private Coroutine Coro_GameEnd;

        protected override void Awake()
        {
            base.Awake();
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;
        }

        public void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
            }
            // m_PersistentGameState.Reset();
            lifeStateEventChannelSO.LifeStateEvent_AI += OnLifeStateChangedEventMessage_NPC;
            lifeStateEventChannelSO.LifeStateEvent_Player += OnLifeStateChangedEventMessage_Player;

            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
            NetworkManager.Singleton.SceneManager.OnSynchronizeComplete += OnSynchronizeComplete;

            SessionManager<SessionPlayerData>.Instance.OnSessionStarted();


        }

        void OnNetworkDespawn()
        {
            // m_PersistentPlayerCollection.RemoveALL();
            lifeStateEventChannelSO.LifeStateEvent_AI -= OnLifeStateChangedEventMessage_NPC;
            lifeStateEventChannelSO.LifeStateEvent_Player += OnLifeStateChangedEventMessage_Player;

            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
            NetworkManager.Singleton.SceneManager.OnSynchronizeComplete -= OnSynchronizeComplete;

        }

        protected override void OnDestroy()
        {
            lifeStateEventChannelSO.LifeStateEvent_AI -= OnLifeStateChangedEventMessage_NPC;
            lifeStateEventChannelSO.LifeStateEvent_Player += OnLifeStateChangedEventMessage_Player;

            if (m_NetcodeHooks)
            {
                m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
                m_NetcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            }
            base.OnDestroy();
        }

        void OnSynchronizeComplete(ulong clientId)
        {
            if (InitialSpawnDone && !PlayerServerCharacter.GetPlayerServerCharacter(clientId))
            {
                //somebody joined after the initial spawn. This is a Late Join scenario. This player may have issues
                //(either because multiple people are late-joining at once, or because some dynamic entities are
                //getting spawned while joining. But that's not something we can fully address by changes in
                //ServerBossRoomState.
                SpawnPlayer(clientId, true);
            }
        }


        void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            if (!InitialSpawnDone && loadSceneMode == LoadSceneMode.Single)
            {
                InitialSpawnDone = true;
                foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
                {
                    SpawnPlayer(kvp.Key, false);
                }
                SpawnBOTandBackGround(NetworkManager.ServerClientId, true);
            }
        }

        void OnClientDisconnect(ulong clientId)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                // If a client disconnects, check for game over in case all other players are already down
                // NEW 
                // if (m_NetworkGameState.NetworkGameMode.gameMode.Value == GameMode.VS){
                //     StartCoroutine(WaitToCheckForGameOver());
                // }
            }
        }





        private void SpawnPlayer(ulong clientId, bool lateJoin)
        {
            Debug.Log("local Client Id : " + clientId );
            Transform spawnPoint = null;

            if (m_PlayerSpawnPointsList == null || m_PlayerSpawnPointsList.Count == 0)
            {
                m_PlayerSpawnPointsList = new List<Transform>(m_PlayerSpawnPoints);
            }

            Debug.Assert(m_PlayerSpawnPointsList.Count > 0,
                $"PlayerSpawnPoints array should have at least 1 spawn points.");

            int index = Random.Range(0, m_PlayerSpawnPointsList.Count);
            spawnPoint = m_PlayerSpawnPointsList[index];
            m_PlayerSpawnPointsList.RemoveAt(index);


            var newPlayer = Instantiate(m_PlayerPrefab, Vector3.zero, Quaternion.identity);

            var newPlayerCharacter = newPlayer.GetComponent<ServerCharacter>();
            // _allPlayer.Add(newPlayerCharacter);

            var physicsTransform = newPlayerCharacter.physicsWrapper.Transform;

            if (spawnPoint != null)
            {
                physicsTransform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            }

            var persistentPlayerExists = m_PersistentPlayerCollection.TryGetPlayer(clientId , out PersistentPlayer persistentPlayer);
            // var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
            // var persistentPlayerExists = playerNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer);
            Assert.IsTrue(persistentPlayerExists,
                $"Matching persistent PersistentPlayer for client {clientId} not found!");

            // Debug.Log(persistentPlayer.name);
            var networkAvatarGuidStateExists =
                newPlayer.TryGetComponent(out NetworkAvatarGuidState networkAvatarGuidState);

            Assert.IsTrue(networkAvatarGuidStateExists,
                $"NetworkCharacterGuidState not found on player avatar!");

            // if reconnecting, set the player's position and rotation to its previous state
            if (lateJoin)
            {
                SessionPlayerData? sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);
                if (sessionPlayerData is { HasCharacterSpawned: true })
                {
                    physicsTransform.SetPositionAndRotation(sessionPlayerData.Value.PlayerPosition, sessionPlayerData.Value.PlayerRotation);
                }
            }
            // pass character type from persistent player to avatar

            networkAvatarGuidState.AvatarGuid.Value =
                persistentPlayer.NetworkAvatarGuidState.AvatarGuid.Value;

            // pass name from persistent player to avatar
            if (newPlayer.TryGetComponent(out NetworkNameState networkNameState))
            {
                // Debug.Log("Network Name " + persistentPlayer.NetworkNameState.Name.Value);
                networkNameState.Name.Value = persistentPlayer.NetworkNameState.Name.Value;
                // Debug.Log("Network Team " + persistentPlayer.NetworkNameState.Team.Value);
                networkNameState.Team.Value = persistentPlayer.NetworkNameState.Team.Value;

                // List<ServerCharacter> listPlayerByTeam;
                // _allPlayerByTeam[networkNameState.Team.Value].Add(newPlayerCharacter);
                // if (!_allPlayerByTeam.TryGetValue(persistentPlayer.NetworkNameState.Team.Value, out listPlayerByTeam))
                // {
                //     listPlayerByTeam = new List<ServerCharacter>();

                // }
                // // At this point we know that "existing" refers to the relevant list in the 
                // // dictionary, one way or another.
                // _allPlayerByTeam[persistentPlayer.NetworkNameState.Team.Value].Add(newPlayerCharacter) ;

            }

            // spawn players characters with destroyWithScene = true
            newPlayer.SpawnWithOwnership(clientId, true);


        }

        public void SpawnBOTandBackGround(ulong serverId, bool spawnBackGround){
            Transform spawnPoint = null;

            if (m_PlayerSpawnPointsList == null || m_PlayerSpawnPointsList.Count == 0)
            {
                m_PlayerSpawnPointsList = new List<Transform>(m_PlayerSpawnPoints);
            }

            Debug.Assert(m_PlayerSpawnPointsList.Count > 0,
                $"PlayerSpawnPoints array should have at least 1 spawn points.");




            // var botNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var persistentPlayerExists = m_PersistentPlayerCollection.TryGetPlayer(serverId ,out PersistentPlayer persistentPlayer);
            // var persistentPlayerExists = botNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer);
            Assert.IsTrue(persistentPlayerExists,
                $"Matching persistent PersistentPlayer for Bot not found!");


            for (int b = 0; b < persistentPlayer.HowManyBOTData(); b++){
                // Find a spawn point 
                int index = Random.Range(0, m_PlayerSpawnPointsList.Count);
                spawnPoint = m_PlayerSpawnPointsList[index];
                // Instaite a Bot Object
                var newBOT = Instantiate(m_BOTPrefab, Vector3.zero, Quaternion.identity);
                var newBOTCharacter = newBOT.GetComponent<ServerCharacter>();

                var physicsTransform = newBOTCharacter.physicsWrapper.Transform;
                if (spawnPoint != null)
                {
                    // Set position and rotation to the Bot Object in the scene
                    physicsTransform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
                }

                // Check if the Bot Object have Componenet NetworkAvatarGuidState  
                var networkAvatarGuidStateExists =
                    newBOT.TryGetComponent(out NetworkAvatarGuidState networkAvatarGuidState);

                Assert.IsTrue(networkAvatarGuidStateExists,
                    $"NetworkCharacterGuidState not found on player avatar!");

                // assign the Avatar value to the component NetworkAvatarGuidState from Selection Scene 
                networkAvatarGuidState.AvatarGuid.Value =
                    persistentPlayer.PersistentBOT.Items[b].NetworkAvatarGuid;

                // pass name , team type from persistent player to avatar
                if (newBOT.TryGetComponent(out NetworkNameState networkNameState))
                {
                    networkNameState.Name.Value = persistentPlayer.PersistentBOT.Items[b].Name;
                    // Debug.Log("Network Team " + persistentPlayer.NetworkNameState.Team.Value);
                    networkNameState.Team.Value = persistentPlayer.PersistentBOT.Items[b].TeamType;


                }

                // spawn players characters with destroyWithScene = true
                newBOT.Spawn(true);
            }

            // Reset Bot Collection (Avoid case bot spwan even not select)
            persistentPlayer.RemoveBotCollection();

            if (spawnBackGround)
            {
                _ClientLF2State.SpawnBackGroundServerRPC(persistentPlayer.PersistentBackGround.NetworkBackGround.Value);
            }
        
        }

        
        // IEnumerator WaitToCheckForGameOver()
        // {
        //     // Wait until next frame so that the client's player character has despawned
        //     yield return null;
        //     CheckForGameOverVsMode();
        // }


        static IEnumerator WaitToReposition(Transform moveTransform, Vector3 newPosition, Quaternion newRotation)
        {
            yield return new WaitForSeconds(1.5f);
            moveTransform.SetPositionAndRotation(newPosition, newRotation);
        }

        // Every time a player's life state changes we check to see if game is over
        void OnLifeStateChangedEventMessage_NPC(LifeState lifeState)
        {

            // Logic below for versus mode only
            CheckGameOverVsMode(lifeState);
            // CheckGameOverStageMode(true);
            
        }
        void OnLifeStateChangedEventMessage_Player(LifeState lifeState , ulong playerID)
        {
            
            // Logic below for versus mode only
            CheckGameOverVsMode(lifeState);
            // CheckGameOverStageMode(false);
            
            
        }



        void CheckGameOverVsMode(LifeState lifeState , ulong playerID = default)
        {
            // Logic end game 

            // Player VS Com or Player
            //  Logic if dont have any COM so we win.
            // Or if one team left in the game , so the game end

            // If we already Call End Game so return (Just sainitaire checked)
            if (Coro_GameEnd != null ) return;


            List<TeamType> listTeamNow = new List<TeamType>();
            int i = 0;
            // Check the life state of all players in the scene
            foreach (var serverCharacter in PlayerServerCharacter.GetPlayerServerCharacters())
            {
                // Because a character will destroyed when died , 
                // so they dont have Component NetStat , that reasone why we check NetState first
                if (serverCharacter.NetState && serverCharacter.NetState.LifeState == LifeState.Alive)
                {
                    i++;
                    if (listTeamNow.Contains(serverCharacter.NetState.TryGetTeamType())) return;
                    listTeamNow.Add(serverCharacter.NetState.TryGetTeamType());
                    if (listTeamNow.Count > 1) return;   // more than 1 mean 2 , 3 team left during the game 
                                                        // so that mean can not End the game 


                }
            }


            // 1: Find Team Left 
            TeamType teamType_left = listTeamNow[0];

            // 2: Set Win State For Each player
            // _ClientLF2State.EndGameServerRPC(teamType_left);
            foreach (ClientCharacterVisualization player in NbPlayer.GetPlayers()){
                PersistentPlayer persistentPlayer;
                Debug.Log("player Owner ID server" + player.OwnerClientId);
                if (player.m_NetState.TryGetTeamType() != teamType_left){
                    
                    m_PersistentPlayerCollection.TryGetPlayer(player.OwnerClientId , out persistentPlayer);
                    persistentPlayer.SetWinState(WinState.Loss);
                    // continue;
                }
                else if (teamType_left == TeamType.INDEPENDANT && (player.m_NetState.LifeState == LifeState.Dead)){
                    m_PersistentPlayerCollection.TryGetPlayer(player.OwnerClientId , out persistentPlayer);
                    persistentPlayer.SetWinState(WinState.Loss);

                }else{
                    // 3: Find All Players In this Team  
                    m_PersistentPlayerCollection.TryGetPlayer(player.OwnerClientId , out persistentPlayer);
                    persistentPlayer.SetWinState(WinState.Win);

                }
            }

            



            // If we made it this far, all players are down! switch to post game
            
            Coro_GameEnd = StartCoroutine(CoroGameOver(k_LoseDelay, true));
        }

        void CheckGameOverStageMode(bool checkAI){
            if (checkAI){

            }
        }

        // private void SaveSummaryResults()
        // {
        //     foreach (CharSelectData.LobbyPlayerState playerInfo in CharSelectData.LobbyPlayers)
        //     {
        //         // Find Object in SpawnManager ( in scene StartUp clicked to GameObject NetworkManager look to see in Inspector)
        //         var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerInfo.ClientId);

        //         if (playerNetworkObject && playerNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer))
        //         {
        //             // pass avatar GUID to PersistentPlayer
        //                 // CharSelectData.AvatarByHero.TryGetValue(m_PlayerSeats[seatIdx].NameChampion,out avatar);
        //                 // Debug.Log("palyer Infor Champ" + playerInfo.PlayerChamp);
        //                 if (!CharSelectData.AvatarByHero.TryGetValue(playerInfo.PlayerChamp , out Avatar avatar)){
        //                     Debug.LogError("Dont Have Avatar for " + playerInfo.PlayerChamp);
        //                     return ; 
        //                 }
        //                 // Debug.Log("Character Type " + avatar.CharacterClass.CharacterType); 
        //                 persistentPlayer.NetworkAvatarGuidState.AvatarGuid.Value = avatar.Guid.ToNetworkGuid();
        //                 persistentPlayer.NetworkNameState.Team.Value = playerInfo.PlayerTeam;
        //                 // Save result Background 
        //                 persistentPlayer.PersistentBackGround.NetworkBackGroundGuid = BackGroundSelectData.backGroundGameRegistry.m_BackGrounds[BackGroundSelectData.BackGroundNumber.Value].Guid.ToNetworkGuid() ;
        //         }
        //     }
        // }


        // // Call When A Stage Finish
        // void OnStageEndEventMessage(){
        //     // CheckForGameOverStageMode();
        //     if (Coro_GameEnd != null ) return;


        //     // Check Again to ensure the game correctly Finish
        //     List<TeamType> listTeamNow = new List<TeamType>();
        //     int i = 0;
        //     // Check the life state of all players in the scene
        //     foreach (var serverCharacter in PlayerServerCharacter.GetPlayerServerCharacters())
        //     {
        //         // if any player is alive just return
        //         if (serverCharacter.NetState && serverCharacter.NetState.LifeState == LifeState.Alive)
        //         {
        //             i++;
        //             if (listTeamNow.Contains(serverCharacter.NetState.TryGetTeamType())) return;
        //             listTeamNow.Add(serverCharacter.NetState.TryGetTeamType());
        //             if (listTeamNow.Count > 1) return;   // more than 1 mean 2 , 3 team left so can not End 


        //         }
        //     }

        //     Coro_GameEnd = StartCoroutine(CoroGameOver(k_LoseDelay, true));

        // }






        IEnumerator CoroGameOver(float wait, bool gameWon)
        {
            // wait 5 seconds for game animations to finish
            // _ClientLF2State.EndGameServerRPC();
            
            // m_PersistentGameState.SetWinState(gameWon ? WinState.Win : WinState.Loss);


            yield return new WaitForSeconds(wait);


            SceneLoaderWrapper.Instance.LoadScene("PostGame", useNetworkSceneManager: true);
        }

    }

}
