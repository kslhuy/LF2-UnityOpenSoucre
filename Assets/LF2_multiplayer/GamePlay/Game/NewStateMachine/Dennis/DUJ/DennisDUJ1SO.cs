using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client
{

    [CreateAssetMenu(fileName = "DennisDUJ1", menuName = "StateLogic/Dennis/Special/DUJ1")]
    public class DennisDUJ1SO : StateLogicSO<DennisDUJ1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    // xoay vong 
    public class DennisDUJ1Logic : LaunchProjectileLogic, IFrameCheckHandler
    {

        // private float timeNow;

        private List<IHurtBox> _Listdamagable = new List<IHurtBox>();

        // private Collider[] _damagables = new Collider[3];

        private AttackDataSend Atk_data ;
        // private float stoploopPersentage;
        private bool stoploop;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            // stoploopPersentage = stateData.frameChecker.percentageOnFrame(10);
            // stateData.frameChecker.initialize(this , stateMachineFX.m_ClientVisual.NormalAnimator,stateMachineFX.m_ClientVisual.spriteRenderer);
        }

        public override bool ShouldAnticipate(ref StateType requestData)        {
            if (requestData.Equals(StateType.Defense) ){
                // specialFX.ShutDownSlow();
                SendEndAnimation();
            }
            // stateData.frameChecker.CheckTransition(requestData);
            return true;
        }
        public override void Enter()        
        {
            if (!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();
        }


        public override StateType GetId()
        {
            return StateType.DUJ1;
        }


        public override void End()
        {
            // stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
            _Listdamagable = new List<IHurtBox>();
            stoploop = false;
            stateMachineFX.idle();

        }
        public override void Exit()
        {
            _Listdamagable = new List<IHurtBox>();
            stoploop = false;
            // stateMachineFX.m_ClientVisual.NormalAnimator.enabled = true;

        }


        public override void PlayAnim(int frameRender = 1, bool sequence = false)
        {
            base.PlayAnim();
            // timeNow = TimeStarted_Animation;
            // stateMachineFX.m_ClientVisual.NormalAnimator.enabled = false;
            // stateData.frameChecker.initCheck(frameRender);
            // stateMachineFX.m_ClientVisual.SetHitBox(true);

            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
        }

        public override void LogicUpdate()
        {
            if ( !stoploop && nbTickRender % stateData.DamageDetails[0].vrest == 0){
                foreach (IHurtBox damagable in _Listdamagable){
                    if (damagable != null && damagable.IsDamageable(stateMachineFX.m_ClientVisual.teamType)) {
                        damagable.ReceiveHP(Atk_data);
                    }
                }
            }
            if (nbTickRender > 4){
                stateMachineFX.CoreMovement.CustomMove_InputZ(stateData.Dx,stateData.Dz,stateMachineFX.InputZ);
            }
            
            if (nbTickRender %16 == 0 && !stoploop){
                stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1 , 0,0.333f);
            }
            if (stoploop){
                if (IsAnimationEnd()){
                    End();
                }
            }

        }

        public override void OnAnimEvent(int id){

            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);
            
        }



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




        public override void PlayEndAnimation()
        {
            stoploop = true;
        }

        private void SendEndAnimation()
        {
            if (stateMachineFX.m_ClientVisual.Owner)
            {
                stateMachineFX.m_ClientVisual.m_NetState.PlayEndAniamtion_SyncServerRpc(GetId());
            }
            stoploop = true;
        }





        public override void AddCollider(Collider collider)
        {
            IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();
            if (damagable != null)
            {
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
            if (damagable != null)
            {
                _Listdamagable.Remove(damagable);
            }
        }

        public void onMoveFrame(int frame)
        {
            stateMachineFX.CoreMovement.CustomMove_InputZ(stateData.frameChecker._frameStruct[frame].dvx, stateData.frameChecker._frameStruct[frame].dvz);
        }

        public void playAnimation(int frame)
        {
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1 , 0,stateData.frameChecker.percentageOnFrame(frame));
        }

        public void playSound(AudioCueSO audioCue)
        {
            
            stateMachineFX.m_ClientVisual.PlayAudio(audioCue);
        }

        public void onAttackFrame(AttackDataSend AttkData)
        {
            // int layer1 = 0; 
            // layer1 |= 1 << LayerMask.NameToLayer("HurtBox");
            // // Debug.Log("layer bitWise Or"+ layer1);
            // // Debug.Log("layer bitWise "+ (1 << LayerMask.NameToLayer("HurtBox")));
            // // Debug.Log("Layer Mask" + LayerMask.NameToLayer("HurtBox"));
            // // Debug.Log("Layer Mask" + LayerMask.GetMask(new[] { "HurtBox" }));

            // // mask |= (1 << s_PCLayer);
            // Vector3 center = stateMachineFX.m_ClientVisual.HitBox.transform.TransformPoint(stateMachineFX.m_ClientVisual.HitBox.bounds.center);
            // Debug.Log("local position center hit box" + stateMachineFX.m_ClientVisual.HitBox.bounds.center);
            // Debug.Log("world position center hit box" + center);
            // Debug.Log("position of the hit box" + stateMachineFX.m_ClientVisual.HitBox.transform.position);

            
            // int res =  Physics.OverlapBoxNonAlloc(stateMachineFX.m_ClientVisual.HitBox.transform.position ,
            //                         stateMachineFX.m_ClientVisual.HitBox.bounds.extents ,
            //                         _damagables,
            //                         Quaternion.identity,
            //                         layer1 );
                                    
            // AttkData.Direction = new Vector3 (AttkData.Direction.x * stateMachineFX.CoreMovement.GetFacingDirection(),AttkData.Direction.y , AttkData.Direction.z ) ;
            // Debug.Log(res);
            // for (int i = 0; i<res ;i++)
            // {
            //     IHurtBox damagable = _damagables[i].GetComponentInParent<IHurtBox>();
            //     if (damagable != null && damagable.NetworkObjectId != stateMachineFX.m_ClientVisual.NetworkObjectId && damagable.IsDamageable(stateMachineFX.m_ClientVisual.teamType))
            //     {
            //         damagable.ReceiveHP(AttkData);
            //     }
            // }
            foreach (IHurtBox damagable in _Listdamagable)
            {
                if (damagable != null && damagable.IsDamageable(stateMachineFX.m_ClientVisual.teamType))
                {
                    damagable.ReceiveHP(AttkData);
                }
            }
        }

    }

}
