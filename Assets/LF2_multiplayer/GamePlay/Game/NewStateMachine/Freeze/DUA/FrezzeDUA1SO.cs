using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "FrezzeDUA1", menuName = "StateLogic/Frezze/Special/DUA1")]
    public class FrezzeDUA1SO : StateLogicSO<FrezzeDUA1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
    //// xSlash 
    public class FrezzeDUA1Logic : LaunchProjectileLogic
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

        public override void End(){

            stateMachineFX.idle();
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUA_1);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);

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
            // Beacause the xSlash move quick and big size so dont need to sync transform and so , we can spwan direct in client 
            SpwanProjectile(stateData.Projectiles[0], new Vector3(stateMachineFX.CoreMovement.GetFacingDirection() ,0,stateMachineFX.InputZ));
            
                   
            m_Launched = true;
        }



    }

}
