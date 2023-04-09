using UnityEngine;
using System;
using LF2;
[CreateAssetMenu(menuName = "Events/Event Channel Basic")]
public class EventChannelSO : EventChannelBaseSO
{
    public Action OnEventRaised;

    public void RaiseEvent(){
        OnEventRaised?.Invoke();
    }

}