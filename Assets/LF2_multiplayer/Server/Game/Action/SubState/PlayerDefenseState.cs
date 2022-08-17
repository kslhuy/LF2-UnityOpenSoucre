using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Server{

    public class PlayerDefenseState : State
    {
        private int xInput;

        public PlayerDefenseState(StateMachineServer player) : base(player)
        {
            m_skillsDescription = stateMachine.SkillDescription(StateType.Defense);

        }

        public override bool CanChangeState(InputPackage actionRequestData)
        {
            if (actionRequestData.StateTypeEnum == StateType.DDA1||
                actionRequestData.StateTypeEnum == StateType.DDJ1||
                actionRequestData.StateTypeEnum == StateType.DUA1||
                actionRequestData.StateTypeEnum == StateType.DUJ1){
                stateMachine.ChangeState(actionRequestData.StateTypeEnum);
                return true;
            }
            return false;
        }

        public override void Enter()
        {
            base.Enter();
            Data.StateTypeEnum = StateType.Defense;
            
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
            return StateType.Defense;
        }


    }
}
