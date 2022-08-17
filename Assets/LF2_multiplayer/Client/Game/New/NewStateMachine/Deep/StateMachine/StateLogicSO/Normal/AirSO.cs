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

        public override void Enter()
        {
            stateMachineFX.CoreMovement.SyncRigidVSTransform();
            base.Enter();
        }


        
        public override void LogicUpdate() {

            
            // Debug.Log($" Air_FX = {Time.time - TimeStarted_Animation}"); 
            
            if (stateMachineFX.CoreMovement.IsGounded()){
                stateMachineFX.ChangeState(StateType.Crouch);
            }
            stateMachineFX.CoreMovement.SetFallingDown();

        

        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateData.vizAnim[nbanim-1].AnimHashId);

        }
        // // The state inherited by Air State dont need to call base PlayPredictState
        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // // HUY : MAybe This Air State Dont need to be sync , just let client do by instinct
            
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            // if (stateMachineFX.m_ClientVisual.CanCommit) 
            //     stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            

            base.PlayPredictState(nbAniamtion, sequen);
        }


        public override StateType GetId()
        {
            return StateType.Air;
        }


    }

}
