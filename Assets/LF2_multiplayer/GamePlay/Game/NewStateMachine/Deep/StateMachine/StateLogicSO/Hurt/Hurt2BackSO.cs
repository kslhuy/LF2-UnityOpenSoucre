using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Hurt2Back", menuName = "StateLogic/Common/Hurt/Hurt2Back")]
    public class Hurt2BackSO : StateLogicSO<Hurt2BackLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class Hurt2BackLogic : StateActionLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override void Enter()        {
            if( !Anticipated)
            {
                PlayAnim() ;
            }    
            // stateMachineFX.calculStatics.Reset();            
        
            base.Enter();
        }

        public override void LogicUpdate()
        {
            if(!stateMachineFX.CoreMovement.CheckGoundedClose(10)){
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
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Hurt_2_Back);
        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            if (stateMachineFX.m_ClientVisual.Owner) {

                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim();
        }


        public override StateType GetId()
        {
            return StateType.Hurt2Back;
        }

    }

}
