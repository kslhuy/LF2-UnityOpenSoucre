using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "HenryDDA1", menuName = "StateLogic/Henry/Special/DDA1")]
    public class HenryDDA1SO : StateLogicSO<HenryDDA1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
// Projectle normal == REd Arrow , Critical Shot , 30 MP
    public class HenryDDA1Logic : LaunchProjectileLogic
    {
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
            return StateType.DDA1;
        }

        public override void OnAnimEvent(int id)
        {

            if (stateMachineFX.m_ClientVisual._IsServer) {
                SpwanProjectile(stateData, new Vector3(stateMachineFX.CoreMovement.FacingDirection , 0, stateMachineFX.InputZ));
                m_Launched = true;
                }
            

        }


        public override void End(){
            if (stateMachineFX.m_ClientVisual._IsServer) {
                if (!m_Launched) SpwanProjectile(stateData, new Vector3(stateMachineFX.CoreMovement.FacingDirection , 0, stateMachineFX.InputZ));

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
