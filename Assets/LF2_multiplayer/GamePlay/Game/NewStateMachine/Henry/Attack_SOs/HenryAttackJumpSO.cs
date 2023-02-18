using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "HenryAttackJump", menuName = "StateLogic/Henry/Attack/AttackJump")]
    public class HenryAttackJumpSO : StateLogicSO<HenryAttackJumpLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class HenryAttackJumpLogic : LaunchProjectileLogic
    {

        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from

        public override void Awake(StateMachineNew stateMachineFX)
        {
            this.stateMachineFX = stateMachineFX;

        }

        public override void Enter()        {
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();

        }


        public override StateType GetId()
        {
            return stateData.StateType;
        }


        public override void End(){
            stateMachineFX.idle();        
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {

            base.PlayAnim(nbanim);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);

            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Jump_Attack);

        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Owner Send to Server   =>>  Server propagate to all others players (except this client , the owner (who send))).
            if (stateMachineFX.m_ClientVisual.Owner){
                Debug.Log("Attack Send to server");
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            int facing = stateMachineFX.m_ClientVisual.coreMovement.GetFacingDirection();
            PlayAnim(nbAniamtion, sequen);
            stateMachineFX.m_ClientVisual.coreMovement.SetHurtMovement(new Vector3 (-stateData.Dx*facing ,stateData.Dy ,0 ) );
            Debug.Log("GetFacingDirection" + stateMachineFX.CoreMovement.GetFacingDirection() );

            if (stateMachineFX.m_ClientVisual._IsServer) {
                Vector3 rotationArrow = (facing == 1)? new Vector3(0,0,-30) : new Vector3(0,180,-30)  ;
                SpwanProjectileObjectPooling(stateData.Projectiles[0], Vector3.right*stateMachineFX.CoreMovement.GetFacingDirection() , rotationArrow);
            }
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





    }
}
