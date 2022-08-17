using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 namespace LF2.Server{

    public class PlayerFallState : State
    {

        private int m_nbHurt; 

        public PlayerFallState(StateMachineServer player) : base(player)
        {
        }



        public override void Enter()
        {
            base.Enter();
            Data.StateTypeEnum = StateType.Falling;
            
;

        }

        public override void LogicUpdate()
        {
            // if (stateMachine.ServerCharacterMovement.IsGounded()){

            //     if (Time.time - TimeStarted_Server > 0.2f ){
            //         stateMachine.ChangeState(StateType.Laying);
            //     }
            // }
        }

        public override void Exit()
        {
            base.Exit();
        }


        public override StateType GetId()
        {
            return StateType.Falling;
        }

    }
 }
