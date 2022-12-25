using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.BossRoom.Shared.Infrastructure;
using Unity.Multiplayer.Samples.BossRoom;

using Unity.Multiplayer.Samples.Utilities;

using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.AddressableAssets;


namespace LF2.Server
{
    /// <summary>
    /// Server specialization of core BossRoom game logic.
    /// </summary>
    public class ServerLF2State : GameStateBehaviour
    {
        [SerializeField]
        TransformVariable m_NetworkGameStateTransform;

        private NetworkGameState m_NetworkGameState;

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
        private NetLF2State _NetLF2State;

        public override GameState ActiveState { get { return GameState.LF2_Net; } }

        private GameNetPortal m_NetPortal;
        private ServerGameNetPortal m_ServerNetPortal;

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
        ISubscriber<LifeStateChangedEventMessage> m_LifeStateChangedEventMessageSubscriber;

        IDisposable m_Subscription;


        [SerializeField]
        private BackGroundGameRegistry m_BackGroundResigtry;

        [SerializeField]
        private Transform BackGroundSpwanPoint;
        // private List<ServerCharacter> _ListTeamInTheGame;

        private Dictionary<TeamType, List<ServerCharacter>> _allPlayerByTeam = new Dictionary<TeamType, List<ServerCharacter>>();

        private Coroutine Coro_GameEnd;

        [Inject]
        void InjectDependencies(ISubscriber<LifeStateChangedEventMessage> subscriber)
        {
            m_LifeStateChangedEventMessageSubscriber = subscriber;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
            }
            else
            {
                // reset win state
                // SetWinState(WinState.Invalid);

                m_NetPortal = GameObject.FindGameObjectWithTag("GameNetPortal").GetComponent<GameNetPortal>();
                m_ServerNetPortal = m_NetPortal.GetComponent<ServerGameNetPortal>();

                NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;
                NetworkManager.SceneManager.OnSceneEvent += OnClientSceneChanged;

                DoInitialSpawnIfPossible();

                SessionManager<SessionPlayerData>.Instance.OnSessionStarted();
                m_Subscription = m_LifeStateChangedEventMessageSubscriber.Subscribe(OnLifeStateChangedEventMessage);

                if (m_NetworkGameStateTransform && m_NetworkGameStateTransform.Value)
                {
                    m_NetworkGameState = m_NetworkGameStateTransform.Value.GetComponent<NetworkGameState>();
                }
            }

        }

        public override void OnDestroy()
        {
            m_Subscription?.Dispose();
        }

        private bool DoInitialSpawnIfPossible()
        {
            if (m_ServerNetPortal.AreAllClientsInServerScene() && !InitialSpawnDone)
            {
                InitialSpawnDone = true;
                foreach (var kvp in NetworkManager.ConnectedClients)
                {
                    SpawnPlayer(kvp.Key, false);
                }
                SpawnBOTandBackGround(NetworkManager.ServerClientId, true);
                return true;
            }
            return false;
        }

        void OnClientDisconnect(ulong clientId)
        {
            if (clientId != NetworkManager.LocalClientId)
            {
                // If a client disconnects, check for game over in case all other players are already down
                if (m_NetworkGameState.NetworkGameMode.gameMode.Value == GameMode.VS){
                    StartCoroutine(WaitToCheckForGameOver());
                }
            }
        }

        IEnumerator WaitToCheckForGameOver()
        {
            // Wait until next frame so that the client's player character has despawned
            yield return null;
            CheckForGameOverVsMode();
        }



        public void OnClientSceneChanged(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;

            var clientId = sceneEvent.ClientId;
            var sceneIndex = SceneManager.GetSceneByName(sceneEvent.SceneName).buildIndex;
            int serverScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            if (sceneIndex == serverScene)
            {
                Debug.Log($"client={clientId} now in scene {sceneIndex}, server_scene={serverScene}, all players in server scene={m_ServerNetPortal.AreAllClientsInServerScene()}");

                bool didSpawn = DoInitialSpawnIfPossible();

                if (!didSpawn && InitialSpawnDone &&
                    !PlayerServerCharacter.GetPlayerServerCharacters().Find(
                        player => player.OwnerClientId == clientId))
                {
                    //somebody joined after the initial spawn. This is a Late Join scenario. This player may have issues
                    //(either because multiple people are late-joining at once, or because some dynamic entities are
                    //getting spawned while joining. But that's not something we can fully address by changes in
                    //ServerBossRoomState.
                    SpawnPlayer(clientId, true);
                }

            }
        }

        public override void OnNetworkDespawn()
        {
            if (m_NetPortal != null)
            {
                NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;
                NetworkManager.SceneManager.OnSceneEvent -= OnClientSceneChanged;
            }
            if (stageManager != null){
                stageManager.StageFinishEvent -= OnStageEndEventMessage;
            }

            m_Subscription?.Dispose();

        }



        private void SpawnPlayer(ulong clientId, bool lateJoin)
        {
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

            var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);

            var newPlayer = Instantiate(m_PlayerPrefab, Vector3.zero, Quaternion.identity);

            var newPlayerCharacter = newPlayer.GetComponent<ServerCharacter>();

            var physicsTransform = newPlayerCharacter.physicsWrapper.Transform;

            if (spawnPoint != null)
            {
                physicsTransform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            }

            var persistentPlayerExists = playerNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer);
            Assert.IsTrue(persistentPlayerExists,
                $"Matching persistent PersistentPlayer for client {clientId} not found!");

            // pass character type from persistent player to avatar
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

            networkAvatarGuidState.AvatarGuid.Value =
                persistentPlayer.NetworkAvatarGuidState.AvatarGuid.Value;

            // pass name from persistent player to avatar
            if (newPlayer.TryGetComponent(out NetworkNameState networkNameState))
            {
                // Debug.Log("Network Name " + persistentPlayer.NetworkNameState.Name.Value);
                networkNameState.Name.Value = persistentPlayer.NetworkNameState.Name.Value;
                // Debug.Log("Network Team " + persistentPlayer.NetworkNameState.Team.Value);
                networkNameState.Team.Value = persistentPlayer.NetworkNameState.Team.Value;

                List<ServerCharacter> existing;
                if (!_allPlayerByTeam.TryGetValue(networkNameState.Team.Value, out existing))
                {
                    existing = new List<ServerCharacter>();
                    _allPlayerByTeam[networkNameState.Team.Value] = existing;
                }
                // At this point we know that "existing" refers to the relevant list in the 
                // dictionary, one way or another.
                existing.Add(newPlayerCharacter);

            }

            // spawn players characters with destroyWithScene = true
            newPlayer.SpawnWithOwnership(clientId, true);


        }

        public void SpawnBOTandBackGround(ulong clientId, bool spawnBackGround)
        {
            Transform spawnPoint = null;

            if (m_PlayerSpawnPointsList == null || m_PlayerSpawnPointsList.Count == 0)
            {
                m_PlayerSpawnPointsList = new List<Transform>(m_PlayerSpawnPoints);
            }

            Debug.Assert(m_PlayerSpawnPointsList.Count > 0,
                $"PlayerSpawnPoints array should have at least 1 spawn points.");




            var botNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

            var persistentPlayerExists = botNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer);
            Assert.IsTrue(persistentPlayerExists,
                $"Matching persistent PersistentPlayer for Bot not found!");


            for (int b = 0; b < persistentPlayer.HowManyBOTData(); b++)
            {
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

                    List<ServerCharacter> existing;
                    if (!_allPlayerByTeam.TryGetValue(networkNameState.Team.Value, out existing))
                    {
                        existing = new List<ServerCharacter>();
                        _allPlayerByTeam[networkNameState.Team.Value] = existing;
                    }
                    // At this point we know that "existing" refers to the relevant list in the 
                    // dictionary, one way or another.
                    existing.Add(newBOTCharacter);

                    // if (!_allPlayerByTeam.ContainsKey(networkNameState.Team.Value)) {
                    //     var newList = new List<ServerCharacter>();
                    //     newList.Add(newBOTCharacter); 
                    //     _allPlayerByTeam.Add(networkNameState.Team.Value , new List<ServerCharacter>());
                    // }else{
                    //     var exitingList = _allPlayerByTeam[networkNameState.Team.Value];
                    //     exitingList.Add(newBOTCharacter);
                    // }

                }

                // spawn players characters with destroyWithScene = true
                newBOT.Spawn(true);
            }

            if (spawnBackGround)
            {
                _NetLF2State.BackGroundGUID.Value = persistentPlayer.PersistentBackGround.NetworkBackGroundGuid; 
                m_BackGroundResigtry.TryGetBackGround(persistentPlayer.PersistentBackGround.NetworkBackGroundGuid.ToGuid(), out BackGroundGame backGroundGame);

                backGroundGame.BackGroundPreFab.InstantiateAsync(BackGroundSpwanPoint).Completed += (handle) =>
                {
                    var backgroundOBject = handle.Result;
                    Debug.Log(backgroundOBject);
                    backgroundOBject.GetComponent<NetworkObject>().Spawn(true);
                    if (m_NetworkGameState.NetworkGameMode.gameMode.Value == GameMode.Stage){
                        stageManager = backgroundOBject.GetComponent<StageManager>();
                        stageManager.StageFinishEvent += OnStageEndEventMessage;
                    }
                };
            }

        }


        static IEnumerator WaitToReposition(Transform moveTransform, Vector3 newPosition, Quaternion newRotation)
        {
            yield return new WaitForSeconds(1.5f);
            moveTransform.SetPositionAndRotation(newPosition, newRotation);
        }

        // Every time a player's life state changes we check to see if game is over
        void OnLifeStateChangedEventMessage(LifeStateChangedEventMessage message)
        {



            // Logic below for versus mode only
            if (message.NewLifeState == LifeState.Dead)
            {
                CheckForGameOverVsMode();
            }


        }



        void CheckForGameOverVsMode()
        {
            // Logic end game 

            //  Logic if dont have any COM so we win.

            // If we already Call End Game so return 
            if (Coro_GameEnd != null ) return;
            List<TeamType> listTeamNow = new List<TeamType>();
            int i = 0;
            // Check the life state of all players in the scene
            foreach (var serverCharacter in PlayerServerCharacter.GetPlayerServerCharacters())
            {
                // if any player is alive just return
                if (serverCharacter.NetState && serverCharacter.NetState.LifeState == LifeState.Alive)
                {
                    i++;
                    if (listTeamNow.Contains(serverCharacter.NetState.TryGetTeamType())) return;
                    listTeamNow.Add(serverCharacter.NetState.TryGetTeamType());
                    if (listTeamNow.Count > 1) return;   // more than 1 mean 2 , 3 team left so can not End 


                }
            }



            // If we made it this far, all players are down! switch to post game

            Coro_GameEnd = StartCoroutine(CoroGameOver(k_LoseDelay, true));
        }


        // Call When A Stage Finish
        void OnStageEndEventMessage()
        {
            // CheckForGameOverStageMode();
            if (Coro_GameEnd != null ) return;


            // Check Again to ensure the game correctly Finish
            List<TeamType> listTeamNow = new List<TeamType>();
            int i = 0;
            // Check the life state of all players in the scene
            foreach (var serverCharacter in PlayerServerCharacter.GetPlayerServerCharacters())
            {
                // if any player is alive just return
                if (serverCharacter.NetState && serverCharacter.NetState.LifeState == LifeState.Alive)
                {
                    i++;
                    if (listTeamNow.Contains(serverCharacter.NetState.TryGetTeamType())) return;
                    listTeamNow.Add(serverCharacter.NetState.TryGetTeamType());
                    if (listTeamNow.Count > 1) return;   // more than 1 mean 2 , 3 team left so can not End 


                }
            }

            Coro_GameEnd = StartCoroutine(CoroGameOver(k_LoseDelay, true));

        }



        void SetWinState(WinState winState)
        {
            if (m_NetworkGameStateTransform && m_NetworkGameStateTransform.Value &&
                m_NetworkGameStateTransform.Value.TryGetComponent(out NetworkGameState networkGameState))
            {
                // networkGameState.NetworkWinState = winState;
            }
        }


        private IEnumerator CoroGameOver(float wait, bool gameWon)
        {
            // wait 5 seconds for game animations to finish
            _NetLF2State.EndGameServerRPC();

            yield return new WaitForSeconds(wait);

            SetWinState(gameWon ? WinState.Win : WinState.Loss);

            SceneLoaderWrapper.Instance.LoadScene("PostGame", useNetworkSceneManager: true);
        }

    }
}
