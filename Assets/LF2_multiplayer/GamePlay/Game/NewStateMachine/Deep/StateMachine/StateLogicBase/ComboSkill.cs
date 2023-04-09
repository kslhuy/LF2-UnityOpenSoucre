using System.Collections.Generic;
using UnityEngine;
using LF2.Client;
using System;

namespace LF2.Data{

    [CreateAssetMenu(fileName = "ComboSkill", menuName = "State Machines/ComboSkill")]
    public class ComboSkill : ScriptableObject {
        public string nameSkills;
        public string Descrtion;

        public bool Own;
        public int Price;
        public StateLogicSO[] StateLogicSO;

 
    }
    

    [Serializable]
    public class StateLogicSOAddInfo{
        public StateType RunTimeStateType;
        // public StateLogicSO StateLogicSO;

        public StateType InputToConvert; // Attack , defense , jump (may be DDA , DDJ ..)

        // public StateType NextRunTimeStateType;
    }
}

