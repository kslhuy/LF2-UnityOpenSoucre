using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "FirenDUJ1", menuName = "StateLogic/Firen/Special/DUJ1")]
    public class FirenDUJ1SO : StateLogicSO<FirenDUJ1Logic>
    {
        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

// // Explosion,  MP = 300
    public class FirenDUJ1Logic : MeleLogic
    {

        AttackDataSend Atk_data = new AttackDataSend();

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
            return StateType.DUJ1;
        }


        public override void End(){
            stateMachineFX.idle();

        }

        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);

                        // Spwan just some effet for visual 
            // m_SpawnedGraphics = InstantiateSpecialFXGraphic( stateData.SpawnsFX[0]._Object, stateMachineFX.m_ClientVisual.PhysicsWrapper.Transform ,Vector3.zero ,true, false,GetId());

            
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
            InstantiateSpecialFXGraphic(stateData.SpawnsFX[0]._Object, stateMachineFX.m_ClientVisual.PhysicsWrapper.Transform ,stateData.SpawnsFX[0].pivot ,false, false,GetId());

        }


        public override void AddCollider (Collider collider)
        {
            base.AddCollider(collider);
        }




    }

}
