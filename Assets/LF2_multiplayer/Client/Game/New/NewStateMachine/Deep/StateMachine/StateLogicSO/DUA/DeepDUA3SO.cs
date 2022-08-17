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
        private AttackDataSend Atk_data ;

        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            Atk_data = new AttackDataSend();

            Atk_data.Direction = stateData.DamageDetails[0].Dirxyz;
            Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
            Atk_data.Fall_p = stateData.DamageDetails[0].fall;
        }


        public override bool ShouldAnticipate(ref InputPackage requestData)
        {

            if (requestData.StateTypeEnum == StateType.Attack){
                stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateData.DamageDetails[0].AnimationNameHash);
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
            stateMachineFX.CoreMovement.CustomJump(stateData.Dy ,stateData.Dx );
            PlayAnim(nbanim , sequence);
        }
        public override StateType GetId()
        {
            return StateType.DUA3;
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
