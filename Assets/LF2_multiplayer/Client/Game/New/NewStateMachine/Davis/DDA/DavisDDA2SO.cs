using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DavisDDA2", menuName = "StateLogic/Davis/Special/DDA2")]
    public class DavisDDA2SO : StateLogicSO<DavisDDA2Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
// Projectle normal 
    public class DavisDDA2Logic : LaunchProjectileLogic
    {
        private bool cantransition_ToNextAnimation; // Or mean Next State
        private bool m_Launched;

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


        public override StateType GetId()
        {
            return StateType.DDA2;
        }

        public override void OnAnimEvent(int id)
        {
            if (stateMachineFX.m_ClientVisual._IsServer) {
                SpwanProjectile(stateData, new Vector3 (stateMachineFX.CoreMovement.FacingDirection,0,stateMachineFX.InputZ));
                m_Launched = true;
            }       
        }


        public override void End(){
            if (stateMachineFX.m_ClientVisual._IsServer) {
                if (!m_Launched) SpwanProjectile(stateData, new Vector3 (stateMachineFX.CoreMovement.FacingDirection,0,stateMachineFX.InputZ));

            }
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
