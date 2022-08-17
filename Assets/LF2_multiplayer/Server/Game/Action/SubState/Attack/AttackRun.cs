using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Server{


    public class AttackRun : State
    {
        public AttackRun(StateMachineServer player) : base(player)
        {
        }



        public override void Enter()
        {
            base.Enter();
            Data.StateTypeEnum = StateType.AttackRun;
            
;
        }


        public override StateType GetId()
        {
            return StateType.AttackRun;
        }
    }


}
