using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Client{
    [CreateAssetMenu(fileName = "A_DDA1_Deep", menuName = "UtilityAI/Actions/Deep/A_DDA1")]

    public class A_DDA1_Deep : StateAction
    {
        public override void Execute(AIBrain npc)
        {
            npc.Do(StateType.DDA1);
        }
    }
}
