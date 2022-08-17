using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Server{

    //In this State :  Player Stand Still  , do nothing , wait to request. 
    //                  Can do some initilize (for Exemple reset number Jump)
    public class PlayerIdleState : State
    {
        public PlayerIdleState(StateMachineServer player) : base(player)
        {
            // m_skillsDescription = stateMachine.SkillDescription(GetId());

        }

        public override bool CanChangeState(InputPackage requestData)
        {

            if (requestData.StateTypeEnum == StateType.Jump){
                // stateMachine.ServerCharacterMovement.SetJump(0,0,5,0);
                stateMachine.ChangeState(StateType.Jump);
            }
            // Debug.Log($"Nbanimation = {requestData.NbAnimation}" );


            else if ( requestData.StateTypeEnum == StateType.Attack ){
                if (stateMachine.nbHit == 3 ) {
                    stateMachine.ChangeState(StateType.Attack3);
                    stateMachine.nbHit = 0;
                    }
                else if (requestData.NbAnimation == 1) stateMachine.ChangeState(StateType.Attack);
                else if (requestData.NbAnimation == 2) stateMachine.ChangeState(StateType.Attack2);
            }else{
                stateMachine.ChangeState(requestData.StateTypeEnum);
            }
            return true;

        }



        public override void Enter()
        {
            // stateMachine.ServerCharacterMovement.SetNetworkStatePackage();

            Data.StateTypeEnum = StateType.Idle;
            
            // stateMachine.ServerCharacterMovement.SetNetworkStatePackage(StateType.Idle);

        }


        public override StateType GetId()
        {
            return StateType.Idle;
        }



    }
}

