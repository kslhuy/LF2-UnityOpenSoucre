using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_DistanceZ_InRange", menuName = "UtilityAI/Considerations/CO_Distance_InRange/CO_Distance_Z_InRange")]
    public class CO_DistanceZ_InRange : Consideration
    {
        [SerializeField] int minRang_Z ;
        [SerializeField] int maxRang_Z ;
        [Tooltip("if inverLogic the score will be 0 in the condition normal")]
        [SerializeField] bool invertLogic ;
        
        public override float ScoreConsideration(AIBrain brain)
        {
            if (invertLogic){
                if (brain.rangeAbs(minRang_Z,maxRang_Z,brain.variables.zdistance)) return 0;
                else return 1;    
            }
            else {

                if (brain.rangeAbs(minRang_Z,maxRang_Z,brain.variables.zdistance)) return 1;
                else return 0;
            }

        }
    }

}