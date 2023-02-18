using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DeepDDA2", menuName = "StateLogic/Deep/Special/DDA2")]
    public class DeepDDA2SO : StateLogicSO<DeepDDA2Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class DeepDDA2Logic : LaunchProjectileLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        private bool inputEnable;
        private bool cantransition_ToNextAnimation; // Or mean Next State
        private bool frameTransitionAnim;
        private bool m_Launched;

        public override bool ShouldAnticipate(ref InputPackage requestData)
        {
            if (inputEnable && requestData.StateTypeEnum == StateType.Attack){
                cantransition_ToNextAnimation = true;
                return true;
            }

            // For Debug Only
            if (requestData.StateTypeEnum == StateType.Defense){
                stateMachineFX.idle();
            }
            return false;
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
            return StateType.DDA2;
        }

        public override void LogicUpdate()
        {   
            if (cantransition_ToNextAnimation && frameTransitionAnim){
                inputEnable = false;
                frameTransitionAnim = false;
                cantransition_ToNextAnimation = false;
                stateMachineFX.AnticipateState(StateType.DDA3);
            }
        }

        public override void OnAnimEvent(int id)
        {
            if (id == 1 ) inputEnable = true;
            else if( id == 2 ) frameTransitionAnim = true;
            else {
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
                if (stateMachineFX.m_ClientVisual._IsServer) {
                    stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
                    SpwanProjectileObjectPooling(stateData.Projectiles[0], new Vector3(stateMachineFX.CoreMovement.GetFacingDirection() ,0,stateMachineFX.InputZ));
                    m_Launched = true;
                }
            }                  

        }




        public override void End(){
            if (!m_Launched) {
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
                if (stateMachineFX.m_ClientVisual._IsServer) {
                    SpwanProjectile(stateData.Projectiles[0], new Vector3 (stateMachineFX.CoreMovement.GetFacingDirection(),0,stateMachineFX.InputZ));
                }
            }
            stateMachineFX.idle();
        }
        public override void Exit()
        {
            inputEnable = false;
            frameTransitionAnim = false;
            cantransition_ToNextAnimation = false;
            m_Launched = false;   
            base.Exit();

        }


        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();

            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDA_2);
            // SpwanProjectile(stateData, stateMachineFX.CoreMovement.GetFacingDirection());

        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.Owner) {

                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }

        // public async Task PlaySequence(int i){
        //     float end  = Time.time + subStateDetails[i].durration; 
  
        //     Debug.Log((Time.time < end)); 

        //     while(Time.time < end){
        //         Debug.Log((Time.time < end)); 
        //         await Task.Yield();
        //     }
        // }


        // public async void Test (){
        //     for (int i = 0 ; i < 2; i++ ){
        //         stateMachineFX.m_ClientVisual.NormalAnimator.Play(subStateDetails[i+1].AnimationNameHash);
        //         await PlaySequence(i+1);
        //     }
        //     Exit();

        // }
    }

}
