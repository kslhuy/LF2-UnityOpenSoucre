using System.Collections.Generic;
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

    public class DeepDDJLogic : StateActionLogic
    {
        //Component references
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
        }

        private List<SpecialFXGraphic> m_SpawnedGraphics = null;
        private bool _open;

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
            stateMachineFX.idle();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequen = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateData.vizAnim[0].AnimHashId);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }


        public override void LogicUpdate()
        {
            stateMachineFX.CoreMovement.CustomMove_InputZ(stateData.Dx,stateData.Dz,stateMachineFX.InputZ);
        }

        public override void OnAnimEvent(int id)
        {
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
        }



        public override void AddCollider(Collider collider)
        {
            IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();

            IRebound reboundable = collider.GetComponent<IRebound>();
            if (reboundable != null && reboundable.IsReboundable() ){
                reboundable.Rebound();
            }
            if (damagable != null && damagable.IsDamageable(stateMachineFX.team)){
                // AllTargets.Add(damagable);
                AttackDataSend Atk_data = new AttackDataSend();
                Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
                Atk_data.Facing = stateMachineFX.CoreMovement.FacingDirection;
                Atk_data.Direction = stateData.DamageDetails[0].Dirxyz;

                Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
                Atk_data.Fall_p = stateData.DamageDetails[0].fall;         

                damagable.ReceiveHP(Atk_data);
                stateMachineFX.m_ClientVisual.ActiveHitLag(0.3f , 0.1f);


                GameObject.Instantiate(stateData.SpawnsFX[0]._Object, damagable.transform.position + stateMachineFX.CoreMovement.FacingDirection *stateData.SpawnsFX[0].pivot, Quaternion.identity);
        

                if (stateData.Sounds != null)
                {
                   stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds, damagable.transform.position);
                }
            }

        }



    }

}
