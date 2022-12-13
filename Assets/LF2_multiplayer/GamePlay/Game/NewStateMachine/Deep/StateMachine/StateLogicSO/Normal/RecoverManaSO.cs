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

        private float timerMPrecover = 0.5f;
        private float timeNow;
        InteractionZone itr ;

        //Component references
        // private RecoverManaLogicSO _originSO => (RecoverManaLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            itr = stateMachine.itr;
        }

        public override bool ShouldAnticipate(ref InputPackage data)
        {
            if (data.StateTypeEnum == StateType.Defense){
                stateMachineFX.AnticipateState(StateType.Idle);
            }
            else if (data.StateTypeEnum == StateType.Move){
                stateMachineFX.AnticipateState(StateType.Move);
            }
            return true;
            
        }

        
        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            timeNow = Time.time;
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_RecoverMana);
            // stateMachineFX.m_ClientVisual.UpdateSizeHurtBox();
        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            if (stateMachineFX.m_ClientVisual.CanCommit) 
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            
            base.PlayPredictState(nbAniamtion, sequen);
        }


        public override StateType GetId()
        {
            return stateData.StateType;
        }

        public override void Enter()
        {
        
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
                if ( Time.time - timeNow > timerMPrecover ){
                    timeNow = Time.time;
                    stateMachineFX.m_ClientVisual.m_NetState.MPPoints += 15; 
                }
            }
            // if (Time.time - timeNowSendtoServer > TimerSendMPToServer){
            //     timeNowSendtoServer = Time.time;
            //     cacheMPChange += 1; 
            // }
        }


    }

}
