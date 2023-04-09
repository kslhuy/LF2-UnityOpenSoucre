using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Fire", menuName = "StateLogic/Frezze/Hurt/Fire")]
    public class FireFrezzeSO : StateLogicSO<FireFrezzeLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class FireFrezzeLogic : MeleLogic
    {
        private bool _isdodge;
        private bool onAir;
        private Vector3 dirHurt;

        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
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
                // Debug.Log("here");
                stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Fire);
                _isdodge = false;
            }else{
                _isdodge = true;
                Debug.Log("Add Effect dodge here");
                // InstantiateFXGraphic(stateData.SpawnsFX[0]._Object ,stateMachineFX.CoreMovement.transform,stateData.SpawnsFX[0].pivot,true);
                stateMachineFX.idle();
            }
            // stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Fire);
        }


        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            // Debug.Log(GetId());
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim();
        }


        public override StateType GetId()
        {
            return StateType.Fire;
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
                stateMachineFX.AnticipateState(StateType.LayingFront);
            }else{
                stateMachineFX.AnticipateState(StateType.LayingBack);
            }
        }

        public override void AddCollider(Collider collider)
        {
            base.AddCollider(collider);
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
