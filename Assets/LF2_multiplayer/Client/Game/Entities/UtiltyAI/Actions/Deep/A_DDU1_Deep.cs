using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Client{
    [CreateAssetMenu(fileName = "A_DUJ1_Deep", menuName = "UtilityAI/Actions/Deep/A_DUJ1")]

    public class A_DDU1_Deep : StateAction
    {
        public override void Execute(AIBrain npc)
        {
            npc.Do(StateType.DDJ1);
        }
    }
}
