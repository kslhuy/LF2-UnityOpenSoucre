using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Idle", menuName = "StateLogic/Henry/Idle")]
    public class HenryIdleSO : StateLogicSO<HenryIdleLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class HenryIdleLogic : StateActionLogic
    {

        // private bool isPlayAnimation;
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

            stateMachineFX.AnticipateState(data.StateTypeEnum);
            return true;
        }

        
        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Idle);  
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
            // Debug.Log("Idle");
            if (stateMachineFX.m_ClientVisual.Owner){

                if(!stateMachineFX.CoreMovement.IsGounded()){
                    Debug.Log("Air Owner");
                    stateMachineFX.ChangeState(StateType.Air);
                } 
            }else{
                if( Time.time - TimeStarted_Animation > 0.1f && !stateMachineFX.CoreMovement.IsGoundedNotOwner()){
                    Debug.Log("Air Not Owner");
                    stateMachineFX.ChangeState(StateType.Air);
                } 
            }

            // if (!isPlayAnimation){
            //     isPlayAnimation = IsAnimating();
            // }


        }

        // public bool IsAnimating()
        // {
        //     if (stateMachineFX.m_ClientVisual.NormalAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle_anim")) {
        //         return true;
        //     }
        //     stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateData.vizAnim[0].AnimHashId);
        //     return false;

        // }
    }

}
