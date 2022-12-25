using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Land", menuName = "StateLogic/Common/Land")]
    public class LandSO : StateLogicSO<LandLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class LandLogic : StateActionLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

       public override bool ShouldAnticipate(ref InputPackage data)
        {
            if (data.StateTypeEnum == StateType.Jump && stateMachineFX.nbJump == 0 ){
                stateMachineFX.nbJump += 1 ; 
                stateMachineFX.CoreMovement.SetDoubleJump(stateMachineFX.InputX , stateMachineFX.InputZ);
                stateMachineFX.CoreMovement.CheckIfShouldFlip((int)stateMachineFX.InputX);
                stateMachineFX.AnticipateState(StateType.DoubleJump);
                return true;
            }
            if (data.StateTypeEnum == StateType.Defense){
                stateMachineFX.AnticipateState(StateType.Rolling);
            }
            return false;
        }

        public override void Enter( )
        {
            if( !Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
         }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            GameObject.Instantiate(stateData.SpawnsFX[0]._Object,stateMachineFX.m_ClientVisual.transform.position , Quaternion.identity);
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Land);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }
        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            Debug.Log("  Should not call predict State Land ");

            // if (stateMachineFX.m_ClientVisual.Owner) {
            //     stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            // }
            PlayAnim(nbanim , sequence);
        }

        public override void End(){
            if (stateMachineFX.CoreMovement.IsGounded()){
                stateMachineFX.idle();
            }
        }


        // public void ResetAmountOfJumpsLeft()=> amountOfJumpLeft = playerData.amountOfJumpLeft;

        // public void DecreaseAmountOfJumpsLeft()=>amountOfJumpLeft--;

        public override StateType GetId()
        {
            return StateType.Land;
        }


    }

}
