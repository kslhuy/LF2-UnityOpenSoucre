using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Collections;
using LF2.Utils;
using LF2.Client;
using LF2.Data;
using System.Threading.Tasks;
using UnityGamingServicesUseCases;

namespace LF2.Gameplay.UI
{
    /// <summary>
    /// Only have one Script in the scene 
    /// Controls the "information box" on the character-select screen.
    /// Name   , Champion  of the player . 
    /// </summary>
    /// <remarks>
    /// This box also includes the "READY" button. The Ready button's state (enabled/disabled) is controlled
    /// here, but note that the actual behavior (when clicked) is set in the editor: the button directly calls
    /// ClientCharSelectState.OnPlayerClickedReady().
    /// </remarks>
    public class UIShopCharSelectInfoBox : MonoBehaviour
    {

        // [SerializeField]
        // private Image m_InactiveImage;

        
        [SerializeField]
        private TextMeshProUGUI m_NameChampion;

        [SerializeField]
        private TextMeshProUGUI m_HP;

        [SerializeField]
        private TextMeshProUGUI m_Mana;

        [SerializeField]
        private TextMeshProUGUI m_Amor;

        public Transform TransformSpawnCharacter;

        public Transform TransformSpawnSkill;

        public GameObject SkillObjectSpawn;

        [SerializeField] UIPopUpBuy UIPopUpBuy;

        Dictionary<CharacterTypeEnum, GameObject> m_SpawnedCharacterGraphics = new Dictionary<CharacterTypeEnum, GameObject>();

        Dictionary<CharacterTypeEnum, List<GameObject>> m_SpawnedCharacterSkills = new Dictionary<CharacterTypeEnum, List<GameObject>>();

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

        private CharacterTypeEnum LastCharacterSelect = CharacterTypeEnum.NONE ;
        private ComboSkill currentSelectSkillToBuy;






        #region Select Champion




        public void ConfigureBasicInfoCharacter(string namePlayer , string hp , string mp , string amor)
        {
            m_NameChampion.text =  namePlayer;
            m_HP.text = hp;
            m_Mana.text = mp;
            m_Amor.text = amor;
        }
        // Public information of champs to screen 




 

        
        public GameObject GetCharacterGraphics(Avatar avatar )
        {
            // look in to dict , if we were already spawned it ...  
            if (!m_SpawnedCharacterGraphics.TryGetValue(avatar.CharacterClass.CharacterType, out GameObject characterGraphics))
            {
                characterGraphics = Instantiate(avatar.GraphicsCharacterSelect, TransformSpawnCharacter);
                m_SpawnedCharacterGraphics.Add(avatar.CharacterClass.CharacterType, characterGraphics);
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





        public void ShowALLInfo(Avatar avatar)
        {

            var characterClass = avatar.CharacterClass;
            ConfigureBasicInfoCharacter(characterClass.CharacterType.ToString(), characterClass.BaseHP.Value.ToString(), characterClass.BaseMP.Value.ToString(), characterClass.BaseAmor.ToString());
            ShowCharacterGraphic(avatar);
            ShowCharacterComboSkills(avatar);

        }

        private void ShowCharacterComboSkills(Avatar avatar)
        {
            var newCharacterType = avatar.CharacterClass.CharacterType;
            if (LastCharacterSelect != newCharacterType){
                // 1 : Disable all the skills first 
                if (LastCharacterSelect != CharacterTypeEnum.NONE){
                    foreach (var skill in m_SpawnedCharacterSkills[LastCharacterSelect]){
                        skill.SetActive(false);
                    } 
                }
                // 2 : Check if need to spawn new skills of the character 
                if (!m_SpawnedCharacterSkills.TryGetValue(newCharacterType, out List<GameObject> characterSkills))
                {
                    characterSkills = new List<GameObject>();
                    foreach (ComboSkill comboSkill in avatar.CharacterStateSOs.ListAllComboSkills)
                    {
                        var SkillInfogameobject = Instantiate(SkillObjectSpawn, TransformSpawnSkill);
                        SkillInfogameobject.GetComponent<UIShopSkillInfo>().SetSkillInfo(this, comboSkill);
                        characterSkills.Add(SkillInfogameobject);
                    }
                    // register in the dict , 
                    m_SpawnedCharacterSkills[newCharacterType] = characterSkills;
                }
                // Here we have already of the character in the dict , so just enable this 
                else{
                    foreach (var skill in m_SpawnedCharacterSkills[newCharacterType]){
                        skill.SetActive(true);
                    } 
                }

            }
            LastCharacterSelect = newCharacterType;
        }

        #endregion

        public void OnPurchaseClicked(ComboSkill comboSkill)
        {
            // Popup which currency is choice for purchase (COIN or DIAMAND)
            UIPopUpBuy.Show(comboSkill.PriceCoin , comboSkill.PriceDiamand);
            // 
            currentSelectSkillToBuy = comboSkill;
            // currentBalance
            // UIPopUpBuy.Show();

        }
        public void OnPurchaseByDiamand(){
            // TODO : Change Current ID of PERL 
            int currentBalance = EconomyManager.instance.GetCurrencyBalance("PERL");
            if (currentBalance >= currentSelectSkillToBuy.PriceDiamand){
                UIPopUpBuy.Success();
            }
            else{
                UIPopUpBuy.Fail();
            }
        }

        public void OnPurchaseByCoin( ){
            int currentBalance = EconomyManager.instance.GetCurrencyBalance("COIN");
            if (currentBalance >= currentSelectSkillToBuy.PriceCoin){
                UIPopUpBuy.Success();
            }
            else{
                UIPopUpBuy.Fail();
            }
        }


    }
}
