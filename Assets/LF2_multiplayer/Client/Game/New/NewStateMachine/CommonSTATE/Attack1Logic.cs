using UnityEngine;

namespace LF2.Client
{
    public class Attack1Logic : MeleLogic
    {
        private AttackDataSend Atk_data ;
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from

        public override void Awake(StateMachineNew stateMachineFX)
        {
            this.stateMachineFX = stateMachineFX;

        }

        public override void Enter()
        {
            Debug.Log("attack not onwer");
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



        public override void End(){
            stateMachineFX.idle();        
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            // Debug.Log(nbanim);

            base.PlayAnim(nbanim);
            stateMachineFX.m_ClientVisual.OpenHitBox();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Attack_1);

            // CameraShake.Instance.ShakeCamera(0.5f,0.2f);

        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Owner Send to Server   =>>  Server propagate to all others players (except this client , the owner (who send))).
            if (stateMachineFX.m_ClientVisual.CanCommit){
                // Debug.Log("Attack Send to server");
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbAniamtion, sequen);
            // base.PlayPredictState(nbAniamtion, sequen);
        }


        public override void AddCollider(Collider collider)
        {

            IHurtBox damageables = collider.GetComponentInParent<IHurtBox>();
            IRebound reboundable = collider.GetComponent<IRebound>();

            if (damageables != null && damageables.IsDamageable(stateMachineFX.team) && damageables.NetworkObjectId != stateMachineFX.m_ClientVisual.NetworkObjectId)
            {
                // Debug.Log($"Sendata mele ");

                Atk_data = new AttackDataSend();
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

                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds, damageables.transform.position);
                stateMachineFX.m_ClientVisual.ActiveHitLag(0.3f , 0.1f);

                // stateMachineFX.nbHit += 1;
            }
            if (reboundable != null && reboundable.IsReboundable() ){
                reboundable.Rebound();
            }
   
            
        }


    }

}
