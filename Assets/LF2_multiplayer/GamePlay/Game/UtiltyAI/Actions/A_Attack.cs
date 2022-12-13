using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2.Client{
    [CreateAssetMenu(fileName = "A_Attack", menuName = "UtilityAI/Actions/A_Attack")]

    public class A_Attack : StateAction
    {
        public override void Execute(AIBrain npc)
        {
            npc.Do(TypeName);
        }
    }
}
