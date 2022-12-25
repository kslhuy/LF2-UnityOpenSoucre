using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DeepDDA3", menuName = "StateLogic/Deep/Special/DDA3")]
    public class DeepDDA3SO : StateLogicSO<DeepDDA3Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class DeepDDA3Logic : LaunchProjectileLogic
    {
        private bool m_Launched;

        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override bool ShouldAnticipate(ref InputPackage requestData)
        {
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
            return StateType.DDA3;
        }



        public override void OnAnimEvent(int id)
        {
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
            if (stateMachineFX.m_ClientVisual._IsServer) {
                SpwanProjectileObjectPooling(stateData.Projectiles[0], new Vector3(stateMachineFX.CoreMovement.GetFacingDirection() ,0,stateMachineFX.InputZ));
                m_Launched = true;
                }        
        }



        public override void End(){

            stateMachineFX.idle();

        }
        
        


        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            m_Launched = false;

            base.PlayAnim();
 
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDA_3);

        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            // if (stateMachineFX.m_ClientVisual.Owner) {

                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            // }
            PlayAnim(nbanim , sequence);
        }
    }

}
