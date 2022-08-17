using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Server{

    public class PlayerDDJState : State
    {

        public PlayerDDJState(StateMachineServer player) : base(player)
        {
            m_skillsDescription = stateMachine.SkillDescription(GetId());
            subStateDetails = m_skillsDescription.DamageDetails;
            InputPackage data = new InputPackage();
            data.StateTypeEnum = GetId(); 
        
            logicState = stateMachine.MakeState(stateMachine ,ref data )  ;
        }



        public override void Enter()
        {      
            base.Enter();
            logicState.Enter();
        }
        public override void LogicUpdate() {
            logicState.LogicUpdate();
        }

        public override StateType GetId()
        {
            return StateType.DDJ1;
        }


        public override void End()
        {
            stateMachine.ChangeState(StateType.Idle);
        }

        public override void Exit()
        {
        }

    }
}
