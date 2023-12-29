using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DeepDUA2", menuName = "StateLogic/Deep/Special/DUA2")]
    public class DeepDUA2SO : StateLogicSO<DeepDUA2Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class DeepDUA2Logic : MeleLogic
    {
        private AttackDataSend Atk_data ;

        private bool inputEnable;
        private bool cantransition_ToNextAnimation; // Or mean Next State
        private bool frameTransitionAnim;
        private StateType _stateToPlay;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            // Atk_data = new AttackDataSend();
            //            // Atk_data.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;

            // Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
            // Atk_data.Fall_p = stateData.DamageDetails[0].fall;
        }

        public override bool ShouldAnticipate(ref StateType requestData)        {

            if (inputEnable && requestData == StateType.Jump){
                cantransition_ToNextAnimation = true;
                _stateToPlay = GetId();
                return true;
            }

            

            // For Debug Only
            if (requestData == StateType.Defense){
                stateMachineFX.idle();
            }
            return false;
        }


        public override void Enter(){
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();

        }




        public override void LogicUpdate()
        {    
            if (cantransition_ToNextAnimation && frameTransitionAnim){
                inputEnable = false;
                frameTransitionAnim = false;
                cantransition_ToNextAnimation = false;
                stateMachineFX.AnticipateState(_stateToPlay);
                // switch(_state) {
                //     case 3 :
                        
                //         stateMachineFX.AnticipateState(StateType.DUA3);
                //         break;
                //     default :
                //         break;
                        
                    
                // }

            }
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
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUA_2);
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
            return stateData.StateType;
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
