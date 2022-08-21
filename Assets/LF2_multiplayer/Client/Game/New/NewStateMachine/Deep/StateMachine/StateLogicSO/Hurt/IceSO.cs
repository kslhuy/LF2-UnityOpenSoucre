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

        public override void Enter()
        {
            if( !Anticipated)
            {
                PlayAnim() ;
            }
            
            base.Enter();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Empty);
            stateMachineFX.m_ClientVisual.InjuryAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Ice);
        }


        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {

            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim();
        }

        public override StateType GetId()
        {
            return StateType.Ice;
        }


        public override void LogicUpdate()
        {
            stateMachineFX.CoreMovement.SetXZ(stateMachineFX.InputX,stateMachineFX.InputZ);

        }
    }

}
