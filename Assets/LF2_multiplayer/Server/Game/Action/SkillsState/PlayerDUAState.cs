using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Server{

    public class PlayerDUAState : State
    {

        // float m_MaxDistance = 0.35f;


        // private ulong m_ProvisionalTarget;


        public PlayerDUAState(StateMachineServer player) : base(player)
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

        public override StateType GetId()
        {
            return StateType.DUA1;
        }

        public override void LogicUpdate()
        {
            logicState.LogicUpdate();
        }

        public override void End()
        {
            stateMachine.ChangeState(StateType.Idle);
            // m_ExecutionFired = false;
        }

        public override void Exit()
        {
            logicState.Exit();
        }




    }
}
