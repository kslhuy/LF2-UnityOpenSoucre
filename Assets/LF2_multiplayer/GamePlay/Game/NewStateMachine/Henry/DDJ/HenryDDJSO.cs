using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "HenryDDJ", menuName = "StateLogic/Henry/Special/DDJ")]
    public class HenryDDJSO : StateLogicSO<HenryDDJLogic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    ///// Day xe bo' 
    /// Dragon Palm

    public class HenryDDJLogic : LaunchProjectileLogic
    {
        private  RaycastHit[] _Hits  ;
        AttackDataSend attackDataSend;
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
            return StateType.DDJ1;
        }

        public override void End(){

            stateMachineFX.idle();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequen = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDJ_1);


        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }

        

 
        public override void OnAnimEvent(int id)
        {
            // ID 100 = Play sound
            if (id == 100 )  stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[0]);

            if (id == 0) {
                Vector3 ourPosition = stateMachineFX.CoreMovement.transform.position;

                SpwanFX(stateData.SpawnsFX[0], stateMachineFX.CoreMovement.GetFacingDirection() );
                
                stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[1]);

                int nbHit  = StateUtils.DetectNearbyEntities(true , true ,stateMachineFX.m_ClientVisual.PhysicsWrapper.DamageCollider,out RaycastHit[] results ,2000f );
                // Debug.Log("Raycast Hit " + results.Length); 
                attackDataSend.Amount_injury = stateData.DamageDetails[0].damageAmount;
                attackDataSend.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;
                attackDataSend.BDefense_p = stateData.DamageDetails[0].Bdefend;
                attackDataSend.Fall_p = stateData.DamageDetails[0].fall;
                attackDataSend.Effect = (byte)stateData.DamageDetails[0].Effect;

                for ( int i = 0 ; i < nbHit ; i++){
                    var damageable = results[i].collider.GetComponent<IHurtBox>();
                    if (damageable != null && damageable.IsDamageable(stateMachineFX.team) && damageable.NetworkObjectId != stateMachineFX.m_ClientVisual.NetworkObjectId){
                        damageable.ReceiveHP(attackDataSend);
                        // stateMachineFX.m_ClientVisual.PlayAudio(stateData.DamageDetails[0].SoundHit[0], damageable.transform.position);
                    }
                }
                // _Hits = new RaycastHit[1];
            }
        }
        
    }

}
