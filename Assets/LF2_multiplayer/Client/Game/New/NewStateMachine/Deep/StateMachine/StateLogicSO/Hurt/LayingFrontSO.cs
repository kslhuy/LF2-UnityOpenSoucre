using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "LayingFront", menuName = "StateLogic/Common/Hurt/LayingFront")]
    public class LayingFrontSO : StateLogicSO<LayingFrontLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class LayingFrontLogic : StateActionLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

      public override void Enter()
        {
            if( !Anticipated)
            {
                PlayAnim() ;
            }            
            base.Enter();

        }


        public override void Exit()
        {
            base.Exit();
        }

        public override void End()
        {
            // Debug.Log("End");

            stateMachineFX.m_ClientVisual.FlashCharacter(5);
            stateMachineFX.idle();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.CanCommit) stateMachineFX.m_ClientVisual.m_NetState.LifeStateChangeServerRpc(LifeState.Fainted);
            stateMachineFX.m_ClientVisual.CloseHurtBox();

            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play("Empty_anim");
            stateMachineFX.m_ClientVisual.InjuryAnimator.Play(stateData.vizAnim[nbanim-1].AnimHashId);
            // stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);

            
        }


        public override StateType GetId()
        {
            return StateType.LayingFront;
        }

    }

}
