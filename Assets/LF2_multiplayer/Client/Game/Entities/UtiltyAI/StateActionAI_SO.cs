using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//behavior NPC can do = State 
namespace LF2.Client{
    [CreateAssetMenu(fileName = "New_StateActionAI_SO", menuName = "UtilityAI/StateActionAI_SO")]

    public class StateActionAI_SO : ScriptableObject{
        // public StateAction[] A_AttackAvaillable;
        // public StateAction[] A_MoveAvaillable; 
        // StateType[] values = null;
        public CharacterTypeEnum CharacterType;
        public StateActionDebug[] A_AttackAvaillable;
        public StateActionDebug[] A_MoveAvaillable;

        // public List<StateActionDebug> A_MoveAvaillables;

    }

    [Serializable]
    public struct StateActionDebug{
        public StateAction A_Availlable;
        [Tooltip("Disable this action")]
        public bool Disable; 
        [Tooltip("Show score this action")]
        public bool ShowScore;
    }

    
}

