using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DennisDUJ1", menuName = "StateLogic/Dennis/Special/DUJ1")]
    public class DennisDUJ1SO : StateLogicSO<DennisDUJ1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

// xoay vong 
    public class DennisDUJ1Logic : LaunchProjectileLogic , IFrameCheckHandler
    {
        
        bool hitFrameStart;
        private float timeNow;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            stateData.frameChecker.initialize(this , stateMachineFX.m_ClientVisual.NormalAnimator);

        }

        public override bool ShouldAnticipate(ref InputPackage requestData)
        {
            if (requestData.StateTypeEnum == StateType.Defense){
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


        public override StateType GetId()
        {
            return StateType.DUJ1;
        }


        public override void End(){
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
            stateMachineFX.idle();
        }


        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }

        public override void LogicUpdate()
        {
            stateData.frameChecker.CheckReturnFrame();
            stateMachineFX.CoreMovement.CustomMove_InputZ(stateData.Dx , stateData.Dz,stateMachineFX.InputZ);

            // if (hitFrameStart){
            //     if (Time.time > timeNow + stateData.DamageDetails[0].vrest){
            //         timeNow = Time.time; 
            //         RaycastHit[] result;
            //         int nbHit = StateUtils.DetectMeleeNoRange(true ,stateMachineFX.m_ClientVisual.HitBox,out result );
            //         if (nbHit > 0){
            //             for (int i = 0 ; i < nbHit ; i++){
            //                 IHurtBox damageables = result[i].collider.GetComponent<IHurtBox>(); 
            //                 if (damageables.NetworkObjectId != stateMachineFX.m_ClientVisual.NetworkObjectId){
            //                     Debug.Log("Damageable" + damageables);
            //                     AttackDataSend Atk_data = new AttackDataSend();
            //                     Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
            //                     Atk_data.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;
            //                     Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
            //                     Atk_data.Fall_p = stateData.DamageDetails[0].fall;
            //                     Atk_data.Effect = (byte)stateData.DamageDetails[0].Effect;
            //                     damageables.ReceiveHP(Atk_data);
            //                 }
            //             }
            //         }
            //     }
            // }
        }



        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            stateMachineFX.CoreMovement.TurnONGravity(false);
            stateMachineFX.CoreMovement.TeleportPlayer(stateMachineFX.CoreMovement.transform.position + new Vector3(0f,stateData.Dy,0f) );
            PlayAnim(nbanim , sequence);
        }



        public void onLoopFrame()
        {
            Debug.Log("On Loop Frame");
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1 , 0 , stateData.frameChecker.GetPercentageFrameLoop());
            stateData.frameChecker.initCheck();

        }
    }

}
