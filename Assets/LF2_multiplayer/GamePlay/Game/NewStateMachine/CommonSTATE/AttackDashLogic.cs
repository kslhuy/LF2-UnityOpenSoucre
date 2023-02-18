using UnityEngine;
namespace LF2.Client{

    public class AttackDashLogic : MeleLogic
    {
        AttackDataSend Atk_data ;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override void Enter()        {
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
        }


        public override StateType GetId()
        {
            return StateType.RunJumpAttack;
        }

        public override void LogicUpdate()
        {
            if (Time.time - TimeStarted_Animation > 0.2f){
                if (stateMachineFX.CoreMovement.IsGounded()){
                    Debug.Log("Ground Attack Jump");
                    stateMachineFX.ChangeState(StateType.Crouch);
                }
            }
            stateMachineFX.CoreMovement.SetFallingDown();
        }


        public override void End(){
            stateMachineFX.AnticipateState(StateType.Crouch);
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Run_Jump_Attack);
        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            base.PlayPredictState(nbAniamtion, sequen);
        }


        public override void AddCollider(Collider collider)
        {
            base.AddCollider(collider);
        }

        public override void OnAnimEvent(int id) 
        {
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }

    }
}