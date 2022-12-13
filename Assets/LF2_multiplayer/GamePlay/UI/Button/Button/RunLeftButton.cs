using System;

using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;


namespace UnityEngine.InputSystem.OnScreen
{
    public class RunLeftButton : MonoBehaviour ,IDropHandler
    {
        public event Action runLeftEvent;

        public void OnDrop(PointerEventData eventData)
        {
            runLeftEvent?.Invoke();
        }
    }
}
