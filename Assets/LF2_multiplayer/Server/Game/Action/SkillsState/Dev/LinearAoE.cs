using System.Collections.Generic;
using UnityEngine;
namespace LF2.Server
{
    public class LinearAoE : State
    {
        private bool m_OnceTime;


        public LinearAoE(StateMachineServer statMachine, ref InputPackage data) : base(statMachine)
        {
            m_stateType = data.StateTypeEnum;
            m_skillsDescription = stateMachine.SkillDescription(m_stateType);
            subStateDetails = m_skillsDescription.DamageDetails;
        }
        
        public override void Enter()
        {            
            
;
        }



        public override void End(){
            // stateMachine.idle();
            stateMachine.ChangeState(StateType.Idle);
        }



        public override StateType GetId()
        {
            return m_stateType;
        }

    }
}