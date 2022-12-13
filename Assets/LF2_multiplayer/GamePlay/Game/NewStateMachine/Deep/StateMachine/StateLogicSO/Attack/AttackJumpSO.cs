using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "AttackJump", menuName = "StateLogic/Deep/AttackJump")]
    public class AttackJumpSO : StateLogicSO<AttackJumpLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }


}
