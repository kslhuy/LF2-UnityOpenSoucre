using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.OnScreen
{
    public class DownSlotButton :  MonoBehaviour , IPointerEnterHandler ,IPointerExitHandler
    {
        static readonly Vector3 k_DownScale = new Vector3(0.9f, 0.9f, 0.9f);
        private bool defOK;

        private bool Def ;

        private bool Down ;
        private bool Up ;

        [SerializeField]
        private CanvasGroup canvasGroup;     
        // [SerializeField]
        // Gradient gradient;

        // [SerializeField]
        // private TrailRenderer trail;

        public event Action<bool , bool,bool> downSlotEvent;


        private void Awake() {
            var defenseButton = GameObject.FindGameObjectWithTag("DefenseUI").GetComponent<DefenseButton>();            
            defenseButton.ONdefenseEvent += DefenseEvent;
        }

        private void DefenseEvent(bool defEvent, bool UpEvent)
        {
            Def = defEvent;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {   
            if (Def) {
                Down = true;
                downSlotEvent?.Invoke(Def ,Down , Up);
                canvasGroup.alpha = 0.4f;
                // trail.colorGradient = gradient;
                transform.localScale = k_DownScale;
            }  
            Def = false;
            Down = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            canvasGroup.alpha = 1.0f;
            transform.localScale = Vector3.one;
        }
    }

}