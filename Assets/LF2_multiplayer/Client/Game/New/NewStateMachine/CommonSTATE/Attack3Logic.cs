using UnityEngine;
namespace LF2.Client{


    public class Attack3Logic : StateActionLogic
    {
        private AttackDataSend Atk_data = new AttackDataSend();

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            // Atk_data = new AttackDataSend();
            Atk_data.Direction = stateData.DamageDetails[0].Dirxyz;
            Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
            Atk_data.Fall_p = stateData.DamageDetails[0].fall;
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
            return StateType.Attack3;
        }

        public override void LogicUpdate()
        {        
            stateMachineFX.CoreMovement.CustomMove(stateData.Dx);
        }


        public override void Exit()
        {
            stateMachineFX.nbHit = 1;
        }

        public override void End(){
            stateMachineFX.idle();
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim(nbanim);
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Attack_3);

        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Client Send to Server  =>>>  Server know what state Client is =>>  Server propagate to all others players (except this client (who send))).
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            base.PlayPredictState();
        }


        public override void AddCollider(Collider collider)
        {
            IHurtBox damageables = collider.GetComponentInParent<IHurtBox>();
 
            if (damageables != null && damageables.IsDamageable(stateMachineFX.team) && damageables.NetworkObjectId != stateMachineFX.m_ClientVisual.NetworkObjectId)
            {
                Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
                Atk_data.Direction = stateData.DamageDetails[0].Dirxyz;
                Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
                Atk_data.Fall_p = stateData.DamageDetails[0].fall;
                Atk_data.Facing = stateMachineFX.CoreMovement.FacingDirection;
                damageables.ReceiveHP(Atk_data);

                if (stateData.SpawnsFX.Length > 0)
                {
                    GameObject.Instantiate(stateData.SpawnsFX[0]._Object, damageables.transform.position + stateMachineFX.CoreMovement.FacingDirection *stateData.SpawnsFX[0].pivot, Quaternion.identity);
                }

                // stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds, damageables.transform.position);
                stateMachineFX.m_ClientVisual.ActiveHitLag(0.3f , 0.1f);
                // stateMachineFX.nbHit += 1;
            }  
        }
    }
}