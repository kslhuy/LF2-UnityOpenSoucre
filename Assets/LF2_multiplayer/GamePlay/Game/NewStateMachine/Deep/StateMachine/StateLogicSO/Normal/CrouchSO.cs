using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Crouch", menuName = "StateLogic/Common/Crouch")]
    public class CrouchSO : StateLogicSO<CrouchLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class CrouchLogic : StateActionLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override bool ShouldAnticipate(ref InputPackage data)
        {
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
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Land);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);

        }
        // Should not call 
        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            // Debug.Log("  Should not call predict State Land ");
            // if (stateMachineFX.m_ClientVisual.Owner) {
            //     stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            // }
            PlayAnim(nbanim , sequence);
        }

        public override void End(){
            stateMachineFX.idle();
        }
        


        public override StateType GetId()
        {
            return StateType.Crouch;
        }


    }

}
