using UnityEngine;
using UnityEngine.UI;
using TMPro;
// using UnityEngine.AddressableAssets;


namespace LF2.Client
{
    /// <summary>
    /// Controls the BackGround-select screen.
    /// </summary>
    /// <remarks>
    /// This box also includes the "READY" button. The Ready button's state (enabled/disabled) is controlled
    /// here, but note that the actual behavior (when clicked) is set in the editor: the button directly calls
    /// ClientCharSelectState.OnPlayerClickedReady().
    /// </remarks>




    public class UIBackGroundSelectBox : MonoBehaviour
    {



        [Header("------ Lock Champ ---------")]
        [SerializeField]
        private Button m_LockChampButton;

        [Header("--------- Select Back Ground ---------")]

        [SerializeField] GameMode m_GamePlayMode = GameMode.VS;
        [SerializeField]
        private GameObject m_SelectBackGroundClassSelected;
        [SerializeField]

        private TextMeshProUGUI NameBackGround;

        [SerializeField]
        private Button m_LeftButton;
        [SerializeField]
        private Button m_RightButton;

        [SerializeField]
        private Image m_ImageBackGround;

        [SerializeField]
        [Tooltip("Text element for the Ready button")]

        private TextMeshProUGUI m_TextReadyButton;

        [Header("--------- Select NumberBot ---------")]

        [SerializeField] GameObject m_HowManyCom;

        [SerializeField] Color color_selectBot;
        [SerializeField] Color colorDefaut_selectBot;

        [SerializeField] Image[] ImageNumberBOT;
        private int _nbBOTcurrent = 0;
        
        [SerializeField] TransformVariable m_NetworkGameStateTransform;




        // [SerializeField]
        // private GameObject m_Checkmark; // player locked his champ

        private bool m_IsLockedIn = false;

        public void ConfigureForNoSelection()
        {

            m_SelectBackGroundClassSelected.SetActive(false);

            SetLockedIn(false);
        }



        // UI lien quan den lockedIn (color Startbutton , checkmark )
        public void SetLockedIn(bool lockedIn)
        {

            // m_ReadyButtonImage.color = lockedIn ? Color.green : Color.white;
            m_IsLockedIn = lockedIn;
            // m_Checkmark.SetActive(lockedIn);
        }

        public void LockChampButton(bool enable)
        {
            m_LockChampButton.gameObject.SetActive(enable);
            m_TextReadyButton.gameObject.SetActive(enable);
        }





        #region Select BackGround 
        public void ConfigureForSelectionBackGround()
        {

            m_SelectBackGroundClassSelected.SetActive(true);
            ImageNumberBOT[_nbBOTcurrent].color = color_selectBot;
            // if (GameMode.Stage == m_GamePlayMode){
            //     m_HowManyCom.SetActive(false);
            // }
            // else if (GameMode.VS == m_GamePlayMode){
            //     m_HowManyCom.SetActive(true);
            // }
        }

        public void NextBackGroundButton(bool enable)
        {
            m_LeftButton.gameObject.SetActive(enable);
            m_RightButton.gameObject.SetActive(enable);
        }
        /// Not use yet 

        public void ConfigureBackGround(BackGroundGame backGroundGame)
        {
            m_ImageBackGround.sprite = backGroundGame.BackGroundImage;
            NameBackGround.text = backGroundGame.NameBackGround.ToString(); 
            // System.Text.RegularExpressions.Regex.Replace(backGroundGame.NameBackGround.ToString(), "(\\B[A-Z])", " $1");
            SetLockedIn(false);
        }

        #endregion
        public void ConfigureNumberBOT(int numberBot)
        {
            if (_nbBOTcurrent != numberBot)
            {
                ImageNumberBOT[_nbBOTcurrent].color = colorDefaut_selectBot;
                _nbBOTcurrent = numberBot;
                ImageNumberBOT[_nbBOTcurrent].color = color_selectBot;

            }
        }


        public void OnClickedNextBackGround(bool Left)
        {
            ClientCharSelectState.Instance.OnHostChangedBackGround(Left);
        }
        public void OnClickNumberCOMAI(int i)
        {
            ClientCharSelectState.Instance.OnHostChangedNumberBot(i);

            _nbBOTcurrent = i;
            if (_nbBOTcurrent > 0)
            {
                m_TextReadyButton.text = "READY!";
            }
            else
            {
                m_TextReadyButton.text = "START!";
            }
        }

        public void OnClickReady()
        {
            ClientCharSelectState.Instance.OnHostClickedReadyOrStartGame(_nbBOTcurrent);
        }

        public void OnClickedVSMode()
        {
            // m_GamePlayMode = ;
            // ConfigureForSelectionBackGround();
            SetGameMode(GameMode.VS);

        }

        public void OnClickedStageMode()
        {
            // m_GamePlayMode = GameMode.Stage;
            SetGameMode(GameMode.Stage);

            // ConfigureForSelectionBackGround();
        }

        void SetGameMode(GameMode gameMode)
        {
            if (m_NetworkGameStateTransform && m_NetworkGameStateTransform.Value &&
                m_NetworkGameStateTransform.Value.TryGetComponent(out NetworkGameState networkGameState))
            {
                networkGameState.NetworkGameMode.gameMode.Value = gameMode;
            }
        }
    }
}
