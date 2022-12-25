using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Move", menuName = "StateLogic/Common/Move")]
    public class MoveSO : StateLogicSO<MoveLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class MoveLogic : StateActionLogic
    {
        private bool isPlayAnimation;

        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override bool ShouldAnticipate(ref InputPackage data)
        {
            if ( data.StateTypeEnum == StateType.Jump ){
                // stateMachineFX.m_ClientVisual.coreMovement.SetJump(stateMachineFX.InputX,stateMachineFX.InputZ);
                stateMachineFX.AnticipateState(StateType.Jump);
                return true;
            }
            else if (data.StateTypeEnum == StateType.Attack)
            {
                stateMachineFX.AnticipateState(data.StateTypeEnum);
                return true;
            }
            else if (data.StateTypeEnum == StateType.Defense){
                stateMachineFX.AnticipateState(data.StateTypeEnum);
                return true;
            }
            else if(data.StateTypeEnum == StateType.Run){
                stateMachineFX.AnticipateState(StateType.Run);
                return true;
            }
            else if(data.StateTypeEnum == StateType.Idle){
                stateMachineFX.AnticipateState(StateType.Idle);
                return true;
            }
            return false;
        }

        public override StateType GetId()
        {
            return StateType.Move;
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            stateMachineFX.m_ClientVisual.NormalAnimator.Play("Walk_anim");
            
        }


        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {

            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim();
            // stateMachineFX.m_ClientVisual.NormalAnimator.Play("Walk_anim");
        }

      public override void Enter()
        {
            if( !Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
        }

        public override void LogicUpdate()
        {    
            
            if (stateMachineFX.m_ClientVisual.Owner) {
                // Debug.Log(stateMachineFX.InputX);
                stateMachineFX.CoreMovement.SetXZ(stateMachineFX.InputX,stateMachineFX.InputZ);
            }

            // if (!isPlayAnimation){
            //     isPlayAnimation = IsAnimating();
            // }
        }

        // public bool IsAnimating()
        // {
        //     if (stateMachineFX.m_ClientVisual.NormalAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateData.vizAnim[0].AnimHashId) {
        //         return true;
        //     }
        //     stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Walk);
        //     return false;

        // }

        public override void Exit()
        {
            isPlayAnimation = false;
            stateMachineFX.CoreMovement.ResetVelocity();
        }
    }

}
