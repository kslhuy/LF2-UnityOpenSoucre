using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "JohnDUJ1", menuName = "StateLogic/John/Special/DUJ1")]
    public class JohnDUJ1SO : StateLogicSO<JohnDUJ1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

// Biscuit
    public class JohnDUJ1Logic : LaunchProjectileLogic
    {
        private bool m_Launched;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;

        }


        public override void Enter()
        {
            Debug.Log("Enter");
            if(!Anticipated)
            {
                Debug.Log("PlayAnimation");
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
                    SpwanProjectile(stateData, Vector3.right*stateMachineFX.CoreMovement.FacingDirection);
                }
            }
            m_Launched = false;
            stateMachineFX.idle();
        }


        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
            // stateMachineFX.m_ClientVisual.NormalAnimator.Play("DUJ_1_anim");
        }



        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }

        public override void OnAnimEvent(int id)
        {
            if (stateData.SpawnsFX.Length == 0) {
                Debug.LogWarning($"You forgot set SFX for state {GetId()} John");
                return;
            }
            if (stateMachineFX.m_ClientVisual._IsServer) {
                SpwanProjectile(stateData, Vector3.right*stateMachineFX.CoreMovement.FacingDirection);
                m_Launched = true;
            }
        }

    }

}
