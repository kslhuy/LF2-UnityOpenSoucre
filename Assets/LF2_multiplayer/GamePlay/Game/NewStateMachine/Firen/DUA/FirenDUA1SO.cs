using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "FirenDUA1", menuName = "StateLogic/Firen/Special/DUA1")]
    public class FirenDUA1SO : StateLogicSO<FirenDUA1Logic>
    {
        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
    ////  Infeno => spwan some effect (Thoi lua)  MP = 150
    public class FirenDUA1Logic : LaunchProjectileLogic
    {
        SpecialFXGraphic specialFX;

        private bool m_Launched;
        private float toIdle;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override bool ShouldAnticipate(ref InputPackage requestData)
        {
            if (requestData.StateTypeEnum.Equals(StateType.Defense) ){
                specialFX.ShutDownSlow();
                stateMachineFX.idle();
            }
            return true;
        }




        public override void Enter()
        {
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();

        }


        public override void End(){

            m_Launched = false;  
            specialFX.Shutdown();
            stateMachineFX.idle();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUA_1);

            specialFX = InstantiateSpecialFXGraphic(stateData.SpawnsFX[0]._Object,  stateMachineFX.m_ClientVisual.PhysicsWrapper.Transform,stateData.SpawnsFX[0].pivot,false,false,GetId());

        }


        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }

        public override StateType GetId()
        {
            return StateType.DUA1;
        }

        public override void OnAnimEvent(int id)
        {
            if (id == 0) stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUA_2);
            if (id == 300) stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
            if (id == 400) stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUA_3);


        }






    }

}
