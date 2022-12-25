using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Air", menuName = "StateLogic/Common/Air")]
    public class AirSO : StateLogicSO<AirLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class AirLogic : StateActionLogic
    {
        // 
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override bool ShouldAnticipate(ref InputPackage data)
        {
            if ( data.StateTypeEnum == StateType.Move ){
                stateMachineFX.CoreMovement.SetXZ(0.1f*stateMachineFX.InputX,0.1f*stateMachineFX.InputZ);
            }
            return true;

        }

        public override void Enter()
        {
            base.Enter();
        }


        
        public override void LogicUpdate() {
            
            if (stateMachineFX.CoreMovement.IsGounded()){
                stateMachineFX.ChangeState(StateType.Crouch);
            }
            stateMachineFX.CoreMovement.SetFallingDown();
            // if (stateMachineFX.m_ClientVisual.Owner) {
            //     // Debug.Log(stateMachineFX.InputX);
            //     stateMachineFX.CoreMovement.SetXZ(0.1f*stateMachineFX.InputX,0.1f*stateMachineFX.InputZ);
            // }        
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.DisableHitBox();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Air);

        }
        // // The state inherited by Air State dont need to call base PlayPredictState
        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // // HUY : MAybe This Air State Dont need to be sync , just let client do by instinct
            
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            // if (stateMachineFX.m_ClientVisual.Owner) 
            //     stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            

            base.PlayPredictState(nbAniamtion, sequen);
        }


        public override StateType GetId()
        {
            return StateType.Air;
        }


    }

}
