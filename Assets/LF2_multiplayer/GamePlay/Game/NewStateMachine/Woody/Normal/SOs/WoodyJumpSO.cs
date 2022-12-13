using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Jump", menuName = "StateLogic/Woody/Jump")]
    public class WoodyJumpSO : StateLogicSO<WoodyJumpLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class WoodyJumpLogic : StateActionLogic
    {


        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override bool ShouldAnticipate(ref InputPackage data)
        {            
            if (data.StateTypeEnum == StateType.Attack || data.StateTypeEnum == StateType.DDJ1){
                stateMachineFX.AnticipateState(data.StateTypeEnum);
                return true;
            }
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
            GameObject.Instantiate(stateData.SpawnsFX[0]._Object,stateMachineFX.m_ClientVisual.transform.position, Quaternion.identity);
            stateMachineFX.m_ClientVisual.DisableHitBox();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play("Jump_anim");
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }

        // // The state inherited by Air State dont need to call base PlayPredictState
        // // Instead , call PlayAnimation direct

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            stateMachineFX.CoreMovement.CheckIfShouldFlip((int)stateMachineFX.InputX);

            stateMachineFX.CoreMovement.SetJump(stateMachineFX.InputX,stateMachineFX.InputZ);

            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }
        


        public override void LogicUpdate() {
            
            if (stateMachineFX.m_ClientVisual.CanCommit){
                if (Time.time - TimeStarted_Animation > 0.1f){
                    // Debug.Log($"Time Start  Owner ={TimeStarted_Animation} ");
                    if (stateMachineFX.CoreMovement.IsGounded()){
                        stateMachineFX.ChangeState(StateType.Land);
                    }
                }
                stateMachineFX.CoreMovement.SetFallingDown();
                // Debug.Log(stateMachineFX.InputX);
                stateMachineFX.CoreMovement.CheckIfShouldFlip((int)stateMachineFX.InputX);

            } 
            else {
                // Debug.Log($"Time Start Not Owner ={TimeStarted_Animation} ");
                if (Time.time - TimeStarted_Animation > 0.15f){
                    // Debug.Log("TIMER Ok , Check Is Grounded");
                    if (stateMachineFX.CoreMovement.IsGounded()){
                        Debug.Log("Land Not owner");
                        stateMachineFX.ChangeState(StateType.Land);
                    }
                }

            }
        }


        public override StateType GetId()
        {
            return stateData.StateType;
        }
    }

}
