using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_ScaleToDistance_X", menuName = "UtilityAI/Considerations/CO_ScaleToDistance/CO_ScaleToDistance_X")]
    public class CO_ScaleToDistance_X : Consideration
    {

        [SerializeField] AnimationCurve responseCurve ;
        [SerializeField] int maxRang ;
        [SerializeField] int minRang ;
        
        public override float ScoreConsideration(AIBrain brain)
        {
            var xdistanceAbs = Mathf.Abs(brain.variables.xdistance);
            if (brain.range(minRang , maxRang , xdistanceAbs)) return Mathf.Clamp01(responseCurve.Evaluate((int)(xdistanceAbs)));
            return 0;
        }
    }

}