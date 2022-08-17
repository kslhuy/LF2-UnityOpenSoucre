using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "AttackRun", menuName = "StateLogic/Deep/AttackRun")]
    public class AttackRunSO : StateLogicSO<AttackRunLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }



}
