using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Sliding", menuName = "StateLogic/Common/Sliding")]
    public class SlidingSO : StateLogicSO<SlidingLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class SlidingLogic : StateActionLogic
    {
        // private float _runSpeed_Var;
        // private float _runSpeed_Fix; // cache value
        // private float _gainDecreaseRunSpeed;
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override void Enter()
        {
            if(!Anticipated)
            {
                PlayAnim();    
            }
            base.Enter();
        }

        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim( nbAniamtion);
            stateMachineFX.m_ClientVisual.NormalAnimator.Play("Sliding_anim");  

        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.CanCommit) {

                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }


        public override void LogicUpdate()
        {
            // _runSpeed_Var -= _gainDecreaseRunSpeed*Time.deltaTime;
            if (stateMachineFX.m_ClientVisual.CanCommit) stateMachineFX.m_ClientVisual.coreMovement.SetSliding(stateData.Dx);

            // stateMachineFX.AnticipateState(StateType.Idle);
            // if (_runSpeed_Var < 0){
            // }
        }

        

        public override void End()
        {
            stateMachineFX.AnticipateState(StateType.Idle);

        }

        public override StateType GetId(){
            return StateType.Sliding;
        }



        // public void ResetRunVelocity(){
        //     _runSpeed_Var = _runSpeed_Fix;
        // }

    }

}
