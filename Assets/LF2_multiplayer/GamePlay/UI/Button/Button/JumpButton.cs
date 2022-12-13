using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using LF2;


namespace UnityEngine.InputSystem.OnScreen
{
    public class JumpButton : MonoBehaviour, IPointerDownHandler,IDropHandler,IPointerUpHandler
    {
        static readonly Vector3 k_DownScale = new Vector3(0.9f, 0.9f, 0.9f);

        public Action<StateType> JumpAction;
        
        [SerializeField]
        private CanvasGroup canvasGroup;
        // [InputControl(layout = "Button")]
        // [SerializeField]
        // private string m_ControlPath;
        private bool Up;
        private bool Def;
        private bool Down;

        private UpSlotButton slotUpButton;
        private DownSlotButton slotDownButton;

        private DefenseButton defenseButton;

        private void Awake() {
            slotUpButton = GameObject.FindGameObjectWithTag("UpSlotUI").GetComponent<UpSlotButton>();            
            slotDownButton = GameObject.FindGameObjectWithTag("DownSlotUI").GetComponent<DownSlotButton>();       
            defenseButton = GameObject.FindGameObjectWithTag("DefenseUI").GetComponent<DefenseButton>();            

            slotUpButton.upSlotEvent += SetSlotEvent;
            slotDownButton.downSlotEvent += SetSlotEvent;
            defenseButton.ResetComboEvent += SetSlotEvent;
        }

        private void SetSlotEvent(bool defEvent ,bool downEvent , bool upEvent)
        {
            Down = downEvent;
            Def = defEvent;
            Up = upEvent;
        }

        
        public void OnPointerDown(PointerEventData eventData)
        {
            canvasGroup.alpha = 0.6f;
            transform.localScale = k_DownScale;
            JumpAction?.Invoke(StateType.Jump);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (Up && Def) 
            {
                JumpAction?.Invoke(StateType.DUJ1);
                Def = false;
                Up = false;
            }

            else if (Down && Def) 
            {
                JumpAction?.Invoke(StateType.DDJ1);
                Def = false;
                Down = false;
            }

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            transform.localScale = Vector3.one;
        }
    }

}