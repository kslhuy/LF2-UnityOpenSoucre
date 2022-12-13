using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Defense", menuName = "StateLogic/Common/Hurt/Defense")]
    public class DefenseSO : StateLogicSO<DefenseLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class DefenseLogic : StateActionLogic
    {
        //Component references
        // private IdleSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }
        public override bool ShouldAnticipate(ref InputPackage data)
        {
            if (data.StateTypeEnum == StateType.DDA1 ||
                data.StateTypeEnum == StateType.DDJ1 ||
                data.StateTypeEnum == StateType.DUA1 ||
                data.StateTypeEnum == StateType.DUJ1  ){
                stateMachineFX.AnticipateState(data.StateTypeEnum);
            }
            return true;
            
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
            return StateType.Defense;
        }



        public override void End(){
            stateMachineFX.idle();
        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            if (stateMachineFX.m_ClientVisual.CanCommit) {

                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
                stateMachineFX.CoreMovement.SetXZ(0,0);

            }
            PlayAnim();
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Defend);
        }
    }

}
