using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LF2.Client;

namespace LF2.Gameplay.UI
{
    /// <summary>
    /// Control one hero in the Hero shop UI
    /// Many component scritp in the Scene 
    /// </summary>
    public class UIShopCharSelect : MonoBehaviour
    {
        
        [SerializeField]
        private GameObject m_ActiveStateVisuals;
        
        [SerializeField]
        private Image m_ActiveImage;

        [SerializeField]
        private Button m_Button;

        [SerializeField] private CharacterTypeEnum Champion ;



        // once this is true, we're never clickable again!
        private bool m_IsDisabled;


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


        public void SetCharacterType(CharacterTypeEnum characterType){
            Champion = characterType;
        } 

        public void SetImage(Sprite imageHero){
            m_ActiveImage.sprite = imageHero;
        } 


        // Called directly by Button in UI
        public void OnClicked()
        {
            ClientMusicPlayer.Instance.PlaySoundTap();
            UIHeroInventory.Instance.OnPlayerClickedSeat(Champion);
        }

    }
}
