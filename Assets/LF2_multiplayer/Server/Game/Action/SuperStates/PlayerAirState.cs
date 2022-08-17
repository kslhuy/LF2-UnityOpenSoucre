using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Server{
    //In this State :  Player Jump in to air  , can change to desied State follow some request. 
    //                 Do a jump physics , and check every frame  stateMachine touch Ground
    public class PlayerAirState : State
    {
        public float AirStateTimeStarted { get; private set; }

        public PlayerAirState(StateMachineServer stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            AirStateTimeStarted = Time.time;
        }

        public override void LogicUpdate() {

            // Add some gravity for stateMachine
            // // Check play touched ground ?? 
            // Debug.Log($" Air_Server = {Time.time - AirStateTimeStarted}"); 
            // stateMachine.ServerCharacterMovement.SetFallingDown();

            // if (stateMachine.ServerCharacterMovement.IsGounded() && Time.time - AirStateTimeStarted > 0.2f ){
            //     stateMachine.ChangeState(StateType.Land);
            // }
        }

        public override StateType GetId()
        {
            return StateType.Air;
        }


    }
}