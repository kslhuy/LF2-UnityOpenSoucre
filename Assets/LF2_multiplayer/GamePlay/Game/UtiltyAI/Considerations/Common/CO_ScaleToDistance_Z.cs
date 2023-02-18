using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_ScaleToDistance_Z", menuName = "UtilityAI/Considerations/CO_ScaleToDistance/CO_ScaleToDistance_Z")]
    public class CO_ScaleToDistance_Z : Consideration
    {
        [SerializeField] AnimationCurve responseCurve ;
        [SerializeField] int maxRang ;
        [SerializeField] int minRang ;
        
        public override float ScoreConsideration(AIBrain brain)
        {
            var zdistanceAbs = Mathf.Abs(brain.variables.zdistance);
            return Mathf.Clamp01(responseCurve.Evaluate((int)zdistanceAbs));
            
        }
    }

}