using System;

using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;


namespace UnityEngine.InputSystem.OnScreen
{
    public class RunRightButton : MonoBehaviour ,IDropHandler
    {
        public event Action runRightEvent;

        public void OnDrop(PointerEventData eventData)
        {
            runRightEvent?.Invoke();
        }
    }
}
