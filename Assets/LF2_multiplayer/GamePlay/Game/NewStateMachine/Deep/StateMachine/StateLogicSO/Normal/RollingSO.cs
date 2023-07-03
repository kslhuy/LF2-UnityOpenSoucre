using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Rolling", menuName = "StateLogic/Common/Rolling")]
    public class RollingSO : StateLogicSO<RollingLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class RollingLogic : StateActionLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

       public override bool ShouldAnticipate(ref StateType data)
        {            
                   
            if (data == StateType.DDA1 ||
                data == StateType.DDJ1||
                data == StateType.DUA1||
                data == StateType.DUJ1 ){
                // Make Queue State so we can perform later 
                // That can feel more responsibe , and can thing like a trick to cast skills 
                stateMachineFX.QueueStateFX.Add(stateMachineFX.GetState(data));
                return true;

            }
            return false;        
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
            return StateType.Rolling;
        }

        public override void LogicUpdate()
        {
            stateMachineFX.m_ClientVisual.coreMovement.SetRoll();
        }


        public override void PlayAnim(int nbAniamtion = 1, bool sequen = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Rolling);
            stateMachineFX.m_ClientVisual.SetHitBox(false);
            stateMachineFX.m_ClientVisual.UpdateSizeHurtBox(true);
            if (stateData.Sounds) stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }

        public override void End()
        {
            stateMachineFX.m_ClientVisual.UpdateSizeHurtBox();
            stateMachineFX.CoreMovement.ResetVelocity();
            base.End();
        }

    }

}
