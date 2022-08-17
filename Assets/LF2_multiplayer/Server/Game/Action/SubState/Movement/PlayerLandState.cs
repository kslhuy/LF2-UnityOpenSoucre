using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LF2.Server{
    public class PlayerLandState : State
    {
        public PlayerLandState(StateMachineServer player) : base(player)
        {
        }

        public override bool CanChangeState(InputPackage actionRequestData)
        {
            // Attack , Jump , Defense 
            // DDA , DUA  
            if (actionRequestData.StateTypeEnum == StateType.Jump){
                stateMachine.ChangeState(StateType.DoubleJump);
                return true;
            }
            return false;

        }

        public override void Enter()
        {
            base.Enter();
            Data.StateTypeEnum = StateType.Land;
            
        }

        public override void LogicUpdate()
        {
        }

        public override StateType GetId()
        {
            return StateType.Land;
        }

        public override void End()
        {
            base.End();
        }

    }
}
