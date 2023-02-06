using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_IsSeftState", menuName = "UtilityAI/Considerations/CO_IsSeftState")]
    public class CO_IsSeftState : Consideration
    {
        public StateType[] StateDesir;
        public bool isDiffThisState ;  
        public override float ScoreConsideration(AIBrain brain)
        {
            if (isDiffThisState){
                // 1 cai = stateDesir là Not ok 
                foreach (StateType stateType in StateDesir){
                    if (brain.Self.MStateMachinePlayerViz.CurrentStateViz.GetId() == stateType){
                        return 0;
                    } 
                }
                // // ko 1 cai nao' = stateDesir là  ok
                return 1;

            }else{
                foreach (StateType stateType in StateDesir){
                    // 1 cai bang stateDesir là ok 
                    if (brain.Self.MStateMachinePlayerViz.CurrentStateViz.GetId() == stateType){
                        return 1;
                    } 
                }
                // // ko 1 cai nao' bang stateDesir là Not ok 
                return 0;
                
            }

        }
    }

}