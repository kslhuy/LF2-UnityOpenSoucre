using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Fire", menuName = "StateLogic/Common/Hurt/Fire")]
    public class FireSO : StateLogicSO<FireLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class FireLogic : MeleLogic
    {
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
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Fire);
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
            if (nbTickRender > 10 && stateMachineFX.CoreMovement.IsGounded()){
                stateMachineFX.ChangeState(StateType.LayingFront);
            }
            stateMachineFX.CoreMovement.SetFallingDown();
        }

        public override void AddCollider(Collider collider)
        {
            base.AddCollider(collider);
        }

        public override void HurtResponder(Vector3 dirToRespond)
        {
            // stateMachineFX.CoreMovement.ResetVelocity();
            stateMachineFX.CoreMovement.SetHurtMovement(dirToRespond);

            // nbHitInIce++;
            // // Debug.Log(nbHitInIce);
            // if (nbHitInIce % 2 == 0 ){
            //     Debug.Log("22222");
            //     stateMachineFX.CoreMovement.SetHurtMovement(dirToRespond);
            //     End();
            // }else{
            //     stateMachineFX.CoreMovement.SetHurtMovement(dirToRespond);
            // }
            // stateMachineFX.CoreMovement.TakeControlTransform(true);
            // stateMachineFX.CoreMovement.ResetVelocity();
        }
    }

}
