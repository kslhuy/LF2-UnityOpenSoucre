using System.Collections.Generic;
using LF2.Utils;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "HenryDUJ1", menuName = "StateLogic/Henry/Special/DUJ1")]
    public class HenryDUJ1SO : StateLogicSO<HenryDUJ1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            HenryDUJ1Logic HenryDUJ1 = new HenryDUJ1Logic();
            return  HenryDUJ1;
        }
    }

// // Sonata of the Death,  MP = 70
    public class HenryDUJ1Logic : LaunchProjectileLogic
    {

        private List<IHurtBox> _Listdamagable = new List<IHurtBox>();
        // private float timeNow ;
        // private float timeStart;
        AttackDataSend Atk_data = new AttackDataSend();
        private int indexSounds;
        // private bool m_Started;
        // private bool canHitCreator;
        // private ulong m_SpawnerId;
        // private SkillsDescription.ProjectileInfo m_ProjectileInfo;
        // private int m_CollisionMask;  //mask containing everything we test for while moving

        private SpecialFXGraphic m_SpawnedGraphics = null;



        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;

            Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
            Atk_data.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;
            Atk_data.Effect = ((byte)stateData.DamageDetails[0].Effect);
            Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
            Atk_data.Fall_p = stateData.DamageDetails[0].fall;            
        }

        public override bool ShouldAnticipate(ref StateType requestData)        {
            if(requestData == StateType.Defense){
                stateMachineFX.AnticipateState(StateType.Idle);
            }
            return true;
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
            return StateType.DUJ1;
        }


        public override void End(){
            m_SpawnedGraphics.Shutdown();
            indexSounds = 0;
            _Listdamagable.Clear();
            stateMachineFX.idle();

        }

        public override void PlayAnim(int nbAniamtion = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUJ_1);
                        // Spwan just some effet for visual 
            m_SpawnedGraphics = InstantiateSpecialFXGraphic( stateData.SpawnsFX[0]._Object, stateMachineFX.m_ClientVisual.PhysicsWrapper.Transform ,Vector3.zero ,true, false,GetId());

            
        }
        public override void LogicUpdate()
        {
            if (nbTickRender % stateData.DamageDetails[0].vrest == 0){
                foreach (IHurtBox damagable in _Listdamagable){
                    if (damagable != null && damagable.IsDamageable(stateMachineFX.team)) {
                        damagable.ReceiveHP(Atk_data);
                    }
                }
            }
            // if (stateMachineFX.m_ClientVisual._IsServer) {
            // }
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
            if (indexSounds>= stateData.Start_Sounds.Length ) indexSounds = 0;
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[indexSounds]);
            indexSounds++;
        }


        public override void AddCollider (Collider collider)
        {
            IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();
            if (damagable != null){
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


    }

}
