using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{
    [CreateAssetMenu(fileName = "A_Walk", menuName = "UtilityAI/Actions/A_Walk")]
    public class A_Walk : StateAction
    {
        public override void Execute(AIBrain npc)
        {
            Vector2 dir = new Vector2(npc.variables.xdistance , npc.variables.zdistance).normalized;
            dir = npc.Self.coreMovement.SenseCheck(dir);
            // Debug.Log( dir); 
            npc.ActionMoveInputEvent?.Invoke(dir.x , dir.y);        
            

            
        
        }

        
    }
}
