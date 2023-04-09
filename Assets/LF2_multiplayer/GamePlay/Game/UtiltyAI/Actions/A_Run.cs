using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{
    [CreateAssetMenu(fileName = "A_Run", menuName = "UtilityAI/Actions/A_Run")]
    public class A_Run : StateAction
    {
        public override void Execute(AIBrain npc)
        {
            if (npc.Self.GetStateType() != StateType.Run){
                npc.Do(StateType.Run);
            }
            Vector2 dir = new Vector2(npc.variables.xdistance , npc.variables.zdistance).normalized;
            dir = npc.Self.coreMovement.SenseCheck(dir);
            // Debug.Log( dir); 
            npc.ActionMoveInputEvent?.Invoke(dir.x , dir.y); 
            
        }

    }
}
