using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 namespace LF2.Server{

    public class PlayerHurtState : State
    {

        private int m_nbHurt; 

        public PlayerHurtState(StateMachineServer player) : base(player)
        {
        }


        public override void Enter()
        {
            base.Enter();

            Data.StateTypeEnum = StateType.Hurt1;
            

        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }


        public override void End()
        {
            stateMachine.ChangeState(StateType.Idle);

        }

        public override StateType GetId()
        {
            return StateType.Hurt1;
        }

    }
 }
