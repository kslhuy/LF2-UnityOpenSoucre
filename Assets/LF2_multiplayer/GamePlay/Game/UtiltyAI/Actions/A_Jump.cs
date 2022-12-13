using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{
    [CreateAssetMenu(fileName = "A_Jump", menuName = "UtilityAI/Actions/A_Jump")]
    public class A_Jump : StateAction
    {
        public override void Execute(AIBrain npc)
        {
            Vector2 dir = new Vector2(npc.variables.xdistance , npc.variables.zdistance).normalized;
            // Debug.Log( dir); 
            npc.ActionMoveInputEvent?.Invoke(dir.x , dir.y);     
            npc.Do(StateType.Jump);
        }

    }
}
