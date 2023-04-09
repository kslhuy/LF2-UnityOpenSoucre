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

        public override bool ShouldAnticipate(ref StateType data)
        {
            if (data == StateType.Idle){
                return false;
            }
            else if (data == StateType.Jump){
                stateMachineFX.AnticipateState(StateType.Jump);
            }
            else if ( data == StateType.Attack || data == StateType.Attack2  ){
                // Debug.Log($"NB Animation{data.NbAnimation}");
                if (itr.TriggerAttack3 ) {
                    stateMachineFX.AnticipateState(StateType.Attack3);
                }
                else {
                    stateMachineFX.AnticipateState(data);
                }
            }
            else{
                stateMachineFX.AnticipateState(data);
            }
            return true;
            
        }

        
        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            stateMachineFX.m_ClientVisual.SetHitBox(false);
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

            // if (!isPlayAnimation){
            //     isPlayAnimation = IsAnimating(stateMachineFX.m_ClientVisual.VizAnimation.Idle);
            // }
            stateMachineFX.CoreMovement.SetFallingDown();
        }


    }

}
