using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DennisDDJ", menuName = "StateLogic/Dennis/Special/DDJ")]
    public class DennisDDJSO : StateLogicSO<DennisDDJLogic>
    {
        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    ///// many kick 
    public class DennisDDJLogic : StateActionLogic 
    {
        private bool m_Launched;
        private List<IHurtBox> _Listdamagable = new List<IHurtBox>();
        private float timeNow ;
        AttackDataSend Atk_data ;




        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
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
            m_Launched = false;
        }
        public override void LogicUpdate() {
            if (Time.time - timeNow >  stateData.DamageDetails[0].vrest)
                foreach (IHurtBox damagable in _Listdamagable){
                    if (damagable != null && damagable.IsDamageable(stateMachineFX.m_ClientVisual.teamType)) {
                        damagable.ReceiveHP(Atk_data);
                    }
                }
            timeNow = Time.time;
            stateMachineFX.CoreMovement.CustomMove_InputX(stateData.Dx);

        }


        public override void PlayAnim( int nbanim = 1 , bool sequen = false)
        {
            stateData.frameChecker.initCheck();
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDJ_1);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.CanCommit) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }

        public override void AddCollider(Collider collider)
        {
            IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();
            if (damagable != null){
                Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
                Atk_data.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;
                Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
                Atk_data.Fall_p = stateData.DamageDetails[0].fall;
                Atk_data.Effect = (byte)stateData.DamageDetails[0].Effect;         
                _Listdamagable.Add(damagable);
            }
        }
        public override void RemoveCollider(Collider collider)
        {
            IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();
            if (damagable != null){
                _Listdamagable.Remove(damagable);
            }        
        }
        


        // public void onHitFrameStart()
        // {
        //     RaycastHit[] result;
        //     Debug.Log("center Hit box world" + stateMachineFX.m_ClientVisual.transform.TransformPoint(stateMachineFX.m_ClientVisual.HitBox.center)); 
        //     int nbHit = StateUtils.DetectNearbyEntities(true ,true,stateMachineFX.m_ClientVisual.HitBox,out result );
        //     if (nbHit > 0){
                
        //         for (int i = 0 ; i < nbHit ; i++){
        //             IHurtBox damageables = result[i].collider.GetComponent<IHurtBox>(); 
        //             if (damageables.NetworkObjectId != stateMachineFX.m_ClientVisual.NetworkObjectId){
        //                 Debug.Log("Damageable" + damageables);
        //                 AttackDataSend Atk_data = new AttackDataSend();
        //                 Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
        //                 Atk_data.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;
        //                 Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
        //                 Atk_data.Fall_p = stateData.DamageDetails[0].fall;
        //                 Atk_data.Effect = (byte)stateData.DamageDetails[0].Effect;
        //                 damageables.ReceiveHP(Atk_data);
        //             }
        //         }
        //     }
        // }



    }

}
