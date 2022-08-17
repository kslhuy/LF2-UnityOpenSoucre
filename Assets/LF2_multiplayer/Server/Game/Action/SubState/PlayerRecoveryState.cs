using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 namespace LF2.Server{

    public class PlayerRecoveryState : State
    {

        public PlayerRecoveryState(StateMachineServer player) : base(player)
        {
        }



        public override void Enter()
        {
            base.Enter();
            Data.StateTypeEnum = StateType.Laying;
            
;

        }

        public override void Exit()
        {
            base.Exit();
        }


        public override void End()
        {
            stateMachine.ChangeState(StateType.Idle);
        }

        public override StateType GetId()
        {
            return StateType.Laying;
        }

    }
 }
