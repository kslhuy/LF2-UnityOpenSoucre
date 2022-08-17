using System;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;

////TODO: custom icon for OnScreenStick component


namespace UnityEngine.InputSystem.OnScreen
{

    
    /// <summary>
    /// A stick control displayed on screen and moved around by touch or other pointer
    /// input.
    /// </summary>
    public class DefenseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        private CanvasGroup canvasGroup;
        public Action DefenseAction;

        private bool def;
        private void Start()
        {
            m_StartPos = ((RectTransform)transform).anchoredPosition;

        }
        #region Defaut

             
        public float movementRange
        {
            get => m_MovementRange;
            set => m_MovementRange = value;
        }

        [FormerlySerializedAs("movementRange")]
        [SerializeField]
        private float m_MovementRange = 50;

        [InputControl(layout = "Button")]
        [SerializeField]
        private string m_ControlPath;

        private Vector3 m_StartPos;
        private Vector2 m_PointerDownPos;

        // protected override string controlPathInternal
        // {
        //     get => m_ControlPath;
        //     set => m_ControlPath = value;
        // }
        #endregion

        public event Action<bool,bool> ONdefenseEvent;

        public event Action<bool,bool,bool> ResetComboEvent;

        public void OnPointerDown(PointerEventData eventData)
        {
            
            // SendValueToControl(1.0f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
            ResetComboEvent?.Invoke(false,false,false);
            DefenseAction?.Invoke();
            canvasGroup.alpha = 0.4f;

            canvasGroup.blocksRaycasts = false;


            // RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
            // Debug.Log(m_PointerDownPos); 
        }


        
        public void OnDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 0.4f;
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
            var delta = position - m_PointerDownPos;

            delta = Vector2.ClampMagnitude(delta, movementRange);
            ((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)delta;
            // SendValueToControl(0.0f);
            def = true;
            ONdefenseEvent?.Invoke(def,false);

        }


        public void OnPointerUp(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;

            canvasGroup.blocksRaycasts = true;
            ((RectTransform)transform).anchoredPosition = m_StartPos;
            // SendValueToControl(0.0f);
            def = false;
            ONdefenseEvent?.Invoke(def,false);
        }


    }
}