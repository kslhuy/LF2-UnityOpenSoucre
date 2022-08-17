using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DefenseHit", menuName = "StateLogic/Common/Hurt/DefenseHit")]
    public class DefenseHitSO : StateLogicSO<DefenseHitLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class DefenseHitLogic : StateActionLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override void Enter()
        {
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
        }


        public override StateType GetId()
        {
            return StateType.DefenseHit;
        }


        public override void End(){
            stateMachineFX.idle();
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play("Empty_anim");
            stateMachineFX.m_ClientVisual.InjuryAnimator.Play(stateData.vizAnim[nbanim-1].AnimHashId);
        }
    }

}
