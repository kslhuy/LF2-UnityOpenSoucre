using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Server{


    public class PlayerAttackJump1 : PlayerAirState
    {
        public PlayerAttackJump1(StateMachineServer player) : base(player)
        {
        }



        public override void Enter()
        {
            base.Enter();
  
            Data.StateTypeEnum = StateType.AttackJump;
            
        }



        public override StateType GetId()
        {
            return StateType.AttackJump;
        }
    }


}
