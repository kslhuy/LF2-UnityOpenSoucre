using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{
    [CreateAssetMenu(fileName = "A_", menuName = "UtilityAI/Actions/A_General_SendInput")]
    public class A_General_SendInput : StateAction
    {
        public override void Execute(AIBrain npc)
        {
            npc.Do(TypeName);
        }

    }
}
