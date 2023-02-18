using System.Collections.Generic;
using LF2.Utils;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "Run", menuName = "StateLogic/Common/Run")]
    public class RunSO : StateLogicSO<RunLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    public class RunLogic : StateActionLogic
    {
        // private bool flip;
        private List<SpecialFXGraphic> m_SpawnedGraphics = null;
        private float startTime;
        
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

         public override bool ShouldAnticipate(ref InputPackage requestData)
        {
            if (requestData.StateTypeEnum == StateType.Jump){
                stateMachineFX.nbJump += 1 ; 
                stateMachineFX.AnticipateState(StateType.DoubleJump);
                return true;

            }
            else if (requestData.StateTypeEnum == StateType.Defense){
                stateMachineFX.AnticipateState(StateType.Rolling);
                return true;
            }
            else if (requestData.StateTypeEnum == StateType.Attack)
            {
                stateMachineFX.AnticipateState(StateType.AttackRun);
                return true;
            }           
            return false;
    
        }


        public override void Enter()        {
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();

        }


        public override void Exit()
        {
            stateMachineFX.CoreMovement.ResetVelocity();
            base.Exit();
        }

        public override StateType GetId()
        {
            return StateType.Run;
        }



        public override void OnAnimEvent(int id)
        {
            if (id == 1) stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[0]);
            else stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[1]);
            
        }

        public override void PlayAnim(int nbAniamtion = 1, bool sequen = false)
        {

            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Run);
            // flip =  stateMachineFX.CoreMovement.GetFacingDirection() == 1 ? false : true ;
            // m_SpawnedGraphics = InstantiateSpecialFXGraphics(stateMachineFX.CoreMovement.transform , true,flip, GetId()); 
            // m_SpawnedGraphics[0].m_ParticleSystemsToTurnOffOnShutdown[0].fli
            startTime = Time.time;

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
            if (stateMachineFX.m_ClientVisual.Owner) stateMachineFX.m_ClientVisual.coreMovement.SetRun(stateMachineFX.InputZ);
            if ( stateMachineFX.InputX == 1 && stateMachineFX.m_ClientVisual.coreMovement.GetFacingDirection() == -1 ){
                stateMachineFX.ChangeState(StateType.Sliding);
            }else if ( stateMachineFX.InputX == -1 && stateMachineFX.m_ClientVisual.coreMovement.GetFacingDirection() == 1 ){
                stateMachineFX.ChangeState(StateType.Sliding);
            }
            // if (m_SpawnedGraphics == null){
            //     m_SpawnedGraphics = InstantiateSpecialFXGraphics(stateMachineFX.CoreMovement.transform, true, GetId()); 
            // }

            if (nbTickRender % 30 == 0){
                // float end  = startTime + 0.5f;
                // if (Time.time > end){
                    var pf = stateData.SpawnsFX[0];
                    Vector3 pivot = new Vector3(pf.pivot.x*stateMachineFX.CoreMovement.GetFacingDirection() ,pf.pivot.y,pf.pivot.z );

                    InstantiateFXGraphic(pf._Object, stateMachineFX.CoreMovement.transform,pivot, true, GetId()); 
                    // startTime = Time.time;

                }
            base.LogicUpdate();
        }
    }

}
