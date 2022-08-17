using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Idle", menuName = "StateLogic/Common/Idle")]
    public class IdleSO : StateLogicSO<IdleLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class IdleLogic : StateActionLogic
    {

        private bool isPlayAnimation;
        InteractionZone itr ;

        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            itr = stateMachine.itr;
        }

        public override bool ShouldAnticipate(ref InputPackage data)
        {
            if (data.StateTypeEnum == StateType.Jump){
                stateMachineFX.AnticipateState(StateType.Jump);
            }

            else if ( data.StateTypeEnum == StateType.Attack ){
                // Debug.Log($"NB Animation{data.NbAnimation}");
                if (itr.TriggerAttack3 || stateMachineFX.nbHit == 3 ) {
                    stateMachineFX.AnticipateState(StateType.Attack3, 1);
                    stateMachineFX.nbHit = 1;
                    }
                else if (data.NbAnimation == 1) stateMachineFX.AnticipateState(StateType.Attack, 1);
                else if (data.NbAnimation == 2) stateMachineFX.AnticipateState(StateType.Attack2, 1);
            }else{
                stateMachineFX.AnticipateState(data.StateTypeEnum);
            }
            return true;
            
        }

        
        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Idle);  
            stateMachineFX.m_ClientVisual.InjuryAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Empty);
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
            
            //
            if (stateMachineFX.m_ClientVisual.CanCommit){

                if(!stateMachineFX.CoreMovement.IsGounded()){
                    // Debug.Log("Air Owner");
                    stateMachineFX.ChangeState(StateType.Air);
                } 
            }else{
                if( Time.time - TimeStarted_Animation > 0.1f && !stateMachineFX.CoreMovement.IsGoundedNotOwner()){
                    // Debug.Log("Air Not Owner");
                    stateMachineFX.ChangeState(StateType.Air);
                } 
            }

            if (!isPlayAnimation){
                isPlayAnimation = IsAnimating();
            }


        }

        public bool IsAnimating()
        {
            if (stateMachineFX.m_ClientVisual.NormalAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle_anim")) {
                return true;
            }
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateData.vizAnim[0].AnimHashId);
            return false;

        }
    }

}
