using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "WoodyDUJ1", menuName = "StateLogic/Woody/Special/DUJ1")]
    public class WoodyDUJ1SO : StateLogicSO<WoodyDUJ1Logic>
    {
        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

// fly_crash 
    public class WoodyDUJ1Logic : MeleLogic
    {

        AttackDataSend Atk_data;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }


        public override void Enter()        {
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
        }


        public override StateType GetId()
        {
            return stateData.StateType;
        }

        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
        }

        public override void LogicUpdate() {
            
            if (Time.time - TimeStarted_Animation > 0.3f){
                if (stateMachineFX.CoreMovement.IsGounded()){
                    stateMachineFX.ChangeState(StateType.Land);
                }
                stateMachineFX.CoreMovement.SetFallingDown();
            }
        }

        public override void End(){
            stateMachineFX.idle();
        }


        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            Debug.Log("Call fly crash");
            PlayAnim(nbanim , sequence);
        }

        public override void OnAnimEvent(int id)
        {

            if (id == 0)  stateMachineFX.CoreMovement.CustomJump(stateData.Dy , stateData.Dx , stateMachineFX.CoreMovement.GetFacingDirection() ,stateMachineFX.InputZ );
            else if (id == 100) stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[0]);
            else if (id == 101) stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[1]);
        }

        public override void AddCollider(Collider collider)
        {
            base.AddCollider(collider);
            // IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();
            // if (damagable != null)
            // {
            //     Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
            //     Atk_data.Direction = new Vector3(stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(), stateData.DamageDetails[0].Dirxyz.y, stateData.DamageDetails[0].Dirxyz.z);
            //     Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
            //     Atk_data.Fall_p = stateData.DamageDetails[0].fall;
            //     Atk_data.Effect = (byte)stateData.DamageDetails[0].Effect;
            //     _Listdamagable.Add(damagable);
            // }
        }


    }

}
