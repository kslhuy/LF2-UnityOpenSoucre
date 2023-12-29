using UnityEngine;
namespace LF2.Client{

    [CreateAssetMenu(fileName = "JohnDUA1", menuName = "StateLogic/John/Special/DUA1")]
    public class JohnDUA1SO : StateLogicSO<JohnDUA1Logic>
    {

        protected override StateActionLogic CreateAction()
        {
            return base.CreateAction();
        }
    }
    //// Heal 
    public class JohnDUA1Logic : StateActionLogic
    {


        protected AttackDataSend Atk_data ;
        private StateType _stateToPlay;
        public override void Awake(StateMachineNew stateMachine)
        {
            stateMachineFX = stateMachine;
            Atk_data = new AttackDataSend();
            Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
                       Atk_data.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;

            Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
            Atk_data.Fall_p = stateData.DamageDetails[0].fall;     

        }

         // Can switch to DUA2 or DUA3  direcly
        public override bool ShouldAnticipate(ref StateType requestData)        {


            // For Debug Only
            if (requestData == StateType.Defense){
                stateMachineFX.idle();
            }
            return false;
        }


        public override void Enter()        {
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();

        }


        //// Heal 



        public override void End(){

            stateMachineFX.idle();
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            base.PlayAnim();
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_DUA_1);
            // Remenber set Amount Heal = Amount Damage in SOs with positive number
            stateMachineFX.m_ClientVisual.ReceiveHP(Atk_data);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);

        }

        public override void PlayPredictState( int nbanim = 1 , bool sequence = false)
        {
            
            if (stateMachineFX.m_ClientVisual.Owner) {
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }

            PlayAnim(nbanim , sequence);
        }
        public override StateType GetId()
        {
            return stateData.StateType;
        }

        public override void OnAnimEvent(int id)
        {
            if (stateData.SpawnsFX.Length == 0) {
                Debug.LogWarning($"You forgot set SFX for state {GetId()} John");
                return;
            }
            // VFX so All client call be call
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Start_Sounds[0]);
            GameObject.Instantiate(stateData.SpawnsFX[0]._Object, stateMachineFX.CoreMovement.transform );


        }



    }

}
