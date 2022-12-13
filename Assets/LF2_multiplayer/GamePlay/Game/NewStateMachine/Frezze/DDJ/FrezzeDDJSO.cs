using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "FrezzeDDJ", menuName = "StateLogic/Frezze/Special/DDJ")]
    public class FrezzeDDJSO : StateLogicSO<FrezzeDDJLogic>
    {
        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    ///// Column 
    public class FrezzeDDJLogic : LaunchProjectileLogic
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
            return StateType.DDJ1;
        }

        public override void End(){
            if (stateMachineFX.m_ClientVisual._IsServer) {
                if (!m_Launched) {
                    SpawnObject(); 
                }
            }
            stateMachineFX.idle();
            m_Launched = false;
        }

        private async void SpawnObject (){
            for (int i = 0 ; i < stateData.SpawnsFX.Length; i++ ){
                SpwanFX(stateData.SpawnsFX[i], Vector3.right*stateMachineFX.CoreMovement.GetFacingDirection());
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[0]);
                await SpwanSequence();
            }
        }

        public override void PlayAnim( int nbanim = 1 , bool sequen = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDJ_1);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }

 
        public override void OnAnimEvent(int id)
        {
            ///// Shiled  : only spawn
            if (stateMachineFX.m_ClientVisual._IsServer) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }            
            m_Launched = true;
            SpawnObject();
        }

        
        public async Task SpwanSequence(){
            float end  = Time.time + 0.2f; 

            while(Time.time < end){
                // Debug.Log((Time.time < end)); 
                await Task.Yield();
            }
        }

    }

}
