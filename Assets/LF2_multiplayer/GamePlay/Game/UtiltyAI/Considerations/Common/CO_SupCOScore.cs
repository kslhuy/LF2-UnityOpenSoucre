using UnityEngine;
namespace LF2.Client{

    // The condition is a supplement 
    // In this Condition , we have a list of condition  
    // Do the analyse each consideration
    // So if not valide , that will not affect to the total score 
    // But if is valide , so that add a supplement score to the total , 
    // so that we can have more chance to play that Action
    [CreateAssetMenu(fileName = "CO_SupCOScore", menuName = "UtilityAI/Considerations/Special/CO_SupCOScore")]
    
    public class CO_SupCOScore : Consideration
    {
        [SerializeField] Consideration[] SupConsiderations ;
        
        [Tooltip("Do the analyse each consideration , if score less than threshold , that will not affect to the total score ")]        
        [Range(0,1)]
        [SerializeField] float threshold;
        public override float ScoreConsideration(AIBrain brain)
        {           
            float scoreSup = 0;
            for (int c = 0 ; c < SupConsiderations.Length ; c++)
                if (SupConsiderations[c].ScoreConsideration(brain) > threshold ){
                    scoreSup += SupConsiderations[c].ScoreConsideration(brain);
                }
            return scoreSup/SupConsiderations.Length;
        }
    }

}