using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LF2.Server{

    public class PlayerMoveState : State
    {


        public PlayerMoveState(StateMachineServer player) : base(player)
        {
        }
        
        public override bool CanChangeState(InputPackage actionRequestData)
        {
            if (actionRequestData.StateTypeEnum == StateType.Jump){
                // SetJump here because Vector moveDir is available ;
                // If we move , so wanna Jump , Jump to that direction 
                stateMachine.ChangeState(StateType.Jump);
                return true;
            }
            else if(actionRequestData.StateTypeEnum == StateType.Attack 
                    || actionRequestData.StateTypeEnum == StateType.Defense                 
                    ||actionRequestData.StateTypeEnum == StateType.Run
                    ||actionRequestData.StateTypeEnum == StateType.Idle){

                    stateMachine.ChangeState(actionRequestData.StateTypeEnum);
                    return true;
                    }              
            else if (actionRequestData.StateTypeEnum == StateType.Move){
                return true;
            }
            return false;
        
        }

        public override StateType GetId()
        {
            return StateType.Move;
        }

        public override void Enter(){
            base.Enter();
        }

        public override void LogicUpdate(){

        }

    }
}
