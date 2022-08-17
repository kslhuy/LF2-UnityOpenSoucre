using UnityEngine;
namespace LF2.Client{

    // So the condition is a suplement 
    // So if not valide , that will not affect to the total score 
    // But if is valide , so that add a suplement score to the total , so that we can have more chance to play the state
    [CreateAssetMenu(fileName = "CO_SupCOScore", menuName = "UtilityAI/Considerations/Special/CO_SupCOScore")]
    
    public class CO_SupCOScore : Consideration
    {
        [SerializeField] Consideration[] SupConsiderations ;
        float scoreSup;
        
        public override float ScoreConsideration(AIBrain brain)
        {           
            scoreSup = 0;
            for (int c = 0 ; c < SupConsiderations.Length ; c++)
                if (SupConsiderations[c].ScoreConsideration(brain) > 0 ){
                    scoreSup += SupConsiderations[c].ScoreConsideration(brain);
                }
            return scoreSup/(scoreSup + 1);
        }
    }

}