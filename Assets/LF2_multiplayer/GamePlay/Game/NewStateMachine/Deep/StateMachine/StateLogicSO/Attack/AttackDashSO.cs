using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "AttackDashSO", menuName = "StateLogic/Deep/AttackDash")]
    public class AttackDashSO : StateLogicSO<AttackDashLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }



}
