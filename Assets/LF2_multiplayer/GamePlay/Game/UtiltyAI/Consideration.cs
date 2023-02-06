using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//behavior NPC can do = State 
namespace LF2.Client{
    public abstract class Consideration : ScriptableObject{
        public string Owner;
        [Tooltip("Add score of this Consideration direct to total score of the action , See more math calulation in AIBrain")]
        public bool isSubConsideration;
        
        private float _score;



        public float Score {
            get { return _score;}
            set {
                this._score = Mathf.Clamp01(value);
            }
        }


        private void Awake() {
            _score = 0;    
        }

        public abstract float ScoreConsideration(AIBrain brain);



        
    }
}

