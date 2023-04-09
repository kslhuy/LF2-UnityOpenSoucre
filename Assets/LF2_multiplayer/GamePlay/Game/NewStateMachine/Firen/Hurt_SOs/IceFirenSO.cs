using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "IceFiren", menuName = "StateLogic/Firen/Hurt/Ice")]
    public class IceFirenSO : StateLogicSO<IceFirenLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class IceFirenLogic : StateActionLogic
    {
        //Component references
        private bool onAir;
        private bool _isdodge;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override void Enter()        {
            if( !Anticipated)
            {
                PlayAnim() ;
            }
            
            base.Enter();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            if (stateMachineFX.calculStatics.Current_BDefense > 40 || stateMachineFX.calculStatics.Current_Fall > 40 ){
                stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Ice);
                _isdodge = false;
            }else{
                _isdodge = true;
                Debug.Log("Add Effect dodge here");
                stateMachineFX.idle();
            }
        }


        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {

            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim();
        }

        public override StateType GetId()
        {
            return StateType.Ice;
        }


        public override void LogicUpdate()
        {
            if (_isdodge) return;
            if (nbTickRender > 6 &&  !stateMachineFX.CoreMovement.IsGounded()){
                onAir = true;
                stateMachineFX.CoreMovement.SetFallingDown();
            }
            else if(!onAir) {
                if (nbTickRender > stateData.Duration){
                    End();    
                }
            }
            if (onAir && stateMachineFX.CoreMovement.IsGounded()){
                End();
            }
        }

        public override void End()
        {
            onAir = false;
            if (stateMachineFX.CoreMovement.GetFacingDirection() == 1){
                stateMachineFX.AnticipateState(StateType.LayingBack);
            }else{
                stateMachineFX.AnticipateState(StateType.LayingFront);
            }
        }

        public override void HurtResponder(Vector3 dirToRespond)
        {
            if (_isdodge){
                return;
            }
            // stateMachineFX.CoreMovement.TakeControlTransform(true);
            // stateMachineFX.CoreMovement.ResetVelocity();
            stateMachineFX.CoreMovement.SetHurtMovement(dirToRespond);
        }
    }

}
