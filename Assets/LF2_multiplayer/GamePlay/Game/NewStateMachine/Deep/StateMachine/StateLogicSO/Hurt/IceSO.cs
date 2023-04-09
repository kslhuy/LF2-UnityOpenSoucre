using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Ice", menuName = "StateLogic/Common/Hurt/Ice")]
    public class IceSO : StateLogicSO<IceLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class IceLogic : StateActionLogic
    {
        //Component references
        private int nbHitInIce;
        private bool onAir;
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
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Ice);
            base.PlayAnim();
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
            nbHitInIce = 0;
            if (!onAir){
                if (stateData.SpawnsFX.Length > 0)
                    InstantiateFXGraphic(stateData.SpawnsFX[0]._Object,stateMachineFX.CoreMovement.transform,Vector3.up,false);
            }
            onAir = false;
            if (stateMachineFX.CoreMovement.GetFacingDirection() == 1){
                stateMachineFX.AnticipateState(StateType.FallingBack);
            }else{
                stateMachineFX.AnticipateState(StateType.FallingFront);
            }
        }

        public override void HurtResponder(Vector3 dirToRespond)
        {
            nbHitInIce++;
            // Debug.Log(nbHitInIce);
            if (nbHitInIce % 2 == 0 ){
                Debug.Log("22222");
                stateMachineFX.CoreMovement.SetHurtMovement(dirToRespond);
                End();
            }else{
                stateMachineFX.CoreMovement.SetHurtMovement(dirToRespond);
            }
            // stateMachineFX.CoreMovement.TakeControlTransform(true);
            // stateMachineFX.CoreMovement.ResetVelocity();
        }
    }

}
