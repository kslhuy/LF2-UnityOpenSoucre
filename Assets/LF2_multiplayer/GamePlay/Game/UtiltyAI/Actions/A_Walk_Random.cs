using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{
    [CreateAssetMenu(fileName = "A_Walk_Random", menuName = "UtilityAI/Actions/A_Walk_Random")]
    public class A_Walk_Random : StateAction
    {
        public override void Execute(AIBrain npc)
        {
            // Vector2 dir = new Vector2(npc.variables.xdistance , npc.variables.zdistance).normalized;
            // Debug.Log( dir); 
            npc.WalkRandomly();        
        }
    }
}
