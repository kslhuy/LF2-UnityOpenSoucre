using System.Collections.Generic;
using UnityEngine;
using LF2.Client;
using System;

namespace LF2.Data{

    [CreateAssetMenu(fileName = "ComboSkill", menuName = "State Machines/ComboSkill")]
    public class ComboSkill : ScriptableObject {
        public string nameSkills;
        public StateType StateType;
        public string Descrtion;

        public bool Own;
        public int PriceCoin;
        public int PriceDiamand;

        public StateLogicSO[] StateLogicSOs;

 
    }
    

    


    // [Serializable]
    // public class StateLogicSOInfoRuntime{
    //     public StateLogicSO
    //     public StateType RunTimeStatePlay;
    //     // public StateLogicSO StateLogicSO;

    //     // public StateType InputToConvert; // Attack , defense , jump (may be DDA , DDJ ..)

    //     // public StateType NextRunTimeStateType;
    // }
}

