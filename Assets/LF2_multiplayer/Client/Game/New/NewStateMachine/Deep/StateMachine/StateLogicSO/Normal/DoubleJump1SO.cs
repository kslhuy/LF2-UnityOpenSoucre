using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DoubleJump1", menuName = "StateLogic/Common/DoubleJump1")]
    public class DoubleJump1SO : StateLogicSO<DoubleJump1Logic>
    {
    }

    public class DoubleJump1Logic : AirLogic
    {
        private bool _CanAttack;

        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override bool ShouldAnticipate(ref InputPackage data)
        {
            if (stateMachineFX.CoreMovement.CheckIfShouldFlip((int)stateMachineFX.InputX , false)) {
                // stateMachineFX.m_ClientVisual.NormalAnimator.Play("DoubleJump2_anim");
                stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateData.vizAnim[1].AnimHashId);
                stateMachineFX.CoreMovement.Flip();
                _CanAttack = false;
                return true;
            }
            else if  (data.StateTypeEnum == StateType.Attack && _CanAttack)
            {
                stateMachineFX.AnticipateState(StateType.AttackJump);
                return true;

            }
 
            return false;
        }

        public override void Enter()
        {
            stateMachineFX.CoreMovement.SyncRigidVSTransform();
            if( !Anticipated)
            {
                PlayAnim();
            }
            

            base.Enter();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            GameObject.Instantiate(stateData.SpawnsFX[0]._Object,stateMachineFX.m_ClientVisual.transform.position, Quaternion.identity);
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateData.vizAnim[0].AnimHashId);
        }
        // // The state inherited by Air State dont need to call base PlayPredictState
        // // Instead , call PlayAnimation direct
        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            stateMachineFX.CoreMovement.SetDoubleJump(stateMachineFX.InputX , stateMachineFX.InputZ);
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }



        public override void LogicUpdate() {
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                if (Time.time - TimeStarted_Animation > 0.1f) base.LogicUpdate();
            }else {
                if (Time.time - TimeStarted_Animation > 0.2f) {
                    if (stateMachineFX.CoreMovement.IsGoundedNotOwner()){
                        stateMachineFX.ChangeState(StateType.Crouch);
                    }
                };
            }


        }
        public override void End(){
            _CanAttack = true;
        }

        public override StateType GetId()
        {
            return stateData.StateType;
        }
    }

}
