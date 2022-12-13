using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_DistanceX_InRange", menuName = "UtilityAI/Considerations/CO_Distance_InRange/CO_Distance_X_InRange")]
    public class CO_DistanceX_InRange : Consideration
    {
        [SerializeField] int minRang ;
        [SerializeField] int maxRang ;
        [SerializeField] bool invertLogic ;
        
        public override float ScoreConsideration(AIBrain brain)
        {
            if (invertLogic){
                if (brain.range(minRang,maxRang,brain.variables.xdir_distane)){
                return 0;
                }
                else return 1;    
            }
            if (brain.range(minRang,maxRang,brain.variables.xdir_distane)){
                return 1;
            }
            else return 0;

        }
    }

}