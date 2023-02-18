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

        public override void Enter()
        {
            if( !Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
        }

        public override bool ShouldAnticipate(ref InputPackage data)
        {
            if (data.StateTypeEnum == StateType.Idle){
                return false;
            }
            if (data.StateTypeEnum == StateType.Jump){
                stateMachineFX.AnticipateState(StateType.Jump);
            }

            else if ( data.StateTypeEnum == StateType.Attack ){
                // Debug.Log($"NB Animation{data.NbAnimation}");
                if (itr.TriggerAttack3 ) {
                    stateMachineFX.AnticipateState(StateType.Attack3, 1);

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
            // stateMachineFX.m_ClientVisual.UpdateSizeHurtBox();
            stateMachineFX.m_ClientVisual.InitializeSizeHurtBox();  
        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Debug.Log("IDle play predict");
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
            
            //
            if (nbTickRender > 4 ){
                stateMachineFX.m_ClientVisual.coreMovement.TakeControlTransform(false);
            }             
            if (nbTickRender > 8 && stateMachineFX.m_ClientVisual.Owner){
                if(!stateMachineFX.CoreMovement.IsGounded()){
                    // Debug.Log("Air Owner");
                    stateMachineFX.ChangeState(StateType.Air);
                } 
            }else{
                if( nbTickRender > 12 && !stateMachineFX.CoreMovement.CheckGoundedClose(15f)){
                    // Debug.Log("Air Not Owner");
                    stateMachineFX.ChangeState(StateType.Air);
                } 
            }

            if (!isPlayAnimation){
                isPlayAnimation = IsAnimating();
            }
            stateMachineFX.CoreMovement.SetFallingDown();



        }

        public bool IsAnimating()
        {
            if (stateMachineFX.m_ClientVisual.NormalAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle_anim")) {
                return true;
            }
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Idle);
            return false;

        }
    }

}
