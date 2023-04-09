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
    public class UIPopUpBuy : MonoBehaviour
    {
        
        [SerializeField]
        public GameObject m_Success;
        
        [SerializeField]
        public GameObject m_Fail;


        public UIShopCharSelectInfoBox uIShopCharSelectInfo;

        // once this is true, we're never clickable again!
        private bool m_IsDisabled;


        public void Show(){
            gameObject.SetActive(true);
        }

        public void Success(){
            gameObject.SetActive(true);
            m_Success.SetActive(true);
            m_Fail.SetActive(false);
        }
        public void Fail(){
            gameObject.SetActive(true);
            m_Success.SetActive(false);
            m_Fail.SetActive(true);
        }




        // Called directly by Button in UI
        public void OnClickedDiamon()
        {
            // uIShopCharSelectInfo.
        }

        public void OnClickedCoin()
        {
            
        }

        public void ClickedClose()
        {
            m_Fail.SetActive(false);
            m_Success.SetActive(false);

            gameObject.SetActive(false);
        }

    }
}
