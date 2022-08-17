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
        private bool m_Launched;

        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from

        public override void Awake(StateMachineNew stateMachineFX)
        {
            this.stateMachineFX = stateMachineFX;

        }

        public override void Enter()
        {
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
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Attack_1);

        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Owner Send to Server   =>>  Server propagate to all others players (except this client , the owner (who send))).
            if (stateMachineFX.m_ClientVisual.CanCommit){
                Debug.Log("Attack Send to server");
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbAniamtion, sequen);
            // base.PlayPredictState(nbAniamtion, sequen);
        }

        public override void OnAnimEvent(int id)
        {
            if (stateMachineFX.m_ClientVisual._IsServer) {
                SpwanProjectile(stateData, Vector3.right*stateMachineFX.CoreMovement.FacingDirection);
                m_Launched = true;
            }
        }



    }
}
