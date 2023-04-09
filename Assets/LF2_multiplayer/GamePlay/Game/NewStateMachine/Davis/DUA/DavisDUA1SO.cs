using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DavisDUA1", menuName = "StateLogic/Davis/Special/DUA1")]
    public class DavisDUA1SO : StateLogicSO<DavisDUA1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
    //// Davis Bo cui? 
    // // Nhay len va chat.
    public class DavisDUA1Logic : StateActionLogic
    {


        protected AttackDataSend Atk_data ;
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            Atk_data = new AttackDataSend();
            Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
                       Atk_data.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;

            Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
            Atk_data.Fall_p = stateData.DamageDetails[0].fall;     

        }


        public override void Enter()        {
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();

        }




        public override void End(){

            stateMachineFX.idle();
        }

        // Nhay len troi 1 khoang
        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            stateMachineFX.CoreMovement.CustomJump(stateData.Dy, stateData.Dx);
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

        public override void LogicUpdate()
        {
            if (Time.time - TimeStarted_Animation > 0.2f){
                if (stateMachineFX.CoreMovement.IsGounded()){
                    stateMachineFX.ChangeState(StateType.Crouch);
                }
            }
            stateMachineFX.CoreMovement.SetFallingDown();
        }


        public override void AddCollider(Collider collider)
        {
            IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();
            if (damagable != null){
                // AllTargets.Add(damagable);
                AttackDataSend Atk_data = new AttackDataSend();
                               Atk_data.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;

                Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
                Atk_data.Fall_p = stateData.DamageDetails[0].fall;         

                damagable.ReceiveHP(Atk_data);
                stateMachineFX.m_ClientVisual.ActiveHitLag(0.3f , 0.1f);
                if (stateData.SpawnsFX.Length > 0)
                {
                    GameObject.Instantiate(stateData.SpawnsFX[0]._Object, damagable.transform.position + stateMachineFX.CoreMovement.GetFacingDirection() *stateData.SpawnsFX[0].pivot, Quaternion.identity);
                }
                if (stateData.Sounds != null)
                {
                   stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds, damagable.transform.position);
                }

            }
        }



    }

}
