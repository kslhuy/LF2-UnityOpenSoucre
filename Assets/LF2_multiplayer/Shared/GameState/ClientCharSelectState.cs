using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace LF2.Client
{
    /// <summary>
    /// Client specialization of the Character Select game state. Mainly controls the UI during character-select.
    /// </summary>
    [RequireComponent(typeof(CharSelectData))]
    public partial class ClientCharSelectState : GameStateBehaviour
    {
        /// <summary>
        /// Reference to the scene's state object so that UI can access state
        /// </summary>
        public static ClientCharSelectState Instance { get; private set; }

        public override GameState ActiveState { get { return GameState.CharSelect; } }
        public CharSelectData CharSelectData { get; private set; }

        public BackGroundSelectData BackGroundSelectData { get; private set; }

        // [Header("----- Lobby Seats ------")]
        // [SerializeField]
        // private List<UICharSelectPlayerSeat> m_PlayerSeats;

        public Action<bool > LockUIPlayerSeats;

        [System.Serializable]
        public class ColorAndIndicator
        {
            public Sprite Indicator;
            public Color Color;
        }
        [Tooltip("Representational information for each player")]
        public ColorAndIndicator[] m_IdentifiersForEachPlayerNumber;

        [SerializeField]
        [Tooltip("Text element containing player count which updates as players connect")]
        private TextMeshProUGUI m_NumPlayersText;

        [SerializeField]
        [Tooltip("Text element for the Ready button")]
        private TextMeshProUGUI m_ReadyButtonText;


        #region UI Elements for different lobby modes
    
        [Header("--- UI Elements for different lobby modes --- ")]
        [SerializeField]
        [Tooltip("UI elements to turn on when the player hasn't chosen their seat yet. Turned off otherwise!")]
        private List<GameObject> m_UIElementsForNoSeatChosen;

        [SerializeField]
        [Tooltip("UI elements to turn on when the player has locked in their seat choice (and is now waiting for other players to do the same). Turned off otherwise!")]
        private List<GameObject> m_UIElementsForSeatChosen;

        [SerializeField]
        [Tooltip("UI elements to turn on when the player has locked in their seat choice (and is now waiting for other players to do the same). Turned off otherwise!")]
        private List<GameObject> m_UIElementsForBackGroundChosen;

        [SerializeField]
        [Tooltip("UI elements to turn on when the lobby is closed (and game is about to start). Turned off otherwise!")]
        private List<GameObject> m_UIElementsForLobbyEnding;

        [SerializeField]
        [Tooltip("UI elements to turn on when there's been a fatal error (and the client cannot proceed). Turned off otherwise!")]
        private List<GameObject> m_UIElementsForFatalError;
#endregion

        [Header("--- Misc --- ")]
        [SerializeField]
        [Tooltip("The controller for the class-info box")]
        private List<UICharSelectClassInfoBox> m_UIAvatarInfoBox;



        [SerializeField]
        [Tooltip("The controller for the Background popup")]
        private UIBackGroundSelectBox m_BackGroundBox;



        private CharacterTypeEnum m_LastChampSelected = 0;
        private TeamType m_LastTeamSelected = 0;

        private int m_LastIndexBOTUpdateUI ;
        private CharacterTypeEnum m_LastTeamBOTSelected = 0;


        private bool m_HasLocalPlayerLockedIn = false;
        private bool m_HasLocalBOTLockedIn = false;


        GameObject m_CurrentCharacterGraphics;

        Animator m_CurrentCharacterGraphicsAnimator;

        Dictionary<Guid, GameObject> m_SpawnedCharacterGraphics = new Dictionary<Guid, GameObject>();

        private Dictionary<LobbyMode, List<GameObject>> m_LobbyUIElementsByMode;
        private Avatar avatar;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
            CharSelectData = GetComponent<CharSelectData>();
            BackGroundSelectData = GetComponent<BackGroundSelectData>();

            m_LobbyUIElementsByMode = new Dictionary<LobbyMode, List<GameObject>>()
            {
                { LobbyMode.ChooseChamp, m_UIElementsForNoSeatChosen },
                { LobbyMode.ChampChosen, m_UIElementsForSeatChosen },
                { LobbyMode.ChooseBackGround, m_UIElementsForNoSeatChosen },
                { LobbyMode.ChooseAI, m_UIElementsForNoSeatChosen },
                { LobbyMode.LobbyEnding, m_UIElementsForLobbyEnding },
                { LobbyMode.FatalError, m_UIElementsForFatalError },
            };
        }

        public override void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            base.OnDestroy();
        }


        protected override void Start()
        {
            base.Start();

            for (int i = 0; i < CharSelectData.LobbyPlayers.Count; ++i)
            {
                //  Avatar UI + ( name , team of the player ) (visual)
                int localPlayerIdx = -1;
                if (CharSelectData.LobbyPlayers[i].ClientId == NetworkManager.Singleton.LocalClientId)
                {
                    localPlayerIdx = i;
                    m_UIAvatarInfoBox[localPlayerIdx].Initialize(CharSelectData.LobbyPlayers[localPlayerIdx].PlayerName , localPlayerIdx );
                    
                }
            }

            ConfigureUIForLobbyMode(LobbyMode.ChooseChamp);
            UpdateCharacterSelection(CharSelectData.SeatState.Inactive);
        }
        public override void OnNetworkSpawn()

        {
            if (!IsClient)
            {
                enabled = false;
            }
            else
            {
                // CharSelectData.IsLobbyClosed.OnValueChanged += OnLobbyClosedChanged;
                CharSelectData.LobbyPlayers.OnListChanged += OnLobbyPlayerStateChanged;
                CharSelectData.LobbyBOTs.OnListChanged += OnLobbyBOTStateChanged;

                
                BackGroundSelectData.BackGroundNumber.OnValueChanged += OnHostChangedBackground;
                BackGroundSelectData.IsStateChooseBackGround.OnValueChanged += StateChoseBackGround;
                BackGroundSelectData.LobbyModeChange.OnValueChanged += OnLobbyModeChange;

                BackGroundSelectData.NumberBot.OnValueChanged += OnHostChangeNumberBOT; 
            }
        }



        public override void OnNetworkDespawn()
        {
            if (CharSelectData)
            {
                CharSelectData.LobbyPlayers.OnListChanged -= OnLobbyPlayerStateChanged;
                CharSelectData.LobbyBOTs.OnListChanged -= OnLobbyBOTStateChanged;

            }
            if(BackGroundSelectData){
                BackGroundSelectData.BackGroundNumber.OnValueChanged -= OnHostChangedBackground;
                BackGroundSelectData.LobbyModeChange.OnValueChanged -= OnLobbyModeChange;
                BackGroundSelectData.NumberBot.OnValueChanged -= OnHostChangeNumberBOT; 


            }

            if (Instance == this)
            {
                Instance = null;
            }
        }


        /// <summary>
        /// Called when our PlayerNumber (e.g. P1, P2, etc.) has been assigned by the server
        /// </summary>
        /// <param name="playerNum"></param>
        private void OnAssignedPlayerNumber(int playerNum)
        {
            // m_UIAvatarInfoBox.OnSetPlayerNumber(playerNum);
        }

        private void OnAssignedBOTNumber(int botNum)
        {
            // m_UIAvatarInfoBox.OnSetPlayerNumber(playerNum);
        }

        // Update bao nhieu player trong lobby
        private void UpdatePlayerCount(int numberPlayer)
        {
            int count = CharSelectData.LobbyPlayers.Count;
            var pstr = (count > 1) ? "players" : "player";
            m_NumPlayersText.text = "<b>" + count + "</b> " + pstr +" connected";

            m_UIAvatarInfoBox[numberPlayer].Initialize(CharSelectData.LobbyPlayers[numberPlayer].PlayerName , numberPlayer );


        }
        private void OnLobbyBOTStateChanged(NetworkListEvent<CharSelectData.LobbyPlayerState> changeEvent){
            int currentIdxBOT = -1;

            for (int i = 0; i < CharSelectData.LobbyBOTs.Count; ++i)
            {
                if (CharSelectData.LobbyBOTs[i].SeatState == CharSelectData.SeatState.LockedIn && m_LastIndexBOTUpdateUI <= i ){
                    // Last index is locked , but not update UI 
                    // So do it
                    UpdateBOTSelection(CharSelectData.LobbyBOTs[i].SeatState, currentIdxBOT,CharSelectData.LobbyBOTs[i].PlayerChamp,CharSelectData.LobbyBOTs[i].PlayerTeam);
                    m_LastIndexBOTUpdateUI += 1;
                }
                else 
                {
                    currentIdxBOT = i;
                    break;
                }
            }


            // case ERROR (dont care much)
            if (currentIdxBOT == -1) return;
            
            // we have a seat! Note that if our seat is LockedIn, this function will also switch the lobby mode
            UpdateBOTSelection(CharSelectData.LobbyBOTs[currentIdxBOT].SeatState, currentIdxBOT,CharSelectData.LobbyBOTs[currentIdxBOT].PlayerChamp,CharSelectData.LobbyBOTs[currentIdxBOT].PlayerTeam);
            

        }

        

        /// <summary>
        /// Called by the server when any of the seats in the lobby have changed. (Including ours!)
        /// </summary>
        private void OnLobbyPlayerStateChanged(NetworkListEvent<CharSelectData.LobbyPlayerState> changeEvent)
        {
            // UpdateSeats();

            // now let's find our local player in the list and update the character/info box appropriately
            int localPlayerIdx = -1;
            for (int i = 0; i < CharSelectData.LobbyPlayers.Count; ++i)
            {
                if (CharSelectData.LobbyPlayers[i].ClientId == NetworkManager.Singleton.LocalClientId)
                {
                    localPlayerIdx = i;
                    break;
                }
            }
            UpdatePlayerCount(localPlayerIdx);

            // case ERROR (dont care much)
            if (localPlayerIdx == -1)
            {
                // we aren't currently participating in the lobby!
                // this can happen for various reasons, such as the lobby being full and us not getting a seat.
                UpdateCharacterSelection(CharSelectData.SeatState.Inactive);
            }
            
            // else if (CharSelectData.LobbyPlayers[localPlayerIdx].SeatState == CharSelectData.SeatState.Inactive)
            // {
            //     // we haven't chosen a seat yet (or were kicked out of our seat by someone else)
            //     // Debug.Log("Inactive");
            //     UpdateCharacterSelection(CharSelectData.SeatState.Inactive);
            //     // make sure our player num is properly set in Lobby UI
            //     OnAssignedPlayerNumber(CharSelectData.LobbyPlayers[localPlayerIdx].PlayerNumber);
            // }
            else
            {
                // we have a seat! Note that if our seat is LockedIn, this function will also switch the lobby mode
                UpdateCharacterSelection(CharSelectData.LobbyPlayers[localPlayerIdx].SeatState, CharSelectData.LobbyPlayers[localPlayerIdx].PlayerNumber,CharSelectData.LobbyPlayers[localPlayerIdx].PlayerChamp,CharSelectData.LobbyPlayers[localPlayerIdx].PlayerTeam);
            }
        }

        /// <summary>
        /// Internal utility that sets the character-graphics and class-info box based on our chosen seat. 
        /// It also triggers a LobbyMode change when it notices that our seat-state is LockedIn.
        /// </summary>
        /// <param name="state">Our current seat state</param>
        /// <param name="champ">Which champ we're choose in</param>
        private void UpdateCharacterSelection(CharSelectData.SeatState state, int playerNumber = -1,CharacterTypeEnum champ = 0 , TeamType teamType = 0)
        {
            bool isNewChamp = m_LastChampSelected != champ;
            bool isNewTeam = m_LastTeamSelected != teamType;

            m_LastChampSelected = champ;
            m_LastTeamSelected = teamType;

            // Bat dau chon champion
            // if (state == CharSelectData.SeatState.Inactive)
            // {
            //     // Debug.Log("Inactive");
            //     if (m_CurrentCharacterGraphics)
            //     {
            //         m_CurrentCharacterGraphics.SetActive(false);
            //     }

            //     // m_UIAvatarInfoBox.ConfigureForNoSelection();
            // }

            // else
            // {

                // change character preview when selecting a new seat
                if (isNewChamp)
                {
                    // Lay thong tin champion
                    
                    // CharSelectData.AvatarByHero.TryGetValue(m_PlayerSeats[seatIdx].NameChampion,out avatar);
                    // Debug.Log(avatar.CharacterClass.CharacterType); 
                    Avatar newChamp =  CharSelectData.AvatarByHero[champ];
                    // var selectedCharacterGraphics = GetCharacterGraphics(newChamp, playerNumber);

                    // if (m_CurrentCharacterGraphics)
                    // {
                    //     m_CurrentCharacterGraphics.SetActive(false);
                    // }

                    // selectedCharacterGraphics.SetActive(true);
                    // m_CurrentCharacterGraphics = selectedCharacterGraphics;

                    m_UIAvatarInfoBox[playerNumber].ShowCharacterGraphic(newChamp);

                    // m_CurrentCharacterGraphicsAnimator = m_CurrentCharacterGraphics.GetComponent<Animator>();
                    // Debug.Log("player number " + playerNumber);
                    m_UIAvatarInfoBox[playerNumber].ConfigureForChampion(newChamp.Portrait,newChamp.CharacterClass);
                }
            
                if (isNewTeam){
                    m_UIAvatarInfoBox[playerNumber].ConfigureNewTeam(teamType);
                }


                if (state == CharSelectData.SeatState.LockedIn && !m_HasLocalPlayerLockedIn)
                {
                    // the local player has locked in their seat choice! Rearrange the UI appropriately
                    // the character should act excited
                    ConfigureUIForLobbyMode(CharSelectData.IsLobbyClosed.Value ? LobbyMode.LobbyEnding : LobbyMode.ChampChosen);
                    m_HasLocalPlayerLockedIn = true;
                }


            // }
        }


        private void UpdateBOTSelection(CharSelectData.SeatState state, int playerNumber = -1,CharacterTypeEnum champ = 0 , TeamType teamType = 0)
        {
            Debug.Log(playerNumber);
            bool isNewChamp = m_LastChampSelected != champ;
            bool isNewTeam = m_LastTeamSelected != teamType;

            m_LastChampSelected = champ;
            m_LastTeamSelected = teamType;

            // Bat dau chon champion
            if (state == CharSelectData.SeatState.Inactive )
            {
                Debug.Log("Inactive");
                m_UIAvatarInfoBox[playerNumber].NotShowCharacterGraphic();

            }
            
            if (playerNumber == -1) return;
                // change character preview when selecting a new seat
                if (isNewChamp)
                {
                    // Lay thong tin champion
                    
                    Avatar newChamp =  CharSelectData.AvatarByHero[champ];
                    m_UIAvatarInfoBox[playerNumber].ShowCharacterGraphic(newChamp);
                    m_UIAvatarInfoBox[playerNumber].ConfigureForChampion(newChamp.Portrait,newChamp.CharacterClass);
                }
            
                if (isNewTeam){
                    m_UIAvatarInfoBox[playerNumber].ConfigureNewTeam(teamType);
                }
                


            
        }

        /// <summary>
        /// Internal utility that sets the graphics for the eight lobby-seats (based on their current networked state)
        /// </summary>
        // private void UpdateSeats()
        // {
        //     // Players can hop between seats -- and can even SHARE seats -- while they're choosing a class.
        //     // Once they have chosen their class (by "locking in" their seat), other players in that seat are kicked out.
        //     // But until a seat is locked in, we need to display each seat as being used by the latest player to choose it.
        //     // So we go through all players and figure out who should visually be shown as sitting in that seat.
        //     CharSelectData.LobbyPlayerState[] curSeats = new CharSelectData.LobbyPlayerState[m_PlayerSeats.Count];
        //     foreach (CharSelectData.LobbyPlayerState playerState in CharSelectData.LobbyPlayers)
        //     {
        //         if (playerState.SeatIdx == -1 || playerState.SeatState == CharSelectData.SeatState.Inactive)
        //             continue; // this player isn't seated at all!
        //         if (    curSeats[playerState.SeatIdx].SeatState == CharSelectData.SeatState.Inactive
        //             || (curSeats[playerState.SeatIdx].SeatState == CharSelectData.SeatState.Active && curSeats[playerState.SeatIdx].LastChangeTime < playerState.LastChangeTime))
        //         {
        //             // this is the best candidate to be displayed in this seat (so far)
        //             curSeats[playerState.SeatIdx] = playerState;
        //         }
        //     }

        //     // now actually update the seats in the UI
        //     for (int i = 0; i < m_PlayerSeats.Count; ++i)
        //     {
        //         m_PlayerSeats[i].SetState(curSeats[i].SeatState, curSeats[i].PlayerNumber, curSeats[i].PlayerName);
        //     }
        // }

        private void StateChoseBackGround(bool previousValue, bool newValue)
        {
            ConfigureUIForLobbyMode(LobbyMode.ChooseBackGround);
            m_BackGroundBox.ConfigureBackGround(BackGroundSelectData.backGroundGameRegistry.m_BackGrounds[BackGroundSelectData.BackGroundNumber.Value]);

        }

        /// <summary>
        /// Called by the server when the lobby mode change 
        /// </summary>
        private void OnLobbyModeChange(LobbyMode lastLobbyMode, LobbyMode nowLobbyMode)
        {
            // Many Case : Chose AI , Close Loby
            ConfigureUIForLobbyMode(nowLobbyMode);
        }

        private void OnHostChangedBackground(int presValue, int newValue)
        {
            m_BackGroundBox.ConfigureBackGround(BackGroundSelectData.backGroundGameRegistry.m_BackGrounds[BackGroundSelectData.BackGroundNumber.Value]);
        }

        private void OnHostChangeNumberBOT(int previousValue, int newValue)
        {
            Debug.Log("nunber bot changed");
            m_BackGroundBox.ConfigureNumberBOT(newValue);
        }

        /// <summary>
        /// Turns on the UI elements for a specified "lobby mode", and turns off UI elements for all other modes.
        /// It can also disable/enable the lobby seats and the "Ready" button if they are inappropriate for the
        /// given mode.
        /// </summary>
        public void ConfigureUIForLobbyMode(LobbyMode mode , CharacterTypeEnum Champ = 0 )
        {
            
            // first the easy bit: turn off all the inappropriate ui elements, and turn the appropriate ones on!
            foreach (var list in m_LobbyUIElementsByMode.Values)
            {
                foreach (var uiElement in list)
                {
                    uiElement.SetActive(false);
                }
            }

            foreach (var uiElement in m_LobbyUIElementsByMode[mode])
            {
                uiElement.SetActive(true);
            }

            // that finishes the easy bit. Next, each lobby mode might also need to configure the lobby seats and class-info box.
            bool isSeatsDisabledInThisMode = false;
            switch (mode)
            {
                case LobbyMode.ChooseChamp:
                    // if ( m_LastChampSelected == -1)
                    // {
                    //     if (m_CurrentCharacterGraphics)
                    //     {
                    //         m_CurrentCharacterGraphics.gameObject.SetActive(false);
                    //     }
                    //     // m_UIAvatarInfoBox.ConfigureForNoSelection();
                    //     // m_BackGroundBox.ConfigureForNoSelection();
                    // }
                    m_BackGroundBox.ConfigureForNoSelection();
                    m_ReadyButtonText.text = "Lock!";
                    break;
                case LobbyMode.ChampChosen:
                    isSeatsDisabledInThisMode = true;
                    // m_UIAvatarInfoBox.SetLockedIn(true);
                    // m_ReadyButtonText.text = "UNREADY";
                    LockUIPlayerSeats.Invoke(isSeatsDisabledInThisMode );
                    break;

                case LobbyMode.ChooseBackGround:
                    isSeatsDisabledInThisMode = true;
                    
                    m_BackGroundBox.ConfigureForSelectionBackGround();
                    LockUIPlayerSeats.Invoke(isSeatsDisabledInThisMode );

                    // m_UIAvatarInfoBox.SetLockedIn(true); // Set UI checkmark , button color 
                    // m_ReadyButtonText.text = "Waitting";
                    if (NetworkManager.IsHost){
                        m_ReadyButtonText.text = "Start Game";
                    }
                    break;
                case LobbyMode.ChooseAI:
                    isSeatsDisabledInThisMode = false;
                    for (int i = 0; i < CharSelectData.LobbyBOTs.Count; ++i)
                    {
                        //  Avatar UI + ( name , team of the BOT ) (visual)
                        m_UIAvatarInfoBox[i].Initialize(CharSelectData.LobbyBOTs[i].PlayerName , i );
                        m_UIAvatarInfoBox[i].DisableChangeTeamInClient(true);
                        if (IsHost){
                            // TODO : need Improve here HUY
                            m_UIAvatarInfoBox[i].DisableChangeTeamInClient(false);
                        }
                    }
                    m_BackGroundBox.ConfigureForNoSelection();
                    LockUIPlayerSeats.Invoke(true);
                    if (IsHost){

                        LockUIPlayerSeats.Invoke(isSeatsDisabledInThisMode );
                    }

                    break;


                case LobbyMode.FatalError:
                    isSeatsDisabledInThisMode = true;
                    // m_UIAvatarInfoBox.ConfigureForNoSelection();
                    break;
                case LobbyMode.LobbyEnding:
                    isSeatsDisabledInThisMode = true;
                    // m_UIAvatarInfoBox.ConfigureForNoSelection();
                    break;
            }
            
            // go through all our seats and enable or disable buttons
            // LockUIPlayerSeats.Invoke(isSeatsDisabledInThisMode , Cha);
            // foreach (var seat in m_PlayerSeats)
            // {
            //     // disable interaction if seat is already locked or all seats disabled
            //     seat.SetDisableInteraction(seat.IsLocked() || isSeatsDisabledInThisMode);
            // }

        }

        #region Call Back Function from event unity (Button UI) 
        public void OnPlayerClickedTeamType(TeamType teamType)
        {
            CharSelectData.ChangeSomeThingServerRpc(NetworkManager.Singleton.LocalClientId,m_LastChampSelected , teamType, false);
        }
        /// <summary>
        /// Called directly by UI elements!
        /// </summary>
        /// <param name="seatIdx"></param>
        public void OnPlayerClickedSeat(CharacterTypeEnum champ)
        {
            CharSelectData.ChangeSomeThingServerRpc(NetworkManager.Singleton.LocalClientId, champ ,m_LastTeamSelected, false);
        }

        /// <summary>
        /// Called directly by UI elements! 
        /// </summary>
        public void OnPlayerClickedLockChamp()
        {
            // request to lock in or unlock if already locked in
            if ( BackGroundSelectData.LobbyModeChange.Value == LobbyMode.ChooseAI) {
                CharSelectData.ChangeSomeThingServerRpc(NetworkManager.Singleton.LocalClientId, m_LastChampSelected,m_LastTeamSelected, !m_HasLocalBOTLockedIn );
                return;
            }
            CharSelectData.ChangeSomeThingServerRpc(NetworkManager.Singleton.LocalClientId, m_LastChampSelected,m_LastTeamSelected, !m_HasLocalPlayerLockedIn );
        }

        

        public void OnHostChangedBackGround(bool left){
            if (NetworkManager.IsHost){
                BackGroundSelectData.ChangeBackGroundServerRpc(left);

            }

        }

        public void OnHostChangedNumberBot(int nbBot){
            if (NetworkManager.IsHost){
                BackGroundSelectData.HostChangeNumberBOTServerRpc(nbBot);
            }

        }

        public void OnHostClickedReadyOrStartGame(int nbBot)
        {
            if (NetworkManager.IsHost){
                BackGroundSelectData.HostGameReadyServerRpc(nbBot);
            }
        }



        // /// <summary>
        // /// Called directly by UI elements!
        // /// </summary>
        // public void OnPlayerExit()
        // {
        //     // Player is leaving the group
        //     // first disconnect then return to menu
        //     var gameNetPortal = GameObject.FindGameObjectWithTag("GameNetPortal").GetComponent<GameNetPortal>();
        //     gameNetPortal.RequestDisconnect();
        //     SceneManager.LoadScene("MainMenu");
        // }
        #endregion
                      
                      
        // Lay thong tin champion in Avatar SOs
    
        GameObject GetCharacterGraphics(Avatar avatar , int playerNumber)
        {

            if (!m_SpawnedCharacterGraphics.TryGetValue(avatar.Guid, out GameObject characterGraphics))
            {
                

                characterGraphics = Instantiate(avatar.GraphicsCharacterSelect, m_UIAvatarInfoBox[playerNumber].TransformMiniSpawn);
                // characterGraphics = Instantiate(avatar.Graphics, m_CharacterGraphicsParent);

                m_SpawnedCharacterGraphics.Add(avatar.Guid, characterGraphics);
            }

            return characterGraphics;
        }

// #if UNITY_EDITOR
//         private void OnValidate()
//         {
//             if (gameObject.scene.rootCount > 1) // Hacky way for checking if this is a scene object or a prefab instance and not a prefab definition.
//             {
//                 while (m_PlayerSeats.Count < CharSelectData.k_MaxLobbyPlayers)
//                 {
//                     m_PlayerSeats.Add(null);
//                 }
//             }
//         }
// #endif

    }
}