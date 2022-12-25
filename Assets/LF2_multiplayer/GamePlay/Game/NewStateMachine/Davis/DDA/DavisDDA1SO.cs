using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DavisDDA1", menuName = "StateLogic/Davis/Special/DDA1")]
    public class DavisDDA1SO : StateLogicSO<DavisDDA1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
// Projectle normal == Deep DDA
    public class DavisDDA1Logic : LaunchProjectileLogic
    {
        private bool cantransition_ToNextAnimation; // Or mean Next State
        private bool m_Launched;
        private bool frameTransitionAnim;

        private int countDDA = 1;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }


         public override bool ShouldAnticipate(ref InputPackage requestData)
        {
            if ( requestData.StateTypeEnum == StateType.Attack){
                cantransition_ToNextAnimation = true;
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
            return StateType.DDA1;
        }
        public override void LogicUpdate()
        {   
            if (cantransition_ToNextAnimation && frameTransitionAnim){
                m_Launched = true;
                frameTransitionAnim = false;
                cantransition_ToNextAnimation = false;
                stateMachineFX.AnticipateState(StateType.DDA2);
            }
        }
        public override void OnAnimEvent(int id)
        {
            if (id == 2 ) frameTransitionAnim = true;
            else if (id == 0 ){
                if (stateMachineFX.m_ClientVisual._IsServer) {
                    SpwanProjectileObjectPooling(stateData.Projectiles[0], new Vector3 (stateMachineFX.CoreMovement.GetFacingDirection(),0,stateMachineFX.InputZ));
                    m_Launched = true;
                }
            }
            else if(id == 100) {
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
            }
        }


        public override void End(){
            if (!m_Launched) {
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
                if (stateMachineFX.m_ClientVisual._IsServer) {
                    SpwanProjectileObjectPooling(stateData.Projectiles[0], new Vector3 (stateMachineFX.CoreMovement.GetFacingDirection(),0,stateMachineFX.InputZ));
                }
            }
            m_Launched = false;   
            frameTransitionAnim = false;
            cantransition_ToNextAnimation = false;
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
    }
}
