using System;
using System.Collections;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace LF2.Server
{
    /// <summary>
    /// Server specialization of Character Select game state.
    /// </summary>
    [RequireComponent(typeof(CharSelectData))]
    [RequireComponent(typeof(BackGroundSelectData))]

    public class ServerCharSelectState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.CharSelect; } }
        public CharSelectData CharSelectData { get; private set; }

        // New 
        public BackGroundSelectData BackGroundSelectData { get; private set; }

        Coroutine m_WaitToEndLobbyCoroutine;


        


        protected override void Awake()
        {
            CharSelectData = GetComponent<CharSelectData>();
            BackGroundSelectData = GetComponent<BackGroundSelectData>();

        }
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
            }
            else
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
                // Client request choose this seat or lock or unlock  
                CharSelectData.OnClientChangedSomeThing += OnClientChangedSomething;
                BackGroundSelectData.OnHostChangedBackGround += OnHostChangedBackGround;
                BackGroundSelectData.OnHostClickedReady += OnHostReady;



                NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            }
        }
        public override void OnNetworkDespawn()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
            }
            if (CharSelectData)
            {
                CharSelectData.OnClientChangedSomeThing -= OnClientChangedSomething;
            }
            if (BackGroundSelectData)
            {
                BackGroundSelectData.OnHostChangedBackGround -= OnHostChangedBackGround;
            }
        }

        // Update LobbyPlayerState data in Server SIDE 
        private void OnClientChangedSomething(ulong clientId, CharacterTypeEnum newChampId,TeamType teamType, bool lockedIn)
        {
            if (BackGroundSelectData.LobbyModeChange.Value == LobbyMode.ChooseAI){
                // Now only Host can change (select) champ for BOT 
                if ( CharSelectData.IsLobbyClosed.Value == true){ return;}



                int idxBOT = FindBotIdx();
                // Debug.Log( idxBOT + teamType);
                // Debug.Log(idxBOT);
                CharSelectData.LobbyBOTs[idxBOT] = new CharSelectData.LobbyPlayerState(clientId,
                CharSelectData.LobbyBOTs[idxBOT].PlayerName,
                CharSelectData.LobbyBOTs[idxBOT].PlayerNumber,
                lockedIn ? CharSelectData.SeatState.LockedIn : CharSelectData.SeatState.Active,
                teamType,
                newChampId
                );

                if (EveryBOTReady())   CloseLobby();
                return;
            } 
            int idx = FindLobbyPlayerIdx(clientId);
            if (idx == -1)
            {
                //TODO-FIXME:Netcode See note about Netcode for GameObjects issue 745 in WaitToSeatNowPlayer.
                //while this workaround is in place, we must simply ignore these update requests from the client.
                //throw new System.Exception($"OnClientChangedSomeThing: client ID {clientId} is not a lobby player and cannot change seats!");
                return;
            }




            // if alredy lock the champ
            if (CharSelectData.LobbyPlayers[idx].SeatState == CharSelectData.SeatState.LockedIn || CharSelectData.IsLobbyClosed.Value) return;

            // Logic normal : Update   LobbyPlayerState
            CharSelectData.LobbyPlayers[idx] = new CharSelectData.LobbyPlayerState(clientId,
                CharSelectData.LobbyPlayers[idx].PlayerName,
                CharSelectData.LobbyPlayers[idx].PlayerNumber,
                lockedIn ? CharSelectData.SeatState.LockedIn : CharSelectData.SeatState.Active,
                teamType,
                newChampId
                );


            /// Check if , we lock in the whole lobby, begin to ChooseState
            GotoBackGroundChooseState();

        }




        #region Only Host Do
            
        private void OnHostChangedBackGround(bool Nextleft){
            
            if (BackGroundSelectData.LobbyModeChange.Value == LobbyMode.LobbyEnding)
            {
                // The user tried to change BackGround  after everything was locked in... too late! Discard this choice
                return;
            }
            if (Nextleft && BackGroundSelectData.BackGroundNumber.Value == 0){
                BackGroundSelectData.BackGroundNumber.Value = BackGroundSelectData.backGroundGameRegistry.m_BackGrounds.Length - 1;   

            }else if (Nextleft && BackGroundSelectData.BackGroundNumber.Value > 0 ){
                BackGroundSelectData.BackGroundNumber.Value = BackGroundSelectData.BackGroundNumber.Value - 1;
            }
            else if (!Nextleft && BackGroundSelectData.BackGroundNumber.Value >= BackGroundSelectData.backGroundGameRegistry.m_BackGrounds.Length - 1){
                BackGroundSelectData.BackGroundNumber.Value = 0;
            } 
            else {
                BackGroundSelectData.BackGroundNumber.Value += 1 ;
            }

        }
        private void OnHostReady(int nbBot){
            // Huy here 
            if (nbBot > 0){
                // Creat Data For BOT 
                for (int i = 0 ; i <nbBot; ++i  ){
                    CharSelectData.LobbyBOTs.Add(new CharSelectData.LobbyPlayerState(NetworkManager.ServerClientId, "Com " + (i).ToString(), i, CharSelectData.SeatState.Inactive));
                }
                // for (int bot = 0; bot <nbBot; ++bot)
                // {
                //     Debug.Log(CharSelectData.LobbyBOTs[bot].PlayerNumber + "bot number" ); 
                // } 
                BackGroundSelectData.LobbyModeChange.Value = LobbyMode.ChooseAI;
                return;
            }

            // everybody's ready !
            // Save our choices so the next scene can use the info
            if (!EveryBodyReady()) return;
            CloseLobby();
        }


                // HUY Change Here : Not close yet when everybody's have chosen their champs
        /// <summary>
        /// Looks through all our connections and sees if everyone has locked in their choice;
        /// if so, we lock in the whole lobby, save state, and begin the transition to gameplay
        /// </summary>
        private void CloseLobby()
        {

            // No BOT COM so only Player vs Player 
            // Lock it !
            BackGroundSelectData.LobbyModeChange.Value = LobbyMode.LobbyEnding;
            CharSelectData.IsLobbyClosed.Value = true;
            
            SaveLobbyResults();

            // Huy here 
            // Delay a few seconds to give the UI time to react, then switch scenes
            m_WaitToEndLobbyCoroutine = StartCoroutine(WaitToEndLobby());
        }

        private bool EveryBodyReady(){

            // sanitaire check if evrybody ready
            foreach (CharSelectData.LobbyPlayerState playerInfo in CharSelectData.LobbyPlayers)
            {
                if (playerInfo.SeatState != CharSelectData.SeatState.LockedIn)
                    return false; // nope, at least one player isn't locked in yet!
            }
            return true;


        }

        private bool EveryBOTReady(){

            // sanitaire check if evrybody ready
            foreach (CharSelectData.LobbyPlayerState botInfo in CharSelectData.LobbyBOTs)
            {
                if (botInfo.SeatState != CharSelectData.SeatState.LockedIn)
                    return false; // nope, at least one player isn't locked in yet!
            }
            return true;

        }


        #endregion



        /// <summary>
        /// Returns the index of a client in the master LobbyPlayer list, or -1 if not found
        /// </summary>
        private int FindLobbyPlayerIdx(ulong clientId)
        {
            for (int i = 0; i < CharSelectData.LobbyPlayers.Count; ++i)
            {
                if (CharSelectData.LobbyPlayers[i].ClientId == clientId)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of a BOT in the master LobbyBOT list, 
        /// </summary>
        private int FindBotIdx()
        {
            for (int i = 0; i < CharSelectData.LobbyBOTs.Count; ++i)
            {

                if (CharSelectData.LobbyBOTs[i].SeatState != CharSelectData.SeatState.LockedIn)
                    return i;
                
            }
            return 0;
        }

        // HUY Add new Here : 
        /// <summary>
        /// Looks through all our connections and sees if everyone has locked in their choice;
        /// </summary>
        private void GotoBackGroundChooseState()
        {

            if (!EveryBodyReady()) return;

            // Huy here 
            // everybody's ready at the same time! Change lobby state = ChooseBackGround  !
            BackGroundSelectData.IsStateChooseBackGround.Value = true;

        }



        /// <summary>
        /// Cancels the process of closing the lobby, so that if a new player joins, they are able to chose a character.
        /// </summary>
        void CancelCloseLobby()
        {
            if (m_WaitToEndLobbyCoroutine != null)
            {
                StopCoroutine(m_WaitToEndLobbyCoroutine);
            }
            CharSelectData.IsLobbyClosed.Value = false;
        }

        private void SaveLobbyResults()
        {
            foreach (CharSelectData.LobbyPlayerState playerInfo in CharSelectData.LobbyPlayers)
            {
                // Find Object in SpawnManager ( in scene StartUp clicked to GameObject NetworkManager look to see in Inspector)
                var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerInfo.ClientId);

                if (playerNetworkObject && playerNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer))
                {
                    // pass avatar GUID to PersistentPlayer
                        // CharSelectData.AvatarByHero.TryGetValue(m_PlayerSeats[seatIdx].NameChampion,out avatar);
                        // Debug.Log("palyer Infor Champ" + playerInfo.PlayerChamp);
                        if (!CharSelectData.AvatarByHero.TryGetValue(playerInfo.PlayerChamp , out Avatar avatar)){
                            Debug.LogError("Dont Have Avatar for " + playerInfo.PlayerChamp);
                            return ; 
                        }
                        // Debug.Log("Character Type " + avatar.CharacterClass.CharacterType); 
                        persistentPlayer.NetworkAvatarGuidState.AvatarGuid.Value = avatar.Guid.ToNetworkGuid();
                        persistentPlayer.NetworkNameState.Team.Value = playerInfo.PlayerTeam;
                        // Save result Background 
                        persistentPlayer.PersistentBackGround.NetworkBackGroundGuid = BackGroundSelectData.backGroundGameRegistry.m_BackGrounds[BackGroundSelectData.BackGroundNumber.Value].Guid.ToNetworkGuid() ;
                }
            }

            var playerNetworkObjectServer = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.ServerClientId );
            if (playerNetworkObjectServer.TryGetComponent(out PersistentPlayer persistentPlayer1)){
                foreach (CharSelectData.LobbyPlayerState botInfo in CharSelectData.LobbyBOTs)
                {
                    // pass avatar GUID to PersistentPlayer
                        // CharSelectData.AvatarByHero.TryGetValue(m_PlayerSeats[seatIdx].NameChampion,out avatar);
                        if (!CharSelectData.AvatarByHero.TryGetValue(botInfo.PlayerChamp , out Avatar avatar)){
                            Debug.LogError("Dont Have BOT Avatar for " + botInfo.PlayerChamp);
                            return ; 
                        }
                        
                        persistentPlayer1.AddPersistentBotData(new PersistentBOT (
                            botInfo.PlayerName,
                            botInfo.PlayerTeam,
                            avatar.Guid.ToNetworkGuid()
                        ));
                }
            }
  
        }

        // TO DO : Save result background here 


        private IEnumerator WaitToEndLobby()
        {
            yield return new WaitForSeconds(3);
            SceneLoaderWrapper.Instance.LoadScene("LF2_Net", useNetworkSceneManager: true);
        }


        private void OnSceneEvent(SceneEvent sceneEvent)
        {
            // We need to filter out the event that are not a client has finished loading the scene
            if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
            // When the client finishes loading the Lobby Map, we will need to Seat it
            // Debug.Log("loading scene " + sceneEvent.SceneName);

            SeatNewPlayer(sceneEvent.ClientId);
            // Debug.Log(sceneEvent.ClientId);
            // Debug.Log(OwnerClientId);
        }

        int GetAvailablePlayerNumber()
        {
            for (int possiblePlayerNumber = 0; possiblePlayerNumber < CharSelectData.k_MaxLobbyPlayers; ++possiblePlayerNumber)
            {
                if (IsPlayerNumberAvailable(possiblePlayerNumber))
                {
                    return possiblePlayerNumber;
                }
            }
            // we couldn't get a Player# for this person... which means the lobby is full!
            return -1;
        }

        bool IsPlayerNumberAvailable(int playerNumber)
        {
            bool found = false;
            foreach (CharSelectData.LobbyPlayerState playerState in CharSelectData.LobbyPlayers)
            {
                if (playerState.PlayerNumber == playerNumber)
                {
                    found = true;
                    break;
                }
            }

            return !found;
        }

        void SeatNewPlayer(ulong clientId)
        {
            // Debug.Log("Run Seat New Player");
            // If lobby is closing and waiting to start the game, cancel to allow that new player to select a character
            if (CharSelectData.IsLobbyClosed.Value)
            {
                CancelCloseLobby();
            }
            

            SessionPlayerData? sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);
            if (sessionPlayerData.HasValue)
            {
                var playerData = sessionPlayerData.Value;
                if (playerData.PlayerNumber == -1 || !IsPlayerNumberAvailable(playerData.PlayerNumber))
                {
                    // If no player num already assigned or if player num is no longer available, get an available one.
                    playerData.PlayerNumber = GetAvailablePlayerNumber();
                }
                if (playerData.PlayerNumber == -1)
                {
                    // Sanity check. We ran out of seats... there was no room!
                    throw new Exception($"we shouldn't be here, connection approval should have refused this connection already for client ID {clientId} and player num {playerData.PlayerNumber}");
                }
                Debug.Log("player Name " + playerData.PlayerName);
                CharSelectData.LobbyPlayers.Add(new CharSelectData.LobbyPlayerState(clientId, playerData.PlayerName, playerData.PlayerNumber, CharSelectData.SeatState.Inactive));
                // !!! TO remark IMPORTANCE :  Here we update and save the lastest  SessionPlayerData for the use later (to use in another scene (LF2 Scene))  
                
                // Not very true : but the ideal is ok 
                SessionManager<SessionPlayerData>.Instance.SetPlayerData(clientId, playerData);
            }
        }

        void OnClientDisconnectCallback(ulong clientId)
        {
            // clear this client's PlayerNumber and any associated visuals (so other players know they're gone).
            for (int i = 0; i < CharSelectData.LobbyPlayers.Count; ++i)
            {
                if (CharSelectData.LobbyPlayers[i].ClientId == clientId)
                {
                    CharSelectData.LobbyPlayers.RemoveAt(i);
                    break;
                }
            }

            if (!CharSelectData.IsLobbyClosed.Value)
            {
                // If the lobby is not already closing, close if the remaining players are all ready
                CloseLobby();
            }
        }
    }
}
