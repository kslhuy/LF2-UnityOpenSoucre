using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "RecoverMana", menuName = "StateLogic/Common/RecoverMana")]
    public class RecoverManaSO : StateLogicSO<RecoverManaLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class RecoverManaLogic : StateActionLogic
    {

        InteractionZone itr ;

        //Component references
        // private RecoverManaLogicSO _originSO => (RecoverManaLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            itr = stateMachine.itr;
        }

        public override bool ShouldAnticipate(ref StateType data)
        {
            if (data == StateType.Defense){
                stateMachineFX.AnticipateState(StateType.Idle);
            }
            else if (data == StateType.Move){
                stateMachineFX.AnticipateState(StateType.Move);
            }
            return true;
            
        }

        
        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_RecoverMana);
            // stateMachineFX.m_ClientVisual.UpdateSizeHurtBox();
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

        public override void Enter()        {
        
            if( !Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
        
        }

        public override void LogicUpdate()
        {
            if (stateMachineFX.m_ClientVisual._IsServer){
                if (stateMachineFX.m_ClientVisual.m_NetState.MPPoints >= 500 ) return;
                if ( nbTickRender % 6 == 0 ){
                    stateMachineFX.m_ClientVisual.m_NetState.MPPoints += 15; 
                }
            }
            base.LogicUpdate();
        }


    }

}
