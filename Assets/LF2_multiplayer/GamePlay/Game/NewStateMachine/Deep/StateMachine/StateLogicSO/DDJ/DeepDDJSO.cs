using System.Collections.Generic;
using LF2.Utils;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DeepDDJ", menuName = "StateLogic/Deep/Special/DDJ")]
    public class DeepDDJSO : StateLogicSO<DeepDDJLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class DeepDDJLogic : MeleLogic 
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            // stateData.frameChecker.initialize(this , stateMachineFX.m_ClientVisual.NormalAnimator,stateMachineFX.m_ClientVisual.spriteRenderer);

        }

        // private List<SpecialFXGraphic> m_SpawnedGraphics = null;

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
            return StateType.DDJ1;
        }

        public override void End(){
            // stateMachineFX.m_ClientVisual.NormalAnimator.enabled = true;
            stateMachineFX.idle();

        }

        // public override void Exit(){
        //     // stateMachineFX.m_ClientVisual.NormalAnimator.enabled = true;
        //     // stateMachineFX.idle();

        // }

        public override void PlayAnim( int frameRender = 1 , bool sequen = false)
        {
            base.PlayAnim();
            // stateMachineFX.m_ClientVisual.NormalAnimator.enabled = false;

            // stateData.frameChecker.initCheck(frameRender);
            // stateMachineFX.m_ClientVisual.coreMovement.TakeControlTransform(true);

            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDJ_1);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }

            PlayAnim(nbanim , sequence);
        }


        public override void LogicUpdate()
        {
            // Dont move 2 firt frame 
            if (stateMachineFX.m_ClientVisual.Owner){
                // stateData.frameChecker.CheckFrame( ()=> stateMachineFX.idle());
                if (nbTickRender > 4 ){
                    stateMachineFX.CoreMovement.CustomMove_InputZ(stateData.Dx,stateData.Dz,stateMachineFX.InputZ);
                }
            }
            else{
                stateMachineFX.CoreMovement.CustomMove_InputZ(stateData.Dx,stateData.Dz,stateMachineFX.InputZ);
            }
            
            // base.LogicUpdate();
            // base.LogicUpdate();
        }

        public override void OnAnimEvent(int id)
        {
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }



        public override void AddCollider(Collider collider)
        {
            base.AddCollider(collider);
        }

      
    }

}
