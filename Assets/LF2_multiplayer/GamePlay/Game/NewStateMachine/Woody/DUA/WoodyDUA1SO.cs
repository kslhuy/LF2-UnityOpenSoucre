using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "WoodyDUA1", menuName = "StateLogic/Woody/Special/DUA1")]
    public class WoodyDUA1SO : StateLogicSO<WoodyDUA1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
    //// cleg 
    public class WoodyDUA1Logic : StateActionLogic
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

        public override void End(){

            stateMachineFX.idle();
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUA_1);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);

        }

        public override void LogicUpdate()
        {
            stateMachineFX.CoreMovement.CustomMove_InputX(stateData.Dx);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }

            PlayAnim(nbanim , sequence);
        }
        public override StateType GetId()
        {
            return stateData.StateType;
        }

        public override void OnAnimEvent(int id)
        {
            if (id == 1) {
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
                return;}
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }



    }

}
