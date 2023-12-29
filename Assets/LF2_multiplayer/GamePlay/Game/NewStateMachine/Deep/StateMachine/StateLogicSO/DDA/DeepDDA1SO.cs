using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DeepDDA1", menuName = "StateLogic/Deep/Special/DDA1")]
    public class DeepDDA1SO : StateLogicSO<DeepDDA1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class DeepDDA1Logic : LaunchProjectileLogic
    {
        private bool inputEnable;
        private bool cantransition_ToNextAnimation; // Or mean Next State
        private bool frameTransitionAnim;
        private bool m_Launched;

        private int inputZ;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            // Atk_data = new AttackDataSend();
            //            // Atk_data.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;

            // Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
            // Atk_data.Fall_p = stateData.DamageDetails[0].fall;
        }

         public override bool ShouldAnticipate(ref StateType requestData)        {
            if (inputEnable && (requestData is  StateType.Attack or StateType.Attack2 )){
                cantransition_ToNextAnimation = true;
                return true;
            }

            

            // For Debug Only
            if (requestData == StateType.Defense){
                stateMachineFX.idle();
            }
            return false;
        }


        public override void Enter(){
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
        }


        public override StateType GetId()
        {
            return stateData.StateType;
        }

        public override void LogicUpdate()
        {   
            if (cantransition_ToNextAnimation && frameTransitionAnim){
                inputEnable = false;
                frameTransitionAnim = false;
                cantransition_ToNextAnimation = false;

                stateMachineFX.AnticipateState(GetId());
            }
        }

        public override void OnAnimEvent(int id)
        {
            if (id == 1 ) inputEnable = true;
            else if (id == 2 ) frameTransitionAnim = true;
            else {
                m_Launched = true;
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
                SpwanProjectileNormal(stateData.Projectiles[0], new Vector3 (stateMachineFX.CoreMovement.GetFacingDirection(),0,0));
                
            }
        }

            
        public override void End(){
            if (!m_Launched) {
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
                SpwanProjectileNormal(stateData.Projectiles[0], new Vector3 (stateMachineFX.CoreMovement.GetFacingDirection(),0,0));
            }
            stateMachineFX.idle();
        }

        public override void Exit()
        {
            m_Launched = false;   
            base.Exit();

        }




        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDA_1);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            inputZ = (int)stateMachineFX.InputZ;
            stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId() );
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
