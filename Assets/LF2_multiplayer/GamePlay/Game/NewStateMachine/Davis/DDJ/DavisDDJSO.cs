using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DavisDDJ", menuName = "StateLogic/Davis/Special/DDJ")]
    public class DavisDDJSO : StateLogicSO<DavisDDJLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    ///// Dam lien hoan 

    public class DavisDDJLogic : MeleLogic
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
            return StateType.DDJ1;
        }

        public override void End(){

            stateMachineFX.idle();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequen = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDJ_1);
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
            // ID 100 = Play sound
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }

        ///// Dam lien hoan => vua dam, vua di chuyen , 1 khoang
        public override void LogicUpdate()
        {
            stateMachineFX.CoreMovement.CustomMove_InputZ(stateData.Dx,stateData.Dz,stateMachineFX.InputZ);
        }

        


        public override void AddCollider(Collider collider){
            base.AddCollider(collider);
        }





    }

}
