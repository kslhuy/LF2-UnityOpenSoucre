using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Server{
    
    public class StateMachineServer : stateMachineBase
    {
        public ServerCharacter serverplayer;
        // public ServerCharacterMovement ServerCharacterMovement;

        private SkillsDescription skillsDescription;

        // 
        public List<State> QueueState ;
        public State[] states = new State[Enum.GetNames(typeof(StateType)).Length];
        public State CurrentState{ get ; private set;}
        private State m_lastState;

        
        public float inputX;
        public float inputZ;

        // Variables for movement
        // public float input{ get; private set; }
        public bool IsMove { get; private set; }


        /// 
        public int nbHit;

        // 2 Last State For Client Correction
        private State[] LastStateServer = new State[2];



    

        public StateMachineServer(ServerCharacter serverplayer
                                ,CharacterTypeEnum characterType) : base(characterType)
        {
            this.serverplayer = serverplayer;
            // ServerCharacterMovement = serverplayer.Movement;

            QueueState = new List<State>();

            RegisterState(new PlayerIdleState(this ));
            RegisterState(new PlayerMoveState(this ));
            RegisterState(new PlayerRunState(this));
            RegisterState(new SlidingState(this));
            RegisterState(new PlayerRollingState(this));

            RegisterState(new PlayerJumpState(this ));
            RegisterState(new PlayerDoubleJumpState(this ));
            

            RegisterState(new PlayerLandState(this ));

            RegisterState(new AttackState1(this ));
            RegisterState(new AttackState2(this ));
            RegisterState(new AttackState3(this ));

            RegisterState(new PlayerAttackJump1(this ));
            RegisterState(new AttackRun(this ));

            RegisterState(new PlayerDefenseState(this));

            RegisterState(new PlayerHurtState(this));
            RegisterState(new PlayerFallState(this));
            RegisterState(new PlayerRecoveryState(this));


            RegisterStateSpecial(characterType);




            CurrentState = GetState(StateType.Idle);

            LastStateServer[0] = CurrentState;
            LastStateServer[1] = CurrentState; 
    }

    private void RegisterStateSpecial(CharacterTypeEnum characterType)
    {
        switch (characterType) {
            case CharacterTypeEnum.Deep : 
                // RegisterState(new DeepDDA(this));
                // RegisterState(new DeepDDA2(this));
                // RegisterState(new DeepDDA3(this));

                // RegisterState(new DeepDUA(this));
                // RegisterState(new DeepDUA2(this));
                // RegisterState(new DeepDUA3(this));


                RegisterState(new PlayerDDJState(this));
                RegisterState(new PlayerDUJState(this));
                break;
            case CharacterTypeEnum.John : 
                RegisterState(new PlayerDDAState(this));
                RegisterState(new PlayerDDJState(this));
                RegisterState(new PlayerDUAState(this));
                RegisterState(new PlayerDUJState(this));
                break;
            default :

                RegisterState(new PlayerDDAState(this));
                RegisterState(new PlayerDDJState(this));
                RegisterState(new PlayerDUAState(this));
                RegisterState(new PlayerDUJState(this));
                break;
        } 

        
    }



        // Client send input to Server to change the state of player
        public void RequestToState(ref InputPackage requestData)
        {

            if (RequestChangeState(requestData)) {
                GetState(requestData.StateTypeEnum).Data = requestData;
            }
        }



        // RegisterState , instantiated in the first time 
        public void RegisterState(State state){
            int index = (int)state.GetId();
            states[index] = state;
        }

        // Do convert enum StateType == > State corresponse 
        public State GetState (StateType stateType){
            int index = (int)stateType;
            return states[index];
        }

        public void Update() {
            // Debug.Log(CurrentState);
            if (CurrentState.GetId() == StateType.Idle) {
                            // Priori ComboState Play first   
                if (QueueState.Count>0) {
                    ChangeState(QueueState[0].GetId() ,true);
                    QueueState.RemoveAt(0);
                    return;
                } 

                return;
            }

            if (CurrentState.GetId() == StateType.Move){
                m_lastState = CurrentState;
                CurrentState.LogicUpdate();
                return;
            }

            if ( m_lastState != CurrentState){
                m_lastState = CurrentState;
                if (CurrentState.GetId() == StateType.Idle || CurrentState.GetId() == StateType.Move ) return;
                skillsDescription =  SkillDescription(CurrentState.GetId()); // Get All Skills Data of actual Player Charater we current play.
            } 

            if (skillsDescription!= null){
                if (skillsDescription.expirable)
                {
                    // Debug.Log($"Sub_TimeAnimation = {Time.time -  CurrentState.TimeStarted_Animation} "); 
                    bool timeExpired = Time.time -  CurrentState.TimeStarted_Server >= skillsDescription.DurationSeconds;

                    // Check if this State Can End Naturally (== time Expired )
                    if ( timeExpired ){
                        CurrentState?.End();
                        return;
                    }
                }
            }
            CurrentState.LogicUpdate();

        }


        public void ChangeState(StateType newState , bool queue = false ){
            CurrentState.Exit();
            CurrentState = queue? QueueState[0] : GetState(newState);
            CurrentState.Enter();
        }


        // Note Only 3 basics StateType (Attack , Jump , Defense )  and Skills (DDA , DDJ ...) can be checked 
        public bool RequestChangeState(InputPackage data){

            return CurrentState.CanChangeState(data);
        }

        // TO DO : NEED To change logic 
        /// <summary>
        /// Tells all active Actions that a particular gameplay event happened, such as being hit,
        /// getting healed, dying, etc. Actions can change their behavior as a result.
        /// </summary>
        public void OnGameplayActivity(InputPackage stateRequestData)
        {
            GetState(stateRequestData.StateTypeEnum).Data = stateRequestData;
            ChangeState(stateRequestData.StateTypeEnum);
        }

        // TO DO : So we use StateLogic instead of the State , 
        // So we can simply call ComboState for all DUAState , DDAState , etc ... 

        public State MakeState(StateMachineServer stateMachine ,ref InputPackage data)
        {
            StateLogic logic = SkillDescription(data.StateTypeEnum).Logic;
            switch (logic)
            {
                // case StateLogic.LaunchProjectile: return new LaunchProjectile( stateMachine , ref data);
                // // case StateLogic.Melee: return new Melee(stateMachine , ref data);
                // case StateLogic.CircularUpAttack :return new CircularUpAttack(stateMachine , ref data);
                // case StateLogic.LinearAOE : return new LinearAoE(stateMachine , ref data);


                default: throw new System.Exception($"Dont have class Logic for {logic}");

            }
        }

    }
    
}
