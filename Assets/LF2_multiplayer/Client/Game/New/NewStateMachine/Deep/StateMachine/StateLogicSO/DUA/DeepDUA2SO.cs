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
            // Atk_data.Facing = stateMachineFX.CoreMovement.FacingDirection;
            // Atk_data.Direction = stateData.DamageDetails[0].Dirxyz;

            // Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
            // Atk_data.Fall_p = stateData.DamageDetails[0].fall;
        }

        public override bool ShouldAnticipate(ref InputPackage requestData)
        {

            if (inputEnable && requestData.StateTypeEnum == StateType.Jump){
                cantransition_ToNextAnimation = true;
                _stateToPlay = StateType.DUA3;
                return true;
            }

            

            // For Debug Only
            if (requestData.StateTypeEnum == StateType.Defense){
                stateMachineFX.idle();
            }
            return false;
        }


        public override void Enter()
        {
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
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateData.vizAnim[0].AnimHashId);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }
        public override StateType GetId()
        {
            return StateType.DUA2;
        }

        public override void OnAnimEvent(int id)
        {
            if (id == 1 ) inputEnable = true;
            if (id == 2 ) frameTransitionAnim = true;
        }


        public override void AddCollider(Collider collider)
        {

            IHurtBox damageables = collider.GetComponentInParent<IHurtBox>();
 
            if (damageables != null && damageables.IsDamageable(stateMachineFX.team) && damageables.NetworkObjectId != stateMachineFX.m_ClientVisual.NetworkObjectId)
            {
                Atk_data = new AttackDataSend();
                Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
                Atk_data.Facing = stateMachineFX.CoreMovement.FacingDirection;
                Atk_data.Direction = stateData.DamageDetails[0].Dirxyz;

                Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
                Atk_data.Fall_p = stateData.DamageDetails[0].fall;
                damageables.ReceiveHP(Atk_data);


                GameObject.Instantiate(stateData.SpawnsFX[0]._Object, damageables.transform.position + stateMachineFX.CoreMovement.FacingDirection *stateData.SpawnsFX[0].pivot, Quaternion.identity);
                


                if (stateData.Sounds != null)
                {
                   stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds, damageables.transform.position);
                }
            }              
        }

    }

}
