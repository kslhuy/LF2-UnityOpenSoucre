using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "WoodyDDJ", menuName = "StateLogic/Woody/Special/DDJ")]
    public class WoodyDDJSO : StateLogicSO<WoodyDDJLogic>
    {
        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }

    ///// dich chuyen 
    public class WoodyDDJLogic : StateActionLogic
    {
        List<ClientCharacterVisualization> nbCharacterViz = new List<ClientCharacterVisualization>();

        Vector3 closetFoePosition;

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
            return stateData.StateType;
        }

        public override void End(){
            stateMachineFX.idle();
        }



        public override void PlayAnim( int nbanim = 1 , bool sequen = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDJ_1);
        }
		public override void LogicUpdate(){
            stateMachineFX.CoreMovement.RB.velocity = Vector3.zero;
        }

        

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            nbCharacterViz = new List<ClientCharacterVisualization>();
            // Look throught all active PCs or NPCs 
            foreach (ClientCharacterVisualization viz in NbPlayer.GetCharacter()){
                if (viz.NetworkObjectId != stateMachineFX.m_ClientVisual.NetworkObjectId && viz.IsDamageable(stateMachineFX.m_ClientVisual.teamType)){
                    // Debug.Log("clientCharacter" + viz);
                    nbCharacterViz.Add(viz);
                }
            }
            // Find the closest enemy
            if (nbCharacterViz.Count > 0){

                // ClientCharacterVisualization _selectedFoe = null;
                float closestDistanceSqr = int.MaxValue;
                Vector3 myPosition = stateMachineFX.CoreMovement.transform.position;
                
                foreach (var foe in nbCharacterViz){
                    closetFoePosition =  foe.PhysicsWrapper.Transform.position;
                    float distanceSqr = (myPosition - closetFoePosition).sqrMagnitude;
                    if (distanceSqr < closestDistanceSqr)
                    {
                        closestDistanceSqr = distanceSqr;
                        // _selectedFoe = foe;
                    }

                }
            }
            else{
                closetFoePosition = stateMachineFX.CoreMovement.transform.position;
            }


            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbanim , sequence);
        }

 
        public override void OnAnimEvent(int id)
        {
            ///// Shiled  : only spawn
            stateMachineFX.CoreMovement.TeleportPlayer(closetFoePosition + Vector3.right*stateData.Dx*-stateMachineFX.CoreMovement.GetFacingDirection());
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DDJ_2);

        }



    }

}
