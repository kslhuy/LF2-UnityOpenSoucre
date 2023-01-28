using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "FallingBack", menuName = "StateLogic/Common/Hurt/FallingBack")]
    public class FallingBackSO : StateLogicSO<FallingBackLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class FallingBackLogic : StateActionLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override bool ShouldAnticipate(ref InputPackage data){
            if (data.StateTypeEnum == StateType.Jump){
                stateMachineFX.AnticipateState(StateType.JumpBack1);
            }
            return true;
        }


        public override void Enter()
        {
            if( !Anticipated)
            {
                PlayAnim() ;
                // stateMachineFX.CoreMovement.SetPlayerInAir(Data.Direction.x ,Data.Direction.y );
            }
            base.Enter();
        }



        public override void LogicUpdate()
        {      
            // Debug.Log(("fallback")); 
            if (Time.time - TimeStarted_Animation > 0.15f){
                if (stateMachineFX.CoreMovement.IsGounded()  ){
                    stateMachineFX.ChangeState(StateType.LayingBack);
                    return;
                }
            }
            
            stateMachineFX.CoreMovement.SetFallingDown();
            stateMachineFX.m_ClientVisual.UpdateSizeHurtBox();

        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            // Debug.Log("play");
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Fall_Back , 0 , 0);

            // stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Fall_Back);
        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            base.PlayPredictState();
        }
        public override void HurtResponder(Vector3 dirToRespond)
        {
            // Debug.Log("HurtResponder");
            stateMachineFX.CoreMovement.ResetVelocity();
            stateMachineFX.CoreMovement.SetHurtMovement(dirToRespond);
        }


        public override StateType GetId()
        {
            return StateType.FallingBack;
        }
    }

}
