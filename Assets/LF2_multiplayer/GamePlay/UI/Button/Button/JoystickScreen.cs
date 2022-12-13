using System;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;
// using Unity.Netcode;


////TODO: custom icon for OnScreenStick component

namespace UnityEngine.InputSystem.OnScreen
{
    /// <summary>
    /// A stick control displayed on screen and moved around by touch or other pointer
    /// input.
    /// </summary>

    public class JoystickScreen : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        CanvasGroup canvasGroup;
        
        public Action <int, int> SendControlValue; 
        public Action RunEvent; 


        [SerializeField] RectTransform _JoystickRectransforme ; 
        [SerializeField] RectTransform _buttonRectTranform;

        private float timeLastTouch;
        private short numberTouch; 
        private float timerRun = 0.3f;
        

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        private void Start() {

            m_StartPos = _buttonRectTranform.anchoredPosition;

        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_JoystickRectransforme, eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
            // Debug.Log("Point down position " + m_PointerDownPos);
            var delta = Vector2.ClampMagnitude(m_PointerDownPos, movementRange);
            _buttonRectTranform.anchoredPosition = m_StartPos + (Vector3)delta;
            SendControlValue?.Invoke(Mathf.RoundToInt(delta.x / movementRange) , Mathf.RoundToInt(delta.y / movementRange));
            canvasGroup.blocksRaycasts = false;

            numberTouch++;
            if (numberTouch == 2 && Time.time - timeLastTouch <= timerRun  ) RunEvent?.Invoke();
            if (numberTouch%2 == 0) numberTouch = 0;
            timeLastTouch = Time.time; 

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_JoystickRectransforme, eventData.position, eventData.pressEventCamera, out var position);

            var delta = Vector2.ClampMagnitude(position, movementRange);
            _buttonRectTranform.anchoredPosition = m_StartPos + (Vector3)delta;


            // Debug.Log(newPos.normalized); 
            // SendValueToControl(newPos);
            // Debug.Log("onDrag");
            SendControlValue?.Invoke(Mathf.RoundToInt(delta.x / movementRange) , Mathf.RoundToInt(delta.y / movementRange));
            


        }

        public void OnPointerUp(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            _buttonRectTranform.anchoredPosition = m_StartPos;
            SendControlValue?.Invoke(0,0);

   
        }





        public float movementRange
        {
            get => m_MovementRange;
            set => m_MovementRange = value;
        }

        [FormerlySerializedAs("movementRange")]
        [SerializeField]
        private float m_MovementRange = 50;

        // [InputControl(layout = "Vector2")]
        // [SerializeField]
        // private string m_ControlPath;

        private Vector3 m_StartPos;
        private Vector2 m_PointerDownPos;

        // protected override string controlPathInternal
        // {
        //     get => m_ControlPath;
        //     set => m_ControlPath = value;
        // }
    }
}

