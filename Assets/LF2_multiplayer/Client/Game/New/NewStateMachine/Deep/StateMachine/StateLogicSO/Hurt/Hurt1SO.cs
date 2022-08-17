using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Hurt1", menuName = "StateLogic/Common/Hurt/Hurt1")]
    public class Hurt1SO : StateLogicSO<Hurt1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class Hurt1Logic : StateActionLogic
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
            }
            
            base.Enter();
        }

        public override void LogicUpdate()
        {
            if(!stateMachineFX.CoreMovement.IsGounded()){
                stateMachineFX.ChangeState(StateType.FallingBack);
            } 
        }



        public override void End()
        {
            stateMachineFX.idle();
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
            PlayAnim();
        }



        public override StateType GetId()
        {
            return StateType.Hurt1;
        }

    }

}
