using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "JumpBack1", menuName = "StateLogic/Common/JumpBack1")]
    public class JumpBack1SO : StateLogicSO<JumpBack1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class JumpBack1Logic : StateActionLogic
    {


        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }


        public override void Enter()        {
            PlayAnim();
            base.Enter();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            GameObject.Instantiate(stateData.SpawnsFX[0]._Object,stateMachineFX.m_ClientVisual.transform.position, Quaternion.identity);
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DoubleJump_1);
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
                if (nbTickRender > 7){
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
                if (nbTickRender > 7 ){
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
