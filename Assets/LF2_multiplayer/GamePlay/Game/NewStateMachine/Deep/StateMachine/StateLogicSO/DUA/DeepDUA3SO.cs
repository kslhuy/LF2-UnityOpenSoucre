using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DeepDUA3", menuName = "StateLogic/Deep/Special/DUA3")]
    public class DeepDUA3SO : StateLogicSO<DeepDUA3Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class DeepDUA3Logic : MeleLogic
    {
        // private AttackDataSend Atk_data ;

        private bool attackOnce;
        private int attackDUJ3;

        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            attackDUJ3 = Animator.StringToHash("DUJ_3_sub_anim");
            // Atk_data = new AttackDataSend();

            // Atk_data.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;
            // Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
            // Atk_data.Fall_p = stateData.DamageDetails[0].fall;
        }


        public override bool ShouldAnticipate(ref InputPackage requestData)
        {
            // Debug.Log(attackOnce);
            if (requestData.StateTypeEnum == StateType.Attack && !attackOnce){
                attackOnce = true;
                stateMachineFX.m_ClientVisual.NormalAnimator.Play(attackDUJ3);
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
            if (Time.time - TimeStarted_Animation > 0.15f){
                if (stateMachineFX.CoreMovement.IsGounded()){
                    stateMachineFX.ChangeState(StateType.Crouch);
                }
            }
            stateMachineFX.CoreMovement.SetFallingDown();
 
        }



        public override void End(){
            attackOnce = false;
            stateMachineFX.idle();
        }


        public override void PlayAnim(int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            attackOnce =  false;
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUA_3);
        }

        public override void PlayPredictState(int nbanim = 1 , bool sequence = false)
        {

            if (stateMachineFX.m_ClientVisual.Owner) {

                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            stateMachineFX.CoreMovement.CustomJump(stateData.Dy ,stateData.Dx );
            PlayAnim(nbanim , sequence);
        }
        public override StateType GetId()
        {
            return StateType.DUA3;
        }


        public override void AddCollider(Collider collider)
        {
            base.AddCollider(collider); 
        }
    }

}
