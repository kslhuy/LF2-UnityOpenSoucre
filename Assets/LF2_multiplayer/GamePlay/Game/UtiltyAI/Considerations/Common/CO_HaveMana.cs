using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_HaveMana", menuName = "UtilityAI/Considerations/CO_HaveMana")]
    public class CO_HaveMana : Consideration
    {
        [Tooltip("Not use Boolean , use cuvre")]
        [SerializeField] AnimationCurve responseCurve ;
        [Tooltip("True if MPPoints > ManaNeeded")]
        [SerializeField] bool BooleenCondition ;
        [SerializeField] int ManaNeeded;
        // [SerializeField] bool invertLogic ;
        
        public override float ScoreConsideration(AIBrain brain)
        {
            
            if ( brain.Self.m_NetState.MPPoints < ManaNeeded ){
                return 0;
            }
            // Here we have enough mana to perform the action  
            if (BooleenCondition){
                return 1 ;    
            }else{
                return responseCurve.Evaluate((int)brain.Self.m_NetState.MPPoints);  
            } 
        }
    }

}