using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Jump", menuName = "StateLogic/Common/Jump")]
    public class JumpSO : StateLogicSO<JumpLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class JumpLogic : StateActionLogic
    {


        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override bool ShouldAnticipate(ref InputPackage data)
        {            
            if (data.StateTypeEnum == StateType.Attack){
                stateMachineFX.AnticipateState(StateType.AttackJump);
                return true;
            }
            // else if (data.StateTypeEnum == StateType.Move){
            //     stateMachineFX.CoreMovement.CheckIfShouldFlip((int)stateMachineFX.InputX);
            //     return true;
            // }
            return false;
        }

        public override void Enter()        {
            PlayAnim();
            base.Enter();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            GameObject.Instantiate(stateData.SpawnsFX[0]._Object,stateMachineFX.m_ClientVisual.transform.position, Quaternion.identity);
            stateMachineFX.m_ClientVisual.SetHitBox(false);
            stateMachineFX.m_ClientVisual.NormalAnimator.Play("Jump_anim");
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }

        // // The state inherited by Air State dont need to call base PlayPredictState
        // // Instead , call PlayAnimation direct

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            stateMachineFX.CoreMovement.CheckIfShouldFlip((int)stateMachineFX.InputX);

            stateMachineFX.CoreMovement.SetJump(stateMachineFX.InputX,stateMachineFX.InputZ);

            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }
        


        public override void LogicUpdate() {
            
            if (stateMachineFX.m_ClientVisual.Owner){
                // Debug.Log(nbTickRender + 1 % 6 );
                if (nbTickRender > 7 ){
                    // Debug.Log($"check ground ");
                    if (stateMachineFX.CoreMovement.IsGounded()){
                        stateMachineFX.ChangeState(StateType.Land);
                        return;
                    }
                }
                stateMachineFX.CoreMovement.SetFallingDown();
                // Debug.Log(stateMachineFX.InputX);
                stateMachineFX.CoreMovement.CheckIfShouldFlip((int)stateMachineFX.InputX);
            } 
            else {
                // Debug.Log($"Time Start Not Owner ={TimeStarted_Animation} ");
                if (nbTickRender > 12 ){
                    // Debug.Log("TIMER Ok , Check Is Grounded");
                    if (stateMachineFX.CoreMovement.IsGounded()){
                        // Debug.Log("Land Not owner");
                        stateMachineFX.ChangeState(StateType.Land);
                    }
                }
            }
            base.LogicUpdate();
        }


        public override StateType GetId()
        {
            return stateData.StateType;
        }
    }

}
