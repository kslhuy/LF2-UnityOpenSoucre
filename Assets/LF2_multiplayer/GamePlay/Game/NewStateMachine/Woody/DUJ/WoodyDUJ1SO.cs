using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "WoodyDUJ1", menuName = "StateLogic/Woody/Special/DUJ1")]
    public class WoodyDUJ1SO : StateLogicSO<WoodyDUJ1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

// fly_crash 
    public class WoodyDUJ1Logic : StateActionLogic
    {

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
            return stateData.StateType;
        }

        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[0]);
        }

        public override void LogicUpdate() {
            
            if (!stateMachineFX.CoreMovement.IsGounded()){
                stateMachineFX.CoreMovement.SetFallingDown();
            }
            
        }

        public override void End(){

            stateMachineFX.idle();
        }


        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            Debug.Log("Call fly crash");
            stateMachineFX.CoreMovement.CustomJump(stateData.Dy , stateData.Dx , 0 ,stateMachineFX.InputZ );
            PlayAnim(nbanim , sequence);
        }

        public override void OnAnimEvent(int id)
        {
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[1]);
        }

    }

}
