using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{
    [CreateAssetMenu(fileName = "A_", menuName = "UtilityAI/Actions/A_General_Perform")]
    public class A_General : StateAction
    {
        public override void Execute(AIBrain npc)
        {
            npc.PerformStateDirect(TypeName);
        }

    }
}
