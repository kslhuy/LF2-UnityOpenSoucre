using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_IsEnemyState", menuName = "UtilityAI/Considerations/CO_IsEnemyState")]
    public class CO_IsEnemyState : Consideration
    {
        public StateType[] StateDesir;
        public bool isDiffThisState ;  
        public override float ScoreConsideration(AIBrain brain)
        {
            if ( !isDiffThisState ){
                foreach (StateType stateType in StateDesir){
                    // 1 cai bang stateDesir là ok 
                    if (brain.TargetEnemy.MStateMachinePlayerViz.CurrentStateViz.GetId() == stateType){
                        return 1;
                    } 
                }
                // // ko 1 cai nao' bang stateDesir là Not ok 
                return 0;
            }else{
                // 1 cai bang stateDesir là Not ok 
                foreach (StateType stateType in StateDesir){
                    if (brain.TargetEnemy.MStateMachinePlayerViz.CurrentStateViz.GetId() == stateType){
                        return 0;
                    } 
                }
                // // ko 1 cai nao' bang stateDesir là  ok
                return 1;
                
            }
        }
    }

}