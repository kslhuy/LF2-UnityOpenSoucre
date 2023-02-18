using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DeepDUA1", menuName = "StateLogic/Deep/Special/DUA1")]
    public class DeepDUA1SO : StateLogicSO<DeepDUA1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class DeepDUA1Logic : MeleLogic
    {
        private AttackDataSend Atk_data ;

        private bool inputEnable;
        private bool cantransition_ToNextAnimation; // Or mean Next State
        private bool frameTransitionAnim;

        private StateType _stateToPlay;
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;

        }




         // Can switch to DUA2 or DUA3  direcly
        public override bool ShouldAnticipate(ref InputPackage requestData)
        {
            if (inputEnable && requestData.StateTypeEnum == StateType.Attack){
                cantransition_ToNextAnimation = true;
                _stateToPlay = StateType.DUA2;
                return true;
            }

            if (inputEnable && requestData.StateTypeEnum == StateType.Jump){
                cantransition_ToNextAnimation = true;
                _stateToPlay = StateType.DUA3;

                // _state = 3;
                return true;
            }

            

            // For Debug Only
            if (requestData.StateTypeEnum == StateType.Defense){
                stateMachineFX.idle();
            }
            return false;
        }


        public override void Enter()        {
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();

        }




        public override void LogicUpdate()
        {    
            // 
            if (cantransition_ToNextAnimation && frameTransitionAnim){
                Debug.Log(_stateToPlay);

                inputEnable = false;
                frameTransitionAnim = false;
                cantransition_ToNextAnimation = false;
                stateMachineFX.AnticipateState(_stateToPlay);
            }
            // base.LogicUpdate();
        }



        public override void End(){
            inputEnable = false;
            frameTransitionAnim = false;
            cantransition_ToNextAnimation = false;
            stateMachineFX.idle();
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUA_1);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }

            PlayAnim(nbanim , sequence);
        }
        public override StateType GetId()
        {
            return StateType.DUA1;
        }

        public override void OnAnimEvent(int id)
        {
            if (id == 1 ) {
                inputEnable = true;
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);

            }
            if (id == 2 ) frameTransitionAnim = true;
        }


        public override void AddCollider(Collider collider)
        {

          base.AddCollider(collider);
        }

    }

}
