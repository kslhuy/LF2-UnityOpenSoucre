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



        private List<CharacterTypeEnum> m_LastChampSelected = new List<CharacterTypeEnum>(4){
            CharacterTypeEnum.Deep,
            CharacterTypeEnum.Deep,
            CharacterTypeEnum.Deep,
            CharacterTypeEnum.Deep,
        } ;
        private TeamType m_LastTeamSelected = 0;

        private int m_LastIndexBOTUpdateUI ;
        private CharacterTypeEnum m_LastTeamBOTSelected = 0;

        private int _numberPlayerInLobby ; 

        private int _localPlayerIdx ;



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
            _numberPlayerInLobby = CharSelectData.LobbyPlayers.Count; 
            // Debug.Log("number player " + _numberPlayerInLobby);
            for (int i = 0; i <_numberPlayerInLobby ; ++i)
            {
                //  Avatar UI + ( name , team of the player ) (visual)
                _localPlayerIdx = -1;
                if (CharSelectData.LobbyPlayers[i].ClientId == NetworkManager.Singleton.LocalClientId)
                {
                    _localPlayerIdx = i;
                    Debug.Log("local player " + _localPlayerIdx);

                }
                m_UIAvatarInfoBox[i].Initialize(CharSelectData.LobbyPlayers[i].PlayerName , i );
                m_LastChampSelected[i] = CharSelectData.LobbyPlayers[i].PlayerChamp;
            }

            ConfigureUIForLobbyMode(LobbyMode.ChooseChamp);
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
        private void UpdatePlayerCount()
        {
            int count = CharSelectData.LobbyPlayers.Count;
            var pstr = (count > 1) ? "players" : "player";
            m_NumPlayersText.text = "<b>" + count + "</b> " + pstr +" connected";




        }
        private void OnLobbyBOTStateChanged(NetworkListEvent<CharSelectData.LobbyPlayerState> changeEvent){
            int currentIdxBOT = -1;

            for (int i = 0; i < CharSelectData.LobbyBOTs.Count; ++i)
            {
                if (CharSelectData.LobbyBOTs[i].SeatState == CharSelectData.SeatState.LockedIn && m_LastIndexBOTUpdateUI <= i ){
                    // Last index is locked , but not update UI 
                    // So do it
                    UpdateCharacterSelection(CharSelectData.LobbyBOTs[i].SeatState, currentIdxBOT,CharSelectData.LobbyBOTs[i].PlayerChamp,CharSelectData.LobbyBOTs[i].PlayerTeam);
                    m_LastIndexBOTUpdateUI += 1;
                }
                else if (CharSelectData.LobbyBOTs[i].SeatState != CharSelectData.SeatState.LockedIn)
                {
                    currentIdxBOT = i;
                    break;
                }
                else currentIdxBOT = -1;
            }


            // case ERROR (dont care much)
            if (currentIdxBOT == -1) return;
            
            // we have a seat! Note that if our seat is LockedIn, this function will also switch the lobby mode
            UpdateCharacterSelection(CharSelectData.LobbyBOTs[currentIdxBOT].SeatState, currentIdxBOT,CharSelectData.LobbyBOTs[currentIdxBOT].PlayerChamp,CharSelectData.LobbyBOTs[currentIdxBOT].PlayerTeam);
            

        }

        

        /// <summary>
        /// Called by the server when any of the seats in the lobby have changed. (Including ours!)
        /// </summary>
        private void OnLobbyPlayerStateChanged(NetworkListEvent<CharSelectData.LobbyPlayerState> changeEvent)
        {
            
            // now let's find our local player in the list and update the character/info box appropriately
            
            for (int i = 0; i < CharSelectData.LobbyPlayers.Count; ++i)
            {
                if (CharSelectData.LobbyPlayers[i].ClientId == NetworkManager.Singleton.LocalClientId)
                {
                    _localPlayerIdx = i;
                    // Debug.Log(localPlayerIdx);
                    UpdateCharacterSelection(CharSelectData.LobbyPlayers[i].SeatState, CharSelectData.LobbyPlayers[i].PlayerNumber,CharSelectData.LobbyPlayers[i].PlayerChamp,CharSelectData.LobbyPlayers[i].PlayerTeam);
                    if (CharSelectData.LobbyPlayers[i].SeatState == CharSelectData.SeatState.LockedIn && !m_HasLocalPlayerLockedIn)
                    {
                        // the local player has locked in their seat choice! Rearrange the UI appropriately
                        // the character should act excited
                        ConfigureUIForLobbyMode(CharSelectData.IsLobbyClosed.Value ? LobbyMode.LobbyEnding : LobbyMode.ChampChosen);
                        m_HasLocalPlayerLockedIn = true;
                    }

                }
                else {
                    UpdateCharacterSelection(CharSelectData.LobbyPlayers[i].SeatState, CharSelectData.LobbyPlayers[i].PlayerNumber,CharSelectData.LobbyPlayers[i].PlayerChamp,CharSelectData.LobbyPlayers[i].PlayerTeam);
                }
            }
            // Debug.Log("local Player IDX change " + localPlayerIdx);
            UpdatePlayerCount();
            

        }

        /// <summary>
        /// Internal utility that sets the character-graphics and class-info box based on our chosen seat. 
        /// It also triggers a LobbyMode change when it notices that our seat-state is LockedIn.
        /// </summary>
        /// <param name="state">Our current seat state</param>
        /// <param name="champ">Which champ we're choose in</param>
        private void UpdateCharacterSelection(CharSelectData.SeatState state, int playerNumber = -1,CharacterTypeEnum champ = 0 , TeamType teamType = 0)
        {
            if (playerNumber == -1) return;
            bool isNewChamp = m_LastChampSelected[playerNumber] != champ;
            bool isNewTeam = m_LastTeamSelected != teamType;


        

            // change character preview when selecting a new seat
            if (isNewChamp)
            {
                m_LastChampSelected[playerNumber] = champ;

                // Lay thong tin champion

                Avatar newChamp =  CharSelectData.AvatarByHero[champ];


                m_UIAvatarInfoBox[playerNumber].ShowCharacterGraphic(newChamp);

                // m_CurrentCharacterGraphicsAnimator = m_CurrentCharacterGraphics.GetComponent<Animator>();
                // Debug.Log("player number " + playerNumber);
                m_UIAvatarInfoBox[playerNumber].ConfigureForChampion(newChamp.Portrait,newChamp.CharacterClass);
            }
        
            if (isNewTeam){
                m_LastTeamSelected = teamType;
                m_UIAvatarInfoBox[playerNumber].ConfigureNewTeam(teamType);
            }
        
        }


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

        }

        #region Call Back Function from event unity (Button UI) 
        public void OnPlayerClickedTeamType(TeamType teamType , int playerIndex)
        {
            CharSelectData.ChangeSomeThingServerRpc(NetworkManager.Singleton.LocalClientId,m_LastChampSelected[playerIndex] , teamType, false);
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
                CharSelectData.ChangeSomeThingServerRpc(NetworkManager.Singleton.LocalClientId, m_LastChampSelected[_localPlayerIdx],m_LastTeamSelected, true );
                return;
            }
            CharSelectData.ChangeSomeThingServerRpc(NetworkManager.Singleton.LocalClientId, m_LastChampSelected[_localPlayerIdx],m_LastTeamSelected, true );
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


    }
}