using UnityEngine;
using System;
using LF2;
[CreateAssetMenu(menuName = "Events/LifeState Event Channel")]
public class LifeStateEventChannelSO : EventChannelBaseSO
{
    public Action<LifeState> LifeStateEvent;

    public void RaiseEvent(LifeState lifeState){
        LifeStateEvent?.Invoke(lifeState);
    }

}