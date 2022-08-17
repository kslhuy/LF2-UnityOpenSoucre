using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace  LF2.Server{

    public class SlidingState : State
    {
        private float _runSpeed_Var;
        private float _runSpeed_Fix; // cache value
        private float _gainDecreaseRunSpeed;
        public SlidingState(StateMachineServer player) : base(player)
        {

            _runSpeed_Fix = stateMachine.serverplayer.NetState.CharacterClass.running_speed;
            _runSpeed_Var = _runSpeed_Fix;
            _gainDecreaseRunSpeed = 4f;
            
        }


        public override void Enter()
        {
            base.Enter();
            // ko cho nhay lan thu 2 khi Run
            Data.StateTypeEnum = StateType.Sliding;
            
        }

        public override void LogicUpdate()
        {

            _runSpeed_Var -= _gainDecreaseRunSpeed*Time.deltaTime;

            if (_runSpeed_Var < 0f ){
                stateMachine.ChangeState(StateType.Idle);
            }
        }


        public override void Exit()
        {
            base.Exit();
            ResetRunVelocity();
        }

        public void ResetRunVelocity(){
            _runSpeed_Var = _runSpeed_Fix;
        }

        public override StateType GetId(){
            return StateType.Sliding;
        }


    }
}