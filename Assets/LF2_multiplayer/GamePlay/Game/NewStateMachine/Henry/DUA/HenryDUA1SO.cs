using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "HenryDUA1", menuName = "StateLogic/Henry/Special/DUA1")]
    public class HenryDUA1SO : StateLogicSO<HenryDUA1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
    //// Falcon Summoning : Goi chim ung = Projectil MP = 120
    public class HenryDUA1Logic : LaunchProjectileLogic
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


        public override void End(){

            if (stateMachineFX.m_ClientVisual._IsServer) {
                if (!m_Launched) SpwanProjectile(stateData.Projectiles[0], Vector3.right*stateMachineFX.CoreMovement.GetFacingDirection());
            }
            m_Launched = false;  
            stateMachineFX.idle();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUA_1);
            // if (stateData.Sounds != null ) stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
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

        public override void OnAnimEvent(int id)
        {
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[0]);
            if (stateMachineFX.m_ClientVisual._IsServer) {
                SpwanProjectile(stateData.Projectiles[0], Vector3.right*stateMachineFX.CoreMovement.GetFacingDirection());
                m_Launched = true;
            }
            

        }





    }

}
