using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Ice", menuName = "StateLogic/Common/Hurt/Ice")]
    public class IceSO : StateLogicSO<IceLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class IceLogic : StateActionLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
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

            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim();
            // stateMachineFX.m_ClientVisual.NormalAnimator.Play("Walk_anim");
        }

        public override void Enter()
        {
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            if (stateMachineFX.m_ClientVisual.CanCommit) 
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            
            // base.PlayPredictState(nbAniamtion, sequen);
        
        }

        public override void LogicUpdate()
        {
            stateMachineFX.CoreMovement.SetXZ(stateMachineFX.InputX,stateMachineFX.InputZ);

        }
    }

}
