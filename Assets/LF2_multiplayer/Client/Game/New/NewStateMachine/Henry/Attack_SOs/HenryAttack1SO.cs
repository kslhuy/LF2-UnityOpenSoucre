using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Attack1", menuName = "StateLogic/Henry/Attack/Attack1")]
    public class HenryAttack1SO : StateLogicSO<HenryAttack1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
// Projectle normal == Deep DDA
    public class HenryAttack1Logic : LaunchProjectileLogic
    {
        private bool cantransition_ToNextAnimation; // Or mean Next State
        private bool m_Launched;
        private bool inputEnable;
        private bool frameTransitionAnim;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }


         public override bool ShouldAnticipate(ref InputPackage requestData)
        {
            if (inputEnable && requestData.StateTypeEnum == StateType.Attack){
                inputEnable = false;
                PlayPredictState();
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


        public override StateType GetId()
        {
            return StateType.Attack;
        }

        public override void OnAnimEvent(int id)
        {
            if (id == 1 ) inputEnable = true;
            else if (id == 2 ) frameTransitionAnim = true;
            else {
                if (stateMachineFX.m_ClientVisual._IsServer) {
                SpwanProjectile(stateData, Vector3.right*stateMachineFX.CoreMovement.FacingDirection);
                m_Launched = true;
                }
            }

        }


        public override void End(){
            if (stateMachineFX.m_ClientVisual._IsServer) {
                if (!m_Launched) SpwanProjectile(stateData, Vector3.right*stateMachineFX.CoreMovement.FacingDirection);

            }
            inputEnable = false;
            m_Launched = false;   
            stateMachineFX.idle();
        }


        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDA_1);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }



    }

}
