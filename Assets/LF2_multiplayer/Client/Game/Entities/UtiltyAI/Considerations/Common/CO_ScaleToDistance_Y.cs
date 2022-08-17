using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_ScaleToDistance_Y", menuName = "UtilityAI/Considerations/CO_ScaleToDistance/CO_ScaleToDistance_Y")]
    public class CO_ScaleToDistance_Y : Consideration
    {

        [SerializeField] AnimationCurve responseCurve ;
        [SerializeField] int maxRang ;
        [SerializeField] int minRang ;
        
        public override float ScoreConsideration(AIBrain brain)
        {
            var ydistance = brain.variables.ydistance;
            if (brain.rangeAbs(minRang , maxRang , ydistance)) return Mathf.Clamp01(responseCurve.Evaluate((int)ydistance));
            return 0;
        }
    }

}