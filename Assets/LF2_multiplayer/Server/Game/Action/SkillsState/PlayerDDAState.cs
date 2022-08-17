using Unity.Netcode;
using UnityEngine;

namespace LF2.Server{

    public class PlayerDDAState : State
    {

        bool m_ExecutionFired;
        float m_MaxDistance = 0.35f;


        public PlayerDDAState(StateMachineServer player) : base(player)
        {
            m_skillsDescription = stateMachine.SkillDescription(StateType.DDA1);
            subStateDetails = m_skillsDescription.DamageDetails;
            InputPackage data = new InputPackage();
            data.StateTypeEnum = StateType.DDA1; 
        
            logicState = stateMachine.MakeState(stateMachine ,ref data )  ;
        }



        public override void Enter()
        {      
            base.Enter();
            logicState.Enter(); 
        }


        public override StateType GetId()
        {
            return StateType.DDA1;
        }


        public override void End()
        {
            stateMachine.ChangeState(StateType.Idle);
        }

      
 




    }
}
