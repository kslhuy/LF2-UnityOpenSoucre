using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace LF2.Server{
    //in this State :  Player Actack  , can change to desied State follow some request. 
    //                 It is not explicitly targeted (so can attack to all time ), but rather detects the foe (enemy ) that was hit with a physics check.
    public class AttackState1 : State
    {
        // bool m_ExecutionFired;
        // float m_MaxDistance = 0.35f;

        private List<ulong> m_ProvisionalTarget ;

        // private int nbHit; 

        public AttackState1(StateMachineServer playerStateMachine) : base(playerStateMachine)
        {          
            m_skillsDescription = stateMachine.SkillDescription(GetId());
            subStateDetails = m_skillsDescription.DamageDetails;
            InputPackage data = new InputPackage();
            data.StateTypeEnum = GetId(); 
        
            logicState = stateMachine.MakeState(playerStateMachine ,ref data )  ;


        }



        public override void Enter()
        {      
            // Debug.Log($"logicState : {logicState}" );
            logicState.Enter(); 
            TimeStarted_Server = Time.time;            
        }


        public override StateType GetId()
        {
            return StateType.Attack;
        }

        public override void LogicUpdate()
        {
            logicState.LogicUpdate();
        }


        public override void End()
        {
            // logicState.End();
            base.End();
        }

        public override void Exit()
        {
            logicState.Exit();
        }
    }
    
}
