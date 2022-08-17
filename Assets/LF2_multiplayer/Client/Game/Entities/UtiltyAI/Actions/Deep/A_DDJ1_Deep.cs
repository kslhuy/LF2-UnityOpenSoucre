using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Client{
    [CreateAssetMenu(fileName = "A_DDJ1_Deep", menuName = "UtilityAI/Actions/Deep/A_DDJ1")]

    public class A_DDJ1_Deep : StateAction
    {
        public override void Execute(AIBrain npc)
        {
            npc.Do(StateType.DDJ1);
        }
    }
}
