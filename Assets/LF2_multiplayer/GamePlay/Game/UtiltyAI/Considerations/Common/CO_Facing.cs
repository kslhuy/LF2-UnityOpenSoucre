using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_Facing", menuName = "UtilityAI/Considerations/CO_Facing")]
    public class CO_Facing : Consideration
    {
        [SerializeField] bool facingAgainst ;
        [SerializeField] bool facingTowards ;

        public override float ScoreConsideration(AIBrain brain)
        {
            if (facingAgainst) {
                if (brain.facing_against() ){
                    return 1;
                }
                else return 0;
            }
            if (facingTowards){
                if (brain.facing_towards() ){
                    return 1;
                }
                else return 0;
            }
            return 0;

        }
    }

}