using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "FrezzeDUJ1", menuName = "StateLogic/Frezze/Special/DUJ1")]
    public class FrezzeDUJ1SO : StateLogicSO<FrezzeDUJ1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

// Loc Woay Freeze_DUJ_whirlwind
    public class FrezzeDUJ1Logic : LaunchProjectileLogic
    {
        private bool m_Launched;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;

        }


        public override void Enter()        {
            // Debug.Log("Enter");
            if(!Anticipated)
            {
                // Debug.Log("PlayAnimation");
                PlayAnim();
            }
            base.Enter();
        }


        public override StateType GetId()
        {
            return StateType.DUJ1;
        }


        public override void End(){
            // In case OnAnimEvent() not call 
            if (stateMachineFX.m_ClientVisual._IsServer) {
                if (!m_Launched) {
                    m_Launched = false;
                    SpwanProjectile(stateData.Projectiles[0], Vector3.right*stateMachineFX.CoreMovement.GetFacingDirection());
                }
            }
            m_Launched = false;
            stateMachineFX.idle();
        }


        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }



        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }

        public override void OnAnimEvent(int id)
        {
            if (stateData.SpawnsFX.Length == 0) {
                Debug.LogWarning($"You forgot set SFX for state {GetId()} Frezze");
                return;
            }
            if (stateMachineFX.m_ClientVisual._IsServer) {
                SpwanProjectile(stateData.Projectiles[0], Vector3.right*stateMachineFX.CoreMovement.GetFacingDirection());
                m_Launched = true;
            }
        }

    }

}
