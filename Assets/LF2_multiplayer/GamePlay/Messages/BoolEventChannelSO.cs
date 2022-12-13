using UnityEngine;
using System;
using LF2;
[CreateAssetMenu(menuName = "Events/BoolEventChannelSO")]
public class BoolEventChannelSO : EventChannelBaseSO
{
    public Action<bool> ActionBool;

    public void RaiseEvent(bool eventArg){
        ActionBool?.Invoke(eventArg);
    }

}