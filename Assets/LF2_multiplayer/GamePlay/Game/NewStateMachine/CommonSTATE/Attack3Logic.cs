using UnityEngine;
namespace LF2.Client{


    public class Attack3Logic : MeleLogic
    {

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;

        }

        

       public override void Enter()
        {
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();

        }


        public override StateType GetId()
        {
            return StateType.Attack3;
        }

        public override void LogicUpdate()
        {        
            stateMachineFX.CoreMovement.CustomMove(stateData.Dx);
        }


        public override void Exit()
        {
            stateMachineFX.nbHit = 1;
        }

        public override void End(){
            stateMachineFX.idle();
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim(nbanim);
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Attack_3);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            base.PlayPredictState();
        }


        public override void AddCollider(Collider collider)
        {
            base.AddCollider(collider);
        }
    }
}