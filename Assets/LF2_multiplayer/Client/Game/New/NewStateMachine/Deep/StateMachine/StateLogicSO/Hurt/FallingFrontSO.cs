using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "FallingFront", menuName = "StateLogic/Common/Hurt/FallingFront")]
    public class FallingFrontSO : StateLogicSO<FallingFrontLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class FallingFrontLogic : StateActionLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
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

            
            if (Time.time - TimeStarted_Animation > 0.15f){
                if (stateMachineFX.CoreMovement.IsGounded() ){
                    stateMachineFX.ChangeState(StateType.LayingFront);
                    return;
                }
            }
            stateMachineFX.CoreMovement.SetFallingDown();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {

            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play("Empty_anim");
            stateMachineFX.m_ClientVisual.InjuryAnimator.Play(stateData.vizAnim[nbanim-1].AnimHashId);
        }
        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            if (stateMachineFX.m_ClientVisual.CanCommit) {

                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            base.PlayPredictState();
        }


        public override StateType GetId()
        {
            return StateType.FallingFront;
        }
    }

}
