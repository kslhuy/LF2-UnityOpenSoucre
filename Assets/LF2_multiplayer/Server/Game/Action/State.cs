using System;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Server
{
    /// <summary>
    /// The abstract parent class that all State derive from.
    /// </summary>
    /// <remarks>
    /// The State System is a generalized mechanism for Characters to "do stuff" in a networked way. State
    /// include everything from your basic character attack, to a fancy skill  Shot, 

    /// For every StateType enum, there will be one specialization of this class.

    ///
    /// The flow for State is:
    /// Initially: Enter()
    /// Every frame:   LogicUpdate() + PhysicUpdate() (can be 1 of 2)
    /// On shutdown: End() (end this State Naturelly) or Exit() (be interrupted by some logic (force to change State))
    /// After End(): Almost time will Switch to Idle .  
    ///

    // / Note also that if Start() returns false, no other functions are called on the Action, not even End().
    /// </remarks>
    public abstract class State 
    {
        protected StateMachineServer stateMachine;

        public InputPackage Data = new InputPackage();

        // Use for State have serveral different Logic
        protected State logicState;

        protected StateType m_stateType;
        protected SkillsDescription m_skillsDescription;
        protected DamageDetails[] subStateDetails;

        
        public float TimeStarted_Server { get ; protected set;}
        protected static ulong OurNetWorkID ;


        protected State(StateMachineServer stateMachine)
        {
            
            this.stateMachine = stateMachine;
        }


        // Get the StateType of current State  
        public abstract StateType GetId();
        public virtual void Enter(){
            TimeStarted_Server = Time.time;
        }
        public virtual void LogicUpdate() {
        }


   

        public virtual void Exit(){ }

        // If we have a request so check if we can change to desired state 
        // NOTE :   (Only 3 basic State can check Attack , Jump , Defense)
        public virtual bool  CanChangeState(InputPackage actionRequestData){
            return false;
        }



        public virtual void End()
        {
            stateMachine.ChangeState(StateType.Idle);
        }

    }
}