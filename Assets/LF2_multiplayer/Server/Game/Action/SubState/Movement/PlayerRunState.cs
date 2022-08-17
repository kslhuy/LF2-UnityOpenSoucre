using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LF2.Server{

    public class PlayerRunState : State
    {
        private float runVelocity ;

        public PlayerRunState(StateMachineServer player) : base(player)
        {
        }

        public override bool CanChangeState(InputPackage actionRequestData)
        {

            if (actionRequestData.StateTypeEnum == StateType.Jump ) {
                stateMachine.ChangeState(StateType.DoubleJump); 
                return true;}
            else if(actionRequestData.StateTypeEnum == StateType.Attack){
                stateMachine.ChangeState( StateType.AttackRun);
                return true;
            }
            else if (actionRequestData.StateTypeEnum == StateType.Defense){
                stateMachine.ChangeState(StateType.Rolling);
                return true;}

            return false;
        }

        public override void Enter()
        {
        }


        public override void Exit()
        {
            base.Exit();
        }

        public override StateType GetId()
        {
            return StateType.Run;
        }





    }
}
