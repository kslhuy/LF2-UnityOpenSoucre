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
            attackDataSend = new AttackDataSend();
            attackDataSend.Amount_injury = stateData.DamageDetails[0].damageAmount;
            attackDataSend.Direction = stateData.DamageDetails[0].Dirxyz;
            attackDataSend.BDefense_p = stateData.DamageDetails[0].Bdefend;
            attackDataSend.Fall_p = stateData.DamageDetails[0].fall;
            attackDataSend.Facing = stateMachineFX.CoreMovement.FacingDirection;
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
            return StateType.DDJ1;
        }

        public override void End(){

            stateMachineFX.idle();
        }

        public override void PlayAnim( int nbanim = 1 , bool sequen = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDJ_1);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[0]);

        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }

 
        public override void OnAnimEvent(int id)
        {
            // ID 300 = Play sound
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[1]);

            StateUtils.DetectNearbyEntities(true , true ,stateMachineFX.m_ClientVisual.PhysicsWrapper.DamageCollider,out RaycastHit[] results ,2000f );
            foreach ( RaycastHit rs in results){
                var damageable = rs.collider.GetComponent<IHurtBox>();
                if (damageable != null && damageable.IsDamageable(stateMachineFX.team) && damageable.NetworkObjectId != stateMachineFX.m_ClientVisual.NetworkObjectId){
                    damageable.ReceiveHP(attackDataSend);
                    stateMachineFX.m_ClientVisual.PlayAudio(stateData.DamageDetails[0].SoundHit[0], damageable.transform.position);
                }
            }
        }
    }

}
