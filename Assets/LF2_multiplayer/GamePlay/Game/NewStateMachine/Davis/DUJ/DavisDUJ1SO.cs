using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DavisDUJ1", menuName = "StateLogic/Davis/Special/DUJ1")]
    public class DavisDUJ1SO : StateLogicSO<DavisDUJ1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

// // Dam xoay len troi 
    public class DavisDUJ1Logic : MeleLogic
    {

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
            return StateType.DUJ1;
        }


        public override void End(){
 
            stateMachineFX.idle();
        }

// // Dam xoay len troi 
        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);

        }



        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            stateMachineFX.CoreMovement.CustomJump(stateData.Dy, stateData.Dx);
            PlayAnim(nbanim , sequence);
        }

        public override void LogicUpdate()
        {   
            if (Time.time - TimeStarted_Animation > 0.15f){
                if (stateMachineFX.CoreMovement.IsGounded()) stateMachineFX.AnticipateState(StateType.Crouch);
            }
            stateMachineFX.CoreMovement.SetFallingDown();



        }



        public override void AddCollider(Collider collider)
        {
            Debug.Log(collider);
            base.AddCollider(collider);
        }

    }

}
