using UnityEngine;
namespace LF2.Client
{

    [CreateAssetMenu(fileName = "FirenDDA3", menuName = "StateLogic/Firen/Special/DDA3")]
    public class FirenDDA3SO : StateLogicSO<FirenDDA3Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
    // Projectle normal 
    public class FirenDDA3Logic : LaunchProjectileLogic
    {
        private bool m_Launched;



        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override bool ShouldAnticipate(ref StateType requestData)        
        {
            // For Debug Only
            if (requestData == StateType.Defense)
            {
                stateMachineFX.idle();
            }
            return false;
        }



        public override void Enter()        
        {
            if (!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
        }


        public override StateType GetId()
        {
            return stateData.StateType;
        }




        public override void OnAnimEvent(int id)
        {
            if (id == 0)
            {
                if (stateMachineFX.m_ClientVisual._IsServer)
                {
                    SpwanProjectile(stateData.Projectiles[0], new Vector3(stateMachineFX.CoreMovement.GetFacingDirection(), 0, stateMachineFX.InputZ));
                    m_Launched = true;
                }
            }
            else if(id == 100) {
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
            }

        }


        public override void End()
        {
            if (!m_Launched)
            {
                if (stateMachineFX.m_ClientVisual._IsServer)
                {
                    SpwanProjectile(stateData.Projectiles[0], new Vector3(stateMachineFX.CoreMovement.GetFacingDirection(), 0, stateMachineFX.InputZ));
                }
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
            }
            m_Launched = true;
            stateMachineFX.idle();
        }


        public override void PlayAnim(int nbAniamtion = 1, bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDA_3);
        }

        public override void PlayPredictState(int nbanim = 1, bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.Owner)
            {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim, sequence);
        }




    }

}
