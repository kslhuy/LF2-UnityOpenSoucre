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



        public override void End(){
            if (!m_Launched){
                if (stateMachineFX.m_ClientVisual._IsServer) {
                    SpwanProjectile(stateData.Projectiles[0], new Vector3(stateMachineFX.CoreMovement.GetFacingDirection() , 0, stateMachineFX.InputZ));
                }
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
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }

        public override void OnAnimEvent(int id)
        {
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[id - 100]);

            if (id == 101 && stateMachineFX.m_ClientVisual._IsServer) {
                SpwanProjectile(stateData.Projectiles[0], new Vector3(stateMachineFX.CoreMovement.GetFacingDirection() , 0, stateMachineFX.InputZ));
                m_Launched = true;
            }



        }



    }

}
