using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "LayingBack", menuName = "StateLogic/Common/Hurt/LayingBack")]
    public class LayingBackSO : StateLogicSO<LayingBackLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class LayingBackLogic : StateActionLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

       public override void Enter()
        {
            if( !Anticipated)
            {
                PlayAnim() ;
            }            
            base.Enter();

        }


        public override void Exit()
        {
            base.Exit();
        }

        public override void End()
        {
            // Debug.Log("End");
            if (stateMachineFX.m_ClientVisual.HPRemain() <= 0 ) return;
            stateMachineFX.m_ClientVisual.FlashCharacter(5);
            stateMachineFX.ChangeState(StateType.Crouch);
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {

            if (stateMachineFX.m_ClientVisual.Owner) stateMachineFX.m_ClientVisual.m_NetState.LifeStateChangeServerRpc(LifeState.Fainted);
            base.PlayAnim();

            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Laying_Back);
            stateMachineFX.m_ClientVisual.UpdateSizeHurtBox();

            if (stateData.Sounds) stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
            stateMachineFX.calculStatics.ResetAll();
        }
        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            if (stateMachineFX.m_ClientVisual.Owner) {

                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            base.PlayPredictState();
        }
        // public override void LogicUpdate()
        // {
        //     stateMachineFX.m_ClientVisual.UpdateSizeHurtBox();
        // }

        public override StateType GetId()
        {
            return StateType.LayingBack;
        }

    }

}
