using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_RandomeScore", menuName = "UtilityAI/Considerations/Special/CO_RandomeScore")]
    
    public class CO_RandomeScore : Consideration
    {
        
        // [SerializeField] AnimationCurve responseCurve ;
        [Tooltip("bigger value , high chance to play onwer Action")]
        [Range(0,1f)]
        [SerializeField] float PointFrequencyAddition;
        
        public override float ScoreConsideration(AIBrain brain)
        {            
            return Mathf.Clamp01(Random.value+PointFrequencyAddition);
        }
    }

}