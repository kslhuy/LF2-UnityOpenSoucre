using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client
{

    [CreateAssetMenu(fileName = "WoodyDUA1", menuName = "StateLogic/Woody/Special/DUA1")]
    public class WoodyDUA1SO : StateLogicSO<WoodyDUA1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
    //// cleg 
    public class WoodyDUA1Logic : StateActionLogic
    {

        private List<IHurtBox> _Listdamagable = new List<IHurtBox>();
        private float timeNow;
        AttackDataSend Atk_data;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        public override void Enter()        {
            if (!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
        }




        public override void LogicUpdate()
        {
            if (Time.time - timeNow > stateData.DamageDetails[0].vrest)
                foreach (IHurtBox damagable in _Listdamagable)
                {
                    if (damagable != null && damagable.IsDamageable(stateMachineFX.m_ClientVisual.teamType))
                    {
                        damagable.ReceiveHP(Atk_data);
                    }
                }
            timeNow = Time.time;
            stateMachineFX.CoreMovement.CustomMove_InputX(stateData.Dx);


        }



        public override void PlayAnim(int nbanim = 1, bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUA_1);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);

        }

        public override void PlayPredictState(int nbanim = 1, bool sequence = false)
        {

            if (stateMachineFX.m_ClientVisual.Owner)
            {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }

            PlayAnim(nbanim, sequence);
        }
        public override StateType GetId()
        {
            return stateData.StateType;
        }

        public override void OnAnimEvent(int id)
        {
            if (id == 102)
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[0]);
            else
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }

        public override void AddCollider(Collider collider)
        {
            IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();
            if (damagable != null)
            {
                Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
                Atk_data.Direction = new Vector3(stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(), stateData.DamageDetails[0].Dirxyz.y, stateData.DamageDetails[0].Dirxyz.z);
                Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
                Atk_data.Fall_p = stateData.DamageDetails[0].fall;
                Atk_data.Effect = (byte)stateData.DamageDetails[0].Effect;
                _Listdamagable.Add(damagable);
            }
        }
        public override void RemoveCollider(Collider collider)
        {
            IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();
            if (damagable != null)
            {
                _Listdamagable.Remove(damagable);
            }
        }


        public override void End()
        {
            stateMachineFX.idle();
            _Listdamagable = new List<IHurtBox>();

        }



    }

}
