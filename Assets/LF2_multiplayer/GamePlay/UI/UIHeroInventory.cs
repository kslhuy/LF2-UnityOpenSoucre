using System;
using System.Collections.Generic;
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
            
        }   
    }
}
