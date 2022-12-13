using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.OnScreen
{
    public class UpSlotButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        public event Action<bool,bool,bool> upSlotEvent;
        static readonly Vector3 k_DownScale = new Vector3(0.9f, 0.9f, 0.9f);

        [SerializeField]
        private CanvasGroup canvasGroup;  
        private bool Def;
        private bool Up;
        private bool Down;
        private void Awake() {
            var defenseButton = GameObject.FindGameObjectWithTag("DefenseUI").GetComponent<DefenseButton>();            
            // DefenseButton defenseButton = GetComponent<DefenseButton>();
            // Debug.Log(defenseButton);
            defenseButton.ONdefenseEvent += DefenseEvent;
        }

        private void DefenseEvent(bool defEvent, bool UpEvent)
        {
            Def = defEvent;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {   
            if (Def) {
                canvasGroup.alpha = 0.4f;
                transform.localScale = k_DownScale;
                Up = true;
                upSlotEvent?.Invoke(Def ,Down , Up);
            }  
            Def = false;
            Up = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            transform.localScale = Vector3.one;
        }
    }

}