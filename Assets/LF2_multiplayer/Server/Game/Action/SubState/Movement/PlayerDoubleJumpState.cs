using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Server
{
    public class PlayerDoubleJumpState : PlayerAirState
    {
        public PlayerDoubleJumpState(StateMachineServer player) : base(player)
        {
        }


        public override StateType GetId()
        {
            return StateType.DoubleJump;
        }

        public override void Enter()
        {
            base.Enter();
            Data.StateTypeEnum = StateType.DoubleJump;
            
        }

        public override void LogicUpdate(){
            base.LogicUpdate();
        }


    }
}