using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LF2.Server{
    //In this State :  Player Jump in to air  , can change to desied State follow some request. 
    //                 Do a jump physics , and check every frame  player touch Ground
    public class PlayerJumpState : PlayerAirState
    {

        public PlayerJumpState(StateMachineServer player) : base(player)
        {
        }

        public override bool CanChangeState(InputPackage actionRequestData)
        {
            
            if (actionRequestData.StateTypeEnum == StateType.Attack ){
                stateMachine.ChangeState(StateType.AttackJump);
                return true;
            }
            return false;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void LogicUpdate(){

            base.LogicUpdate();
        }

        

        public override StateType GetId()
        {
            return StateType.Jump;
        }

        

    }
}