using UnityEngine;
using UnityEngine.UI;
using TMPro;


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

        [SerializeField]

        private GameObject m_SelectBackGroundClassSelected;

        private int nbBot = 0;

        [SerializeField]

        private TextMeshProUGUI NameBackGround;

        [SerializeField]
        private Button m_LeftBackGround;

        [SerializeField]
        private Image m_ImageBackGround;

        [SerializeField]
        private Image m_ReadyButtonImage;
        [SerializeField]
        private TextMeshProUGUI m_TextReadyButton;

        [SerializeField] Image[] ImageNumberBOT  ;
        private int _nbBOTcurrent ;

 

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
            m_ReadyButtonImage.color = lockedIn ? Color.green : Color.white;
            m_IsLockedIn = lockedIn;
            // m_Checkmark.SetActive(lockedIn);
        }




        #region Select BackGround 
        public void ConfigureForSelectionBackGround()
        {
            m_SelectBackGroundClassSelected.SetActive(true);

        }
                    /// Not use yet 

        public void ConfigureBackGround(BackGroundGame backGroundGame)
        {
            m_ImageBackGround.sprite = backGroundGame.BackGroundImage;
            NameBackGround.text = backGroundGame.NameBackGround;
            SetLockedIn(false);
        }

        #endregion
        public void ConfigureNumberBOT(int numberBot){
            if ( _nbBOTcurrent != numberBot){
                ImageNumberBOT[_nbBOTcurrent].color = Color.white;
                _nbBOTcurrent = numberBot;
                ImageNumberBOT[_nbBOTcurrent].color = Color.blue;       

            }
        }


        public void OnClickedNextBackGround(bool Left)
        {
            ClientCharSelectState.Instance.OnHostChangedBackGround(Left);
        }
        public void OnClickNumberCOMAI(int i){
            ClientCharSelectState.Instance.OnHostChangedNumberBot(i);

            nbBot = i;
            if (nbBot > 0){
                m_TextReadyButton.text = "READY!";
            }else{
                m_TextReadyButton.text = "START!";
            }
        }

        public void OnClickReady(){
            ClientCharSelectState.Instance.OnHostClickedReadyOrStartGame(nbBot);
        }
    }
}
