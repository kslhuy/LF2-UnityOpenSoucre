using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "FirenDDA1", menuName = "StateLogic/Firen/Special/DDA1")]
    public class FirenDDA1SO : StateLogicSO<FirenDDA1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
// Projectle normal == REd Arrow , Critical Shot , 30 MP
    public class FirenDDA1Logic : LaunchProjectileLogic
    {
        private bool m_Launched;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
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
            return StateType.DDA1;
        }

        public override void OnAnimEvent(int id)
        {

            if (stateMachineFX.m_ClientVisual._IsServer) {
                SpwanProjectile(stateData.Projectiles[0], new Vector3(stateMachineFX.CoreMovement.GetFacingDirection(),0 ,stateMachineFX.InputZ));
                m_Launched = true;
                }
        }


        public override void End(){
            if (stateMachineFX.m_ClientVisual._IsServer) {
                if (!m_Launched) SpwanProjectile(stateData.Projectiles[0], new Vector3(stateMachineFX.CoreMovement.GetFacingDirection(),0 ,stateMachineFX.InputZ));

            }
            m_Launched = false;   
            stateMachineFX.idle();
        }


        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDA_1);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }



    }

}
