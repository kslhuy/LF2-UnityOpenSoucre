using UnityEngine;
namespace LF2.Client{


[CreateAssetMenu(fileName = "CO_DDA_Deep", menuName = "UtilityAI/Considerations/Deep/CO_DDA")]
    public class CO_DDA_Deep : Consideration
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