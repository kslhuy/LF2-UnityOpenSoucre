using System.Collections.Generic;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "FirenDDJ", menuName = "StateLogic/Firen/Special/DDJ")]
    public class FirenDDJSO : StateLogicSO<FirenDDJLogic>
    {
        [SerializeField] private float TickRate = 1;
        [SerializeField] Vector3  DistanceBtw = new Vector3(10,0,0);


        protected override StateActionLogic CreateAction()
        {
            FirenDDJLogic FirenDUJ1 = new FirenDDJLogic();
            FirenDUJ1.tickRate = TickRate;
            FirenDUJ1.distanceBtw = DistanceBtw;
            return  FirenDUJ1;
        }

    }

    ///// Blaze : Chay ra lua  : MP 75

    public class FirenDDJLogic : LaunchProjectileLogic
    {
        public float tickRate;
        private float timeNow ;

        AttackDataSend attackDataSend;
        private SkillsDescription.ProjectileInfo projectileInfo;

        public Vector3 distanceBtw ;

        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            attackDataSend = new AttackDataSend();
            attackDataSend.Amount_injury = stateData.DamageDetails[0].damageAmount;
            attackDataSend.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;
            attackDataSend.BDefense_p = stateData.DamageDetails[0].Bdefend;
            attackDataSend.Fall_p = stateData.DamageDetails[0].fall;
                
            projectileInfo = GetProjectileInfo(stateData);
        }

        public override bool ShouldAnticipate(ref InputPackage requestData)
        {
            if (requestData.StateTypeEnum.Equals(StateType.Defense)){
                stateMachineFX.AnticipateState(StateType.Sliding);

            }
            return true;
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
            timeNow = Time.time;
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
            stateMachineFX.CoreMovement.CustomMove_InputZ(stateData.Dx , 0,stateMachineFX.InputZ);
            // if (stateMachineFX.m_ClientVisual._IsServer) {
            if (Time.time - timeNow >  tickRate){
                timeNow = Time.time;
                var playerPOS = stateMachineFX.CoreMovement.transform;

                var projectile = GameObject.Instantiate(projectileInfo.ProjectilePrefab, playerPOS.position + new Vector3(projectileInfo.pivot.x * stateMachineFX.CoreMovement.GetFacingDirection() ,projectileInfo.pivot.y,projectileInfo.pivot.z) ,playerPOS.rotation);
                projectile.GetComponent<ProjectileLogic>().Initialize(stateMachineFX.m_ClientVisual.NetworkObjectId,stateMachineFX.team ,new Vector3( stateMachineFX.CoreMovement.GetFacingDirection() ,0,0) );

                var projectile2 = GameObject.Instantiate(projectileInfo.ProjectilePrefab, projectile.transform.position + distanceBtw  + new Vector3(projectileInfo.pivot.x * stateMachineFX.CoreMovement.GetFacingDirection() ,projectileInfo.pivot.y,projectileInfo.pivot.z) ,playerPOS.rotation);
                projectile2.GetComponent<ProjectileLogic>().Initialize(stateMachineFX.m_ClientVisual.NetworkObjectId,stateMachineFX.team,new Vector3( stateMachineFX.CoreMovement.GetFacingDirection() ,0,0) );

                // SpwanProjectile(stateData, new Vector3(stateMachineFX.CoreMovement.GetFacingDirection(),0 ,stateMachineFX.InputZ));
                }
            
        }
        public override void OnAnimEvent(int id)
        {
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDJ_2);


        }
    }

}
