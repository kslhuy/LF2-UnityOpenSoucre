using System;
using System.Collections.Generic;
using LF2.Data;
// using LF2.Test;
using UnityEngine;
using UnityEngine.UI;

namespace LF2.Gameplay.UI
{
    /// Only have one component scritp in the Scene  
    public class UIHeroInventory : MonoBehaviour
    {

        public static UIHeroInventory Instance { get; private set; }
        public AvatarRegistry avatarRegistry;

        public RectTransform rectTF_HeroHUD;
        public UIShopCharSelect pf_HeroHUD;

        private bool isShowHeros;



        public UIShopCharSelectInfoBox uIShopCharSelectInfoBox;

        public SkillSlot[] skillSlots;
        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }

        }

        [SerializeField]
        CanvasGroup m_CanvasGroup;
        public void Show()
        {
            gameObject.SetActive(true);
            m_CanvasGroup.alpha = 1f;
            m_CanvasGroup.blocksRaycasts = true;

            ShowHeros();
        }

        private void ShowHeros()
        {
            if (isShowHeros) return;
            foreach (var hero in avatarRegistry.m_Avatars)
            {
                UIShopCharSelect charSelectShop = Instantiate(pf_HeroHUD, rectTF_HeroHUD);
                charSelectShop.SetCharacterType(hero.CharacterClass.CharacterType);
                charSelectShop.SetImage(hero.Portrait);
            }
            isShowHeros = true;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            m_CanvasGroup.alpha = 0f;
            m_CanvasGroup.blocksRaycasts = false;
        }

        public void OnPlayerClickedSeat(CharacterTypeEnum champion)
        {
            Debug.Log(avatarRegistry);

            avatarRegistry.TryGetAvatar(champion , out Avatar avatar);
            Debug.Log(avatar);
            uIShopCharSelectInfoBox.ShowALLInfo(avatar);
            selectedChar = champion;
            // foreach (SkillSlot slot in skillSlots) {
            //     slot.avatar = avatar;
            //     if (avatar.CharacterStateSOs.skillConfig.state(slot.state) == StateType.NONE) {
            //         slot.text.text = slot.state.ToString();
            //         continue;
            //     }
            //     foreach (ComboSkill cb in avatar.CharacterStateSOs.ListAllComboSkills)
            //         if (cb.StateLogicSO[0].StateType == avatar.CharacterStateSOs.skillConfig.state(slot.state)) {
            //             slot.text.text = slot.state.ToString() + '\n' + cb.nameSkills ;
            //         }
            // }
            hideSlotButton();
        }   

        public void showSlotButton() {
            foreach (SkillSlot slot in skillSlots) {
                slot.image.gameObject.SetActive(true);
            }
        } 

        public void hideSlotButton() {
            foreach (SkillSlot slot in skillSlots) {
                slot.image.gameObject.SetActive(false);
            }
        } 
        public CharacterTypeEnum selectedChar;
        public UIShopSkillInfo selectedSkill;
    }
}
