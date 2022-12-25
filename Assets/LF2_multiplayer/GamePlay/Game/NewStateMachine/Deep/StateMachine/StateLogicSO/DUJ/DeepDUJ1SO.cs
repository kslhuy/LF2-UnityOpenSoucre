using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DeepDUJ1", menuName = "StateLogic/Deep/Special/DUJ1")]
    public class DeepDUJ1SO : StateLogicSO<DeepDUJ1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class DeepDUJ1Logic : MeleLogic
    {
        private AttackDataSend Atk_data ;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            // Atk_data = new AttackDataSend{
            //     Amount_injury = stateData.DamageDetails[0].damageAmount,
            //     Direction = stateData.DamageDetails[0].Dirxyz,
            //     BDefense_p = stateData.DamageDetails[0].Bdefend,
            //     Fall_p = stateData.DamageDetails[0].fall,
            // };
        }

         float attack12distance;
        private bool m_ImpactPlayed;

        float _timeDealDamage;
        float _deltaTime = 0.1f;

        private List<IHurtBox> AllTargets = new List<IHurtBox>();




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
            return StateType.DUJ1;
        }

        public override void LogicUpdate()
        {   
            if (Time.time - TimeStarted_Animation > 0.15f){
                if (stateMachineFX.CoreMovement.IsGounded()) stateMachineFX.AnticipateState(StateType.Crouch);
            }
            stateMachineFX.CoreMovement.SetFallingDown();



        }


        public override void Exit()
        {
            base.Exit();
        }

        public override void End(){
            stateMachineFX.idle();
        }


        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            stateMachineFX.CoreMovement.CustomJump(stateData.Dy, stateData.Dx);
            PlayAnim(nbanim , sequence);
        }

        public override void AddCollider(Collider collider)
        {
            base.AddCollider(collider);
        }
    }

}
