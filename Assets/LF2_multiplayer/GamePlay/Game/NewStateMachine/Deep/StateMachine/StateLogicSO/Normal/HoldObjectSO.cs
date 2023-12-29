using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "HoldObject", menuName = "StateLogic/Common/HoldObject")]
    public class HoldObjectSO : StateLogicSO<HoldObjectLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class HoldObjectLogic : StateActionLogic
    {

        private bool isPlayAnimation;
        InteractionZone itr ;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            itr = stateMachine.itr;
        }

        public override void Enter()
        {
            if( !Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
        }

        public override bool ShouldAnticipate(ref StateType data)
        {
            return false;            
        }

        
        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            stateMachineFX.m_ClientVisual.SetHitBox(false);
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Pick_Up_Light);
            stateMachineFX.CoreMovement.SetFallingDown();
            //UpdateSizeHurtBox
            stateMachineFX.m_ClientVisual.InitializeSizeHurtBox();  
        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            if (stateMachineFX.m_ClientVisual.Owner) 
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            
            base.PlayPredictState(nbAniamtion, sequen);
        }


        public override StateType GetId()
        {
            return stateData.StateType;
        }



        public override void LogicUpdate()
        {
            
        }


    }

}
