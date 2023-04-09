using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LF2.Client
{
    /// <summary>
    /// Controls one of the eight "seats" on the character-select screen (the boxes along the bottom).
    /// </summary>
    public class UICharSelectPlayerSeat : MonoBehaviour
    {
        

        [SerializeField]
        private Image m_ActiveImageVisuals;

        [SerializeField]
        private Button m_Button;


        [SerializeField] private CharacterTypeEnum m_Champion ;



        // the last SeatState we were assigned
        private CharSelectData.SeatState m_State =  CharSelectData.SeatState.Inactive;

        // once this is true, we're never clickable again!
        private bool m_IsDisabled;
        ClientCharSelectState m_ClientSelectState;

        private void Start() {
            m_ClientSelectState = GameObject.FindGameObjectWithTag ("ClientSelectState").GetComponent<ClientCharSelectState>();
            m_ClientSelectState.LockUIPlayerSeats += SetDisableInteraction;
        }

        // public void Initialize(int seatIndex)
        // {
        //     m_SeatIndex = seatIndex;
        //     m_State = CharSelectData.SeatState.Inactive;
        //     m_PlayerNumber = -1;
        //     ConfigureStateGraphics();
        // }




        public void SetDisableInteraction(bool disable )
        {
            
            if (disable ){
                m_Button.gameObject.SetActive(!disable);
                m_Button.interactable = !disable;
                m_IsDisabled = disable;    
                // return;
            }else{
                m_Button.gameObject.SetActive(!disable);
                m_Button.interactable = !disable;
                m_IsDisabled = disable;
            }
            
        }



        // Ajust some UI for visual 
        private void ConfigureStateGraphics()
        {
           
        }

        public void SetCharacterType(CharacterTypeEnum characterType , Sprite portrait){
            m_Champion = characterType; 
            m_ActiveImageVisuals.sprite = portrait;
        }


        // Called directly by Button in UI
        public void OnClicked()
        {
            ClientMusicPlayer.Instance.PlaySoundTap();
            ClientCharSelectState.Instance.OnPlayerClickedSeat(m_Champion);
        }

    }
}
