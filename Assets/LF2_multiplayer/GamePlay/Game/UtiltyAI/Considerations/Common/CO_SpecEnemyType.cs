using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "CO_SUP_SpecEnemyType", menuName = "UtilityAI/Considerations/Special/CO_SUP_SpecEnemyType")]
    public class CO_SpecEnemyType : CO_SupCOScore
    {
        [SerializeField] CharacterTypeEnum co_for_Enemy;
        public override float ScoreConsideration(AIBrain brain)
        {
            if (brain.TargetEnemy.m_NetState.CharacterType != co_for_Enemy) return 0;
            return base.ScoreConsideration(brain);

        }
    }

}