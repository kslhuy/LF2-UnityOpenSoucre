using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_DistanceZ", menuName = "UtilityAI/Considerations/CO_Distance/CO_Distance_Z")]
    public class CO_DistanceZ : Consideration
    {

        public int comparator;
        [SerializeField] bool IsSuperior;
        public override float ScoreConsideration(AIBrain brain)
        {
            if (IsSuperior){
                if (brain.variables.zdistanceAbs > comparator) return 1;
                else return 0;    
            }
            else {
                if (brain.variables.zdistanceAbs < comparator) return 1;
                else return 0;    
            }
        }
    }

}