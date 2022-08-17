using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Server{
    public class PlayerRollingState : State
    {

        public PlayerRollingState(StateMachineServer player) : base(player)
        {
        }

        public override bool CanChangeState(InputPackage requestData)
        {
            if (requestData.StateTypeEnum == StateType.DDA1 ||
                requestData.StateTypeEnum == StateType.DDJ1||
                requestData.StateTypeEnum == StateType.DUA1||
                requestData.StateTypeEnum == StateType.DUJ1 ){
                    // Make Queue State so we can perform a sequence of State . 
                    stateMachine.QueueState.Add(stateMachine.MakeState(stateMachine , ref requestData));
                    return true;
                }
                return false;
        }
            

        public override void Enter()
        {
            base.Enter();
            Data.StateTypeEnum = StateType.Rolling;
            
        }

        public override StateType GetId()
        {
            return StateType.Rolling;
        }

        public override void LogicUpdate()
        {
            
        }
    }
}
