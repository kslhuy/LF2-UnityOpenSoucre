using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Sliding", menuName = "StateLogic/Common/Sliding")]
    public class SlidingSO : StateLogicSO<SlidingLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class SlidingLogic : StateActionLogic
    {
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

        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Sliding);  
            if (stateData.Sounds) stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(StateType.Sliding);
            }
            PlayAnim(nbanim , sequence);
        }


        public override void LogicUpdate()
        {
            if (stateMachineFX.m_ClientVisual.Owner) stateMachineFX.m_ClientVisual.coreMovement.SetSliding(stateData.Dx);
        }


        

        public override void End()
        {
            stateMachineFX.AnticipateState(StateType.Idle);

        }

        public override StateType GetId(){
            return StateType.Sliding;
        }


    }

}
