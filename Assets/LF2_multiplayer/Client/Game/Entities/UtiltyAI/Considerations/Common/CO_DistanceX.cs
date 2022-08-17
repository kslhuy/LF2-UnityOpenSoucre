using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_DistanceX", menuName = "UtilityAI/Considerations/CO_Distance/CO_Distance_X")]
    public class CO_DistanceX : Consideration
    {
        public int comparator;
        [SerializeField] bool IsSuperior;
        public override float ScoreConsideration(AIBrain brain)
        {
            if (IsSuperior){
                if (brain.variables.xdir_distane > comparator) return 1;
                else return 0;    
            }
            else {
                if (brain.variables.xdir_distane < comparator) return 1;
                else return 0;    
            }
        }
    }

}