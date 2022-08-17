using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{
    [CreateAssetMenu(fileName = "A_Run", menuName = "UtilityAI/Actions/A_Run")]
    public class A_Run : StateAction
    {
        public override void Execute(AIBrain npc)
        {
            npc.Do(StateType.Run);
        }

    }
}
