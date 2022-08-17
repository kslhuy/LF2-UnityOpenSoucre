using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DoubleJump2", menuName = "StateLogic/Common/DoubleJump2")]
    public class DoubleJump2SO : StateLogicSO<DoubleJump2Logic>
    {
    }
    // This state may not need 
    public class DoubleJump2Logic : AirLogic
    {
    // This state may not need 

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override bool ShouldAnticipate(ref InputPackage data)
        {            
            return false;
        }

        public override void Enter()
        {
            PlayAnim();
            base.Enter();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play("DoubleJump2_anim");
        }
        // // The state inherited by Air State dont need to call base PlayPredictState
        // // Instead , call PlayAnimation direct
        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncClientRPC(GetId());
            }else{
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }
        


        public override void LogicUpdate() {

            if (Time.time - TimeStarted_Animation > 0.15f) base.LogicUpdate();
        }



        public override StateType GetId()
        {
            return stateData.StateType;
        }
    }

}
