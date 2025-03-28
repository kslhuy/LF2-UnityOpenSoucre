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

        public override bool ShouldAnticipate(ref StateType data)
        {            
            if (data == StateType.Attack ){
                stateMachineFX.AnticipateState(StateType.AttackJump);
                return true;
            }
            else if(data == StateType.DDJ1){
                stateMachineFX.AnticipateState(data);
                return true;
            }
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
                if (nbTickRender > 10){
                    if (stateMachineFX.CoreMovement.IsGounded()){
                        stateMachineFX.ChangeState(StateType.Land);
                    }
                }
                stateMachineFX.CoreMovement.SetFallingDown();
                stateMachineFX.CoreMovement.CheckIfShouldFlip((int)stateMachineFX.InputX);

            } 
            else {
                if (nbTickRender > 11){
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
