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
        
        public Action <Vector2> SendControlValue; 
        [SerializeField] CanvasRenderer runSlotsLeft;
        [SerializeField] CanvasRenderer runSlotsRight;
        
        // public override void NetworkStart()
        // {
        //     if (!IsClient)
        //     {
        //         // Disable server component on clients
        //         enabled = false;
        //         return;
        //     }
        // }
        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
            // runSlotsRight.GetComponent<CanvasRenderer>().SetAlpha(0);
            // runSlotsLeft.GetComponent<CanvasRenderer>().SetAlpha(0);
        }
        
        private void Start() {
            runSlotsRight.SetAlpha(0);
            runSlotsLeft.SetAlpha(0);
            m_StartPos = ((RectTransform)transform).anchoredPosition;

        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
            var delta = position - m_PointerDownPos;

            delta = Vector2.ClampMagnitude(delta, movementRange);
            ((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)delta;

            var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
            newPos = newPos.normalized;
            // Debug.Log(newPos.normalized); 
            // SendValueToControl(newPos);
            // Debug.Log("onDrag");
            SendControlValue?.Invoke(newPos);
            

            if (delta.x >= 20){
                runSlotsRight.SetAlpha(1);
                runSlotsLeft.SetAlpha(0);

                // runSlotsRight.SetActive(true);
                // runSlotsLeft.SetActive(false);
            }
            else if(delta.x <= -20){
                runSlotsRight.SetAlpha(0);
                runSlotsLeft.SetAlpha(1);

                // runSlotsRight.SetActive(false);
                // runSlotsLeft.SetActive(true);
            }
            else {
                runSlotsRight.SetAlpha(0);
                runSlotsLeft.SetAlpha(0);
                // runSlotsRight.SetActive(false);
                // runSlotsLeft.SetActive(false);
            }


            // Debug.Log(delta); 
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            ((RectTransform)transform).anchoredPosition = m_StartPos;
            // SendValueToControl(Vector2.zero);
            SendControlValue?.Invoke(Vector2.zero);

            runSlotsRight.SetAlpha(0);
            runSlotsLeft.SetAlpha(0);
   
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

