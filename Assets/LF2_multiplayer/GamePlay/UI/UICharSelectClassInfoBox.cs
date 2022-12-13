using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Collections;

namespace LF2.Client
{
    /// <summary>
    /// Controls the "information box" on the character-select screen.
    /// Name , Team  , Champion  of the player . 
    /// </summary>
    /// <remarks>
    /// This box also includes the "READY" button. The Ready button's state (enabled/disabled) is controlled
    /// here, but note that the actual behavior (when clicked) is set in the editor: the button directly calls
    /// ClientCharSelectState.OnPlayerClickedReady().
    /// </remarks>
    public class UICharSelectClassInfoBox : MonoBehaviour
    {

        // [SerializeField]
        // private Image m_InactiveImage;
        [SerializeField]
        private Image m_ActiveImage;
        
        [SerializeField]
        private TextMeshProUGUI m_NamePlayer;
        
        [SerializeField]
        private TextMeshProUGUI m_NameChampion;
        [SerializeField]
        private TextMeshProUGUI m_TeamTMP;

        private TeamType m_teamType = TeamType.INDEPENDANT ;

        [SerializeField]
        private Button m_ButtonSlectTeam;

        private TeamType[] Arr = (TeamType[])Enum.GetValues(typeof(TeamType));

        private int m_playerNumber;

        public Transform TransformMiniSpawn;

        private bool wasInitialied ; 

        Dictionary<Guid, GameObject> m_SpawnedCharacterGraphics = new Dictionary<Guid, GameObject>();

        GameObject m_CurrentCharacterGraphics;



        // [SerializeField]
        // Transform m_CharacterGraphicsParent;


                    /// Not use yet 

        // [SerializeField]
        // private Image m_ClassBanner;
        // [SerializeField]
        // private Image m_Skill1;
        // [SerializeField]
        // private Image m_Skill2;
        // [SerializeField]
        // private Image m_Skill3;

        // [SerializeField]
        // private GameObject m_Checkmark; // player locked his champ


        private bool m_IsLockedIn = false;
        private bool finishFlash;
        private bool alreadyTwinkling;

        public void Initialize(string playerName , int playerIndex)
        {
            if (wasInitialied) return;
            
            m_playerNumber = playerIndex;
            m_NamePlayer.text =  playerName;
            // m_InactiveImage.gameObject.SetActive(true);
            // m_ActiveImage.gameObject.SetActive(true);
            gameObject.SetActive(true);
            wasInitialied = true;
        }

        public void SetAvatarData(string playerName , int playerIndex){
            m_playerNumber = playerIndex;
            m_NamePlayer.text =  playerName;
        }

        public void DisableChangeTeamInClient(bool yes){
            
            if (yes) m_ButtonSlectTeam.interactable = false;
            m_ButtonSlectTeam.interactable = true;
        }

        public void ConfigureForNoSelection()
        {
            gameObject.SetActive(false);
            SetLockedIn(false);
        }


        public void DisableSelecteTeam(bool yes){
            m_ButtonSlectTeam.enabled = !yes;
        }


        #region Select Champion
  


        // UI lien quan den lockedIn (color Startbutton , checkmark )
        public void SetLockedIn(bool lockedIn)
        {
            // m_ReadyButtonImage.color = lockedIn ? Color.green : Color.white;
            m_IsLockedIn = lockedIn;
            // m_Checkmark.SetActive(lockedIn);
        }
        public void ConfigureNamePlayer(String namePlayer)
        {
            m_NamePlayer.text =  namePlayer;
        }
        // Public information of champs to screen 
        public void ConfigureForChampion(Sprite portrait , CharacterClass characterClass)
        {
            gameObject.SetActive(true);
            
            m_ActiveImage.sprite = portrait;
            // m_InactiveImage.gameObject.SetActive(false);
            m_NameChampion.text =  characterClass.CharacterType.ToString();
             
            m_TeamTMP.text = m_teamType.ToString();

            //// Not use yet 
            // ConfigureSkillIcon(m_Skill1, characterClass.Skill1);
            // ConfigureSkillIcon(m_Skill2, characterClass.Skill2);
            // ConfigureSkillIcon(m_Skill3, characterClass.Skill3);
        }

        public void ConfigureTwinklingInSelectChamp(bool stopFlash)
        {
            Debug.Log("stop falsh" + stopFlash);
            finishFlash = stopFlash;
            if (alreadyTwinkling) return;
            alreadyTwinkling = true;
            StartCoroutine(CoroTwinklingNameChamion());                
        }

        public IEnumerator CoroTwinklingNameChamion( float delay = 0.3f ){

            while (!finishFlash) {            

                    // for changing the alpha
                m_NameChampion.color = new Color(m_NameChampion.color.r, m_NameChampion.color.g, m_NameChampion.color.b, 0.4f);
                
                Debug.Log("twikling");
                // delay specified amount
                yield return new WaitForSeconds(delay);
    

                    // for changing the alpha
                m_NameChampion.color = new Color(m_NameChampion.color.r, m_NameChampion.color.g, m_NameChampion.color.b, 1);
                
                // delay specified amount
                yield return new WaitForSeconds(delay);
            }

            m_NameChampion.color = new Color(m_NameChampion.color.r, m_NameChampion.color.g, m_NameChampion.color.b, 1);



        }

        public void ConfigureNewTeam(TeamType teamType)
        {
            // Debug.Log("call team change");
            m_TeamTMP.text = teamType.ToString();
        }

        public void SetAvatarSetAvatarUI (Sprite portrait , TeamType teamType){
            m_ActiveImage.sprite = portrait;
            m_TeamTMP.text = teamType.ToString();
        }

        
        public GameObject GetCharacterGraphics(Avatar avatar )
        {

            if (!m_SpawnedCharacterGraphics.TryGetValue(avatar.Guid, out GameObject characterGraphics))
            {
                characterGraphics = Instantiate(avatar.GraphicsCharacterSelect, TransformMiniSpawn);
                // characterGraphics = Instantiate(avatar.Graphics, m_CharacterGraphicsParent);

                m_SpawnedCharacterGraphics.Add(avatar.Guid, characterGraphics);
            }

            return characterGraphics;
        }

        public void ShowCharacterGraphic(Avatar avatar){
            var selectedCharacterGraphics = GetCharacterGraphics(avatar);

            if (m_CurrentCharacterGraphics)
            {
                m_CurrentCharacterGraphics.SetActive(false);
            }

            selectedCharacterGraphics.SetActive(true);
            // Debug.Log("show graphic");
            m_CurrentCharacterGraphics = selectedCharacterGraphics;
        }

        public void NotShowCharacterGraphic(){

            if (m_CurrentCharacterGraphics)
            {
                m_CurrentCharacterGraphics.SetActive(false);
            }

        }





        /// Not use yet
        // private void ConfigureSkillIcon(Image iconSlot, StateType type)
        // {
        //     if (type == StateType.None)
        //     {
        //         iconSlot.gameObject.SetActive(false);
        //     }
        //     else
        //     {
        //         iconSlot.gameObject.SetActive(true);
        //         var data = GameDataSource.Instance.ActionDataByType[type];
        //         iconSlot.sprite = data.Icon;
        //         UITooltipDetector tooltipDetector = iconSlot.GetComponent<UITooltipDetector>();
        //         if (tooltipDetector)
        //         {
        //             tooltipDetector.SetText(string.Format(m_TooltipFormat, data.DisplayedName, data.Description));
        //         }
        //     }
        // }

        public void OnButtonTeamTypeChanged(){
            m_teamType = Next(m_teamType);
            ClientMusicPlayer.Instance.PlaySoundTap();
            ClientCharSelectState.Instance.OnPlayerClickedTeamType(m_teamType , m_playerNumber);
        }

        #endregion

        public TeamType Next(TeamType src)
        {
            int j = Array.IndexOf<TeamType>(Arr, src) + 1;
            return (Arr.Length==j) ? Arr[0] : Arr[j];            
        }
 
    }
}
