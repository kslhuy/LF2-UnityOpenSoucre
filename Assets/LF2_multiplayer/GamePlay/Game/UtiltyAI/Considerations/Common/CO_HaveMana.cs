using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_HaveMana", menuName = "UtilityAI/Considerations/CO_HaveMana")]
    public class CO_HaveMana : Consideration
    {
        [SerializeField] AnimationCurve responseCurve ;
        [Tooltip("True if MPPoints > ManaNeeded")]
        [SerializeField] bool BooleenCondition ;
        [SerializeField] int ManaNeeded;
        // [SerializeField] bool invertLogic ;
        
        public override float ScoreConsideration(AIBrain brain)
        {
            if (BooleenCondition && brain.Self.m_NetState.MPPoints > ManaNeeded ){
                return Score = 1;
            }
            Score = responseCurve.Evaluate((int)brain.Self.m_NetState.MPPoints);
            return Score;
        }
    }

}