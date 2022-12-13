using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_HaveHP", menuName = "UtilityAI/Considerations/CO_HaveHP")]
    public class CO_HaveHP : Consideration
    {
        [SerializeField] AnimationCurve responseCurve ;
        [Tooltip("current HP < HP condition => True")]
        [SerializeField] bool BooleenCondition ;
        [SerializeField] int Hp_condition;
        // [SerializeField] bool invertLogic ;
        
        public override float ScoreConsideration(AIBrain brain)
        {
            if (BooleenCondition && brain.Self.m_NetState.HPPoints < Hp_condition ){
                return Score = 1;
            }
            Score = responseCurve.Evaluate((int)brain.Self.m_NetState.HPPoints);
            return Score;
        }
    }

}