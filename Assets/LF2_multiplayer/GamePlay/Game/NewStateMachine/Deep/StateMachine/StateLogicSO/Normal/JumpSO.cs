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

        public override bool ShouldAnticipate(ref StateType data)
        {            
            if (data is StateType.Attack or StateType.Attack2 ){
                stateMachineFX.AnticipateState(StateType.AttackJump);
                return true;
            }
            // else if (data == StateType.Move){
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
            stateMachineFX.CoreMovement.CheckIfShouldFlip((int)stateMachineFX.InputX);
            stateMachineFX.CoreMovement.SetJump(stateMachineFX.InputX,stateMachineFX.InputZ);
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


            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }
        


        public override void LogicUpdate() {
            

            if (stateMachineFX.m_ClientVisual.Owner){
                if (nbTickRender > 12 ){
                    if (stateMachineFX.CoreMovement.IsGounded()){
                        stateMachineFX.ChangeState(StateType.Land);
                        return;
                    }
                }
                stateMachineFX.CoreMovement.CheckIfShouldFlip((int)stateMachineFX.InputX);
                stateMachineFX.CoreMovement.SetFallingDown();
            }else{
                if (nbTickRender > 12 ){
                    if (stateMachineFX.CoreMovement.CheckGoundedClose(10)){
                        stateMachineFX.ChangeState(StateType.Land);
                        return;
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
