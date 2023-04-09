using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "DavisSO4", menuName = "StateLogic/Davis/Special/DavisSO4")]
    public class DavisSO4 : StateLogicSO<DavisSO4Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
// Projectle normal == Deep DDA
    public class DavisSO4Logic : MeleLogic , IFrameCheckHandler
    {
        private bool cantransition_ToNextAnimation; // Or mean Next State
        private bool m_Launched;
        private bool frameTransitionAnim;

        private int countDDA = 1;

        public override void Awake(StateMachineNew stateMachine)
        {

            stateMachineFX = stateMachine;
            stateData.frameChecker.initialize(this , stateMachineFX.m_ClientVisual.NormalAnimator,stateMachineFX.m_ClientVisual.spriteRenderer);
        }


         public override bool ShouldAnticipate(ref StateType requestData)        {
            if ( requestData == StateType.Attack || requestData == StateType.Attack2){
                cantransition_ToNextAnimation = true;
                return true;
            }
            // For Debug Only
            if (requestData == StateType.Defense){
                stateMachineFX.idle();
            }
            return false;
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
            return StateType.DDA1;
        }
        public override void LogicUpdate(){   

        }


        public override void End(){
        }


        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();

            stateMachineFX.m_ClientVisual.NormalAnimator.enabled = false;
            stateData.frameChecker.initCheck(1);
        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }


        public void onMoveFrame(int frame)
        {
            stateMachineFX.CoreMovement.CustomMove_InputZ(stateData.frameChecker._frameStruct[frame].dvx, stateData.frameChecker._frameStruct[frame].dvz);
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

            // foreach (IHurtBox damagable in _Listdamagable)
            // {
            //     if (damagable != null && damagable.IsDamageable(stateMachineFX.m_ClientVisual.teamType))
            //     {
            //         damagable.ReceiveHP(AttkData);
            //     }
            // }
            stateMachineFX.m_ClientVisual.SetHitBox(true);
        }

    }
}
