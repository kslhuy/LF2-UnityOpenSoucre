using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//behavior NPC can do = State 
namespace LF2.Client{
    public abstract class StateAction : ScriptableObject{
        public StateType TypeName;

        private float _score;

        public float Score {
            get { return _score;}
            set {
                this._score = Mathf.Clamp01(value);
            }
        }

        public Consideration[] considerations;

        private void Awake() {
            _score = 0;    
        }

        public abstract void Execute(AIBrain npc);

        public virtual void SetRequiredDestination(AIBrain npc) { }


        
    }
}

