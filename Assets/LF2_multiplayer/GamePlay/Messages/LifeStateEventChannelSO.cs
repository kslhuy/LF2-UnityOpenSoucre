using UnityEngine;
using System;
using LF2;
[CreateAssetMenu(menuName = "Events/LifeState Event Channel")]
public class LifeStateEventChannelSO : EventChannelBaseSO
{
    public Action<LifeState , ulong> LifeStateEvent_Player;
    public Action<LifeState> LifeStateEvent_AI;


    public void RaiseEvent_Player(LifeState lifeState , ulong IDPlayer){
        LifeStateEvent_Player?.Invoke(lifeState , IDPlayer);
    }
    public void RaiseEvent_AI(LifeState lifeState ){
        LifeStateEvent_AI?.Invoke(lifeState );
    }
    

}