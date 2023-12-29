using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LF2.Utils;
using UnityEngine;


namespace LF2.Client
{


	public class StateMachineNew 
	{


        public StateActionLogic CurrentStateViz {get ; private set;} // CurrentState visual we are 

        public StateActionLogic LastStateViz {get ; private set;}

        
        public ClientCharacterVisualization m_ClientVisual ;

        public ClientCharacterMovement CoreMovement  ;

        public InteractionZone itr {get ; private set;}


        // INPUT 
        public float InputX;
        public float InputZ;
        // INPUT 
        public bool IsMove { get; private set; }

        // General Cache Value 
        // public int nbHit = 1;
        public int nbJump;

        //

        public TeamType team {get ;private set;}

        public CalculStatics calculStatics;

        private Dictionary<StateType, StateActionLogic> m_StateLogicaMap ;
        
        private Dictionary<EnumPair, StateActionLogic> m_State_Skill_Dict ;





        // Stats 
        // private ClientInputSender inputSender;



        // private bool IsPlayAnimation_once;

        private int currentFrameNumber;

        private bool _onceEnd;
        public List<StateActionLogic> QueueStateFX = new List<StateActionLogic>();
        public CharacterStateSOs CharacterAllStateSOs;

        public StateMachineNew(ClientCharacterVisualization parentViz , CharacterStateSOs characterStateSOs  )
        {

            CharacterAllStateSOs = characterStateSOs ;
            // Debug.Log(CharacterAllStateSOs);
            m_ClientVisual = parentViz;
            CoreMovement = parentViz.coreMovement; 
            itr = parentViz.InteractionZone;

            calculStatics = new CalculStatics(parentViz.m_NetState.CharacterClass.BaseAmor);
            calculStatics.BaseAmor = parentViz.m_NetState.CharacterClass.BaseAmor;

            team = parentViz.teamType;
            m_StateLogicaMap = new Dictionary<StateType, StateActionLogic>();
            m_State_Skill_Dict = new Dictionary<EnumPair, StateActionLogic>();

            ///
            currentFrameNumber = 0;
            // currentTimerTU = Time.time;
  
            CurrentStateViz = m_StateLogicaMap[StateType.Idle] = CharacterAllStateSOs.StateLogicSOsByType[StateType.Idle].GetAction(this);
        }


        
      
        // Do convert enum StateType == > State corresponse 
        public StateActionLogic GetState (StateType stateType , SkillNumber skillNumber = SkillNumber.Skill_1 ){
            EnumPair key = new EnumPair(stateType, skillNumber);

            // Normal State 
            if (stateType is not (StateType.Combo1 or  StateType.Combo2 or StateType.Combo3  or StateType.Combo4) ){

                // if already instantied so return 
                if ( m_StateLogicaMap.TryGetValue(stateType , out StateActionLogic value)){
                    return value;
                }else {

                    if (CharacterAllStateSOs.StateLogicSOsByType.TryGetValue(stateType , out StateLogicSO stateLogicSO)){
                        m_StateLogicaMap[stateType] = stateLogicSO.GetAction(this);
                        return m_StateLogicaMap[stateType];            
                    }                
                    // Try to get it in IStateLogicSO from CharacterStates
                    else{
                        // May not have this state
                        Debug.Log($"Tried to find State {stateType} but it was missing from CharacterStates!");
                        return m_StateLogicaMap[StateType.Idle];            
                    }
                }         
            }

            // Combo in Skill 
            else
            {
                if (!m_State_Skill_Dict.ContainsKey(key))
                {
                    StateLogicSO stateLogicSo = CharacterAllStateSOs.Get_StateLogicSO(stateType,skillNumber);
                    if (stateLogicSo == null){
                        Debug.Log($"Tried to find State {stateType} but it was missing from CharacterStates!");
                        return m_StateLogicaMap[StateType.Idle];      
                    }
                    m_State_Skill_Dict[key] = stateLogicSo.GetAction(this);
                }

                // Return the PowerPlayer instance from the dictionary
                return m_State_Skill_Dict[key];
            }
        
        
        }
     
            
        // Aticipate State in CLient , 
        // Do change state but most importance its run Animation predict   
        public void ShouldChangeState(ref StateType requestData)
        {
            // Debug.Log($"Currenstate {CurrentStateViz} ");
            CurrentStateViz.ShouldAnticipate(ref requestData);   
        }

        public void SaveMoveInput( sbyte inputX ,  sbyte inputZ  )
        {
            // Debug.Log($"inputX = {inputX}");
            InputX = inputX;
            InputZ = inputZ;
            StateType state ;
            if (inputX  != 0 || inputZ  != 0 ) 
                state = StateType.Move;
            else 
                state = StateType.Idle;

            ShouldChangeState(ref state);


        }

        // Play correct State that sent by Server  (broad-cast to all client)
        public void PerformSyncStateFX(ref StateType stateTypeSync , ref SkillNumber skillNumber )
        {
                // Debug.Log("Change State");
            if (CurrentStateViz.GetId().Equals(stateTypeSync)) return;
            
            // Warring : 22/09/2023 ?? WTF is that 
            if (CurrentStateViz.stateData.Dy > 10) {
                if (stateTypeSync == StateType.Land || stateTypeSync == StateType.Crouch ||  stateTypeSync == StateType.Idle ){
                    // Debug.Log( "Return car StateType is Land");
                    return;
                }
            }
            // if (stateTypeSync == StateType.Land || stateTypeSync == StateType.Crouch ||  stateTypeSync == StateType.Idle ){
            //     // Debug.Log( "Return car StateType is Land");
            //     return;
            // } 
                
            // Debug.Log("Change State 2");
            ChangeState(stateTypeSync , skillNumber);        

            
        }


        // Play correct State that sent by Server  (broad-cast to all client)
        public void PlayEndAnimationFX(ref StateType stateTypeSync)
        {
            // Debug.Log("Change State");
            if (CurrentStateViz.GetId().Equals(stateTypeSync)) {
                CurrentStateViz.PlayEndAnimation();
            }
            
            
        }





        /// Every frame:  Check current Animation to end the animation , 
        // If recevie request form Server can active  OnUpdate() of this State

        public void OnUpdate() {
            // // Update Status of player
            
            if (currentFrameNumber % 2 == 0){
                // currentTimerTU = Time.time;
                calculStatics.FixedUpdate();

            }

            // m_ClientVisual.textMesh.text = calculStatics.Current_Fall.ToString();

            // Check ALL State that have actual Action correspond ( See in Game Data Soucre Objet )
            // Debug.Log(CurrentStateViz);
            if (CurrentStateViz.GetId() == StateType.Idle) {

                    // Cast Skill in queue first when we just arrive in Idle State   
                if (QueueStateFX.Count > 0 ) {
                    AnticipateState(QueueStateFX[0].GetId(),SkillNumber.Skill_1,1,true);
                    QueueStateFX.RemoveAt(0);
                    return;
                } 
                CurrentStateViz.LogicUpdate();
                CurrentStateViz.nbTickRender++;
                return;
            }

            if (CurrentStateViz.GetId() == StateType.Move){
                LastStateViz = CurrentStateViz;
                CurrentStateViz.LogicUpdate();
                return;
            }

            if ( LastStateViz != CurrentStateViz){
                LastStateViz = CurrentStateViz;
            }

            
            //// Use full , take value from ref of SOs , so we can change value in real time
            if (m_ClientVisual.StateChange_After_Timer){
                // Debug.Log("Huy");
                if ( CurrentStateViz.stateData.expirable)
                {
                    // Debug.Log($"Sub_TimeAnimation = {Time.time -  CurrentStateViz.TimeStarted_Animation} "); 
                    // bool timeExpired = Time.time -  CurrentStateViz.TimeStarted_Animation >= CurrentStateViz.stateData.DurationSeconds;
                    bool timeExpired =  CurrentStateViz.nbTickRender >= CurrentStateViz.stateData.Duration;
                    
                    // Check if this State Can End Naturally (== time Expired )
                    if ( timeExpired ){
                        if (CurrentStateViz.GetId() == StateType.Land){
                            CurrentStateViz?.End();
                            return;
                        }
                        if (!_onceEnd){
                            _onceEnd = true;
                            // Debug.Log("End");
                            CurrentStateViz?.End();
                            return;
                            }
                        
                        return;
                    }
                }
            }
            CurrentStateViz?.LogicUpdate();
            CurrentStateViz.nbTickRender++;
            currentFrameNumber++;


            

        }

        // ID 300 = Play sound
        public void OnAnimEvent(int id)
        {
            CurrentStateViz.OnAnimEvent(id);
        }


        // Switch to Another State , (we force to Change State , so that mean this State may be not End naturally , be interruped by some logic  ) 
        ///  Only exucute when server call !!!!
        public void ChangeState( StateType state ,SkillNumber skillNumber = SkillNumber.Skill_1 , int frameRender = 1,bool combo = false ){
            _onceEnd = false;
            var stateAction = GetState(state , skillNumber);

            if (m_ClientVisual.debugUseMana){
                if (stateAction.stateData.ManaCost < 0){
                    Debug.LogWarning("You Forgot to Set Mana Cost");

                }
                if (stateAction.stateData.ManaCost < m_ClientVisual.MPRemain())
                {
                    m_ClientVisual.MPChange(stateAction.stateData.ManaCost);
                    CurrentStateViz.Exit();
                    CurrentStateViz = combo? QueueStateFX[0]: stateAction;
                    CurrentStateViz.nbTickRender = frameRender;
                    CurrentStateViz.Enter();
                }
            }else{
                CurrentStateViz.Exit();
                CurrentStateViz = combo? QueueStateFX[0]: stateAction;
                CurrentStateViz.nbTickRender = frameRender;
                CurrentStateViz.Enter();
            }
        }

        // Switch to Another State , (we force to Change State , so that mean this State may be not End naturally , be interruped by some logic  ) 
        ///  Only exucute when server call !!!!
        public void ChangeState( StateActionLogic stateAction , int frameRender = 1,bool combo = false ){
            _onceEnd = false;

            if (m_ClientVisual.debugUseMana){
                if (stateAction.stateData.ManaCost < 0){
                    Debug.LogWarning("You Forgot to Set Mana Cost");

                }
                if (stateAction.stateData.ManaCost < m_ClientVisual.MPRemain())
                {
                    m_ClientVisual.MPChange(stateAction.stateData.ManaCost);
                    CurrentStateViz.Exit();
                    CurrentStateViz = combo? QueueStateFX[0]: stateAction;
                    CurrentStateViz.nbTickRender = frameRender;
                    CurrentStateViz.Enter();
                }
            }else{
                CurrentStateViz.Exit();
                CurrentStateViz = combo? QueueStateFX[0]: stateAction;
                CurrentStateViz.nbTickRender = frameRender;
                CurrentStateViz.Enter();
            }
        }

        // public void AnticipateState(StateType state,int frameRender = 1, bool combo = false ){
        //     _onceEnd = false;
        //     var stateAction = GetState(state);


        //     if (m_ClientVisual.debugUseMana){
        //         // if (stateAction.stateData.ManaCost > 0){
        //         //     Debug.LogWarning("You Forgot to Set Mana Cost");
        //         // }
        //         if (stateAction.stateData.ManaCost < m_ClientVisual.MPRemain())
        //         {
        //             m_ClientVisual.MPChange(stateAction.stateData.ManaCost);
        //             // Exit current state
        //             CurrentStateViz.Exit();
        //             // assigne new stat to CurrentState
        //             CurrentStateViz = combo? QueueStateFX[0]: stateAction;
        //             CurrentStateViz.nbTickRender = frameRender;
        //             if (m_ClientVisual.Owner){
        //                 m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(state);
        //             }

        //             CurrentStateViz.PlayAnim(frameRender);

        //             // CurrentStateViz.PlayPredictState();
        //             // Debug.Log($"You have {m_ClientVisual.MPRemain()} remain");

        //         }
        //     }else {
        //         CurrentStateViz.Exit();
        //         CurrentStateViz = combo? QueueStateFX[0]: stateAction;
        //         CurrentStateViz.nbTickRender = frameRender;
        //         if (m_ClientVisual.Owner){
        //             m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(state);
        //         } 
        //         CurrentStateViz.PlayAnim(frameRender);
        //     }
        // }

        public void AnticipateState( StateType state , SkillNumber skillNumber = SkillNumber.Skill_1, int frameRender = 1, bool combo = false ){
            _onceEnd = false;
            // var stateAction = GetState(state);

            var stateAction = GetState(state , skillNumber);

            if (m_ClientVisual.debugUseMana){
                // if (stateAction.stateData.ManaCost > 0){
                //     Debug.LogWarning("You Forgot to Set Mana Cost");
                // }
                if (stateAction.stateData.ManaCost < m_ClientVisual.MPRemain())
                {
                    m_ClientVisual.MPChange(stateAction.stateData.ManaCost);
                    // Exit current state
                    CurrentStateViz.Exit();
                    // assigne new stat to CurrentState
                    CurrentStateViz = combo? QueueStateFX[0]: stateAction;
                    CurrentStateViz.nbTickRender = frameRender;
                    if (m_ClientVisual.Owner){
                        m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(state);
                    }

                    CurrentStateViz.PlayAnim(frameRender);

                    // CurrentStateViz.PlayPredictState();
                    // Debug.Log($"You have {m_ClientVisual.MPRemain()} remain");

                }
            }else {
                CurrentStateViz.Exit();
                CurrentStateViz = combo? QueueStateFX[0]: stateAction;
                CurrentStateViz.nbTickRender = frameRender;
                if (m_ClientVisual.Owner){
                    m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(state);
                } 
                CurrentStateViz.PlayAnim(frameRender);
            }
        }


        


        
        /// <summary>
        /// Tells all active Actions that a particular gameplay event happened, such as being hit,
        /// getting healed, dying, etc. Actions can change their behavior as a result.
        /// </summary>
        /// <param name="activityThatOccurred">The type of event that has occurred</param>
        public void OnHurtReponder(ref AttackDataSend attkdata)
        {
            
            if (attkdata.Amount_injury > 0) return;
            StateType nextState = GetActionReact(attkdata );
            // Debug.Log($"nextState : {nextState}");            

            /* --------- Effect , Sound -------- */              


            if (nextState == StateType.AmorAbsord){
                // m_ClientVisual.VibrationHitLag(attkdata.Direction.x);
                m_ClientVisual.PlayAudioHit(5);
                return;
            }
            else if (nextState == StateType.DefenseHit){
                // m_ClientVisual.VibrationHitLag(attkdata.Direction.x);
                m_ClientVisual.PlayAudioHit(6);
                return;
            }
            else {
                m_ClientVisual.PlayAudioHit(attkdata.Effect);
                // Debug.Log(attkdata.Amount_injury);
                m_ClientVisual.DamgePopUp(m_ClientVisual.PhysicsWrapper.Transform.position ,attkdata.Amount_injury);                
                
                /*  ---------------  Calcul HP ------------------- */ 
                if (m_ClientVisual.Owner && attkdata.Amount_injury != 0 ){
                    m_ClientVisual.m_NetState.HPChangeServerRpc(attkdata.Amount_injury);
                } 

            }


            if (CurrentStateViz.GetId() != StateType.Ice){
                AnticipateState(nextState);
            }

            // owner apply direct
            // if (m_ClientVisual.Owner)  {
            // }
            if (attkdata.Direction != Vector3.zero) {
                // Debug.Log(attkdata.Direction);
                m_ClientVisual.coreMovement.TakeControlTransform(true);
            }
            CurrentStateViz.HurtResponder(attkdata.Direction);

            // if (m_ClientVisual.Owner){
            //     Debug.Log("owner be hurted recveied direction" + attkdata.Direction); 
            // }


            // if (!m_ClientVisual.Owner){
            //     Debug.Log("not owner be hurted recveied direction" + attkdata.Direction); 
            // }
        }

        private StateType GetActionReact(AttackDataSend attkdata )
        {

            // Debug.Log( "direction x" + (int)attkdata.Direction.x);
            // Can only block the attack when me and enmy look at opposite direction
            // ->    <- 
            int eFacing = (int)(Mathf.Abs(attkdata.Direction.x)/attkdata.Direction.x);
            bool opositeDir = eFacing != CoreMovement.GetFacingDirection();
            
            if (attkdata.Effect == (int)DamageEffect.Fire ) {
                calculStatics.UpdateBdefend(attkdata.BDefense_p) ;
                calculStatics.UpdateFall(attkdata.Fall_p);
                return StateType.Fire ;
            }
            if (attkdata.Effect == (int)DamageEffect.Ice) {
                calculStatics.UpdateBdefend(attkdata.BDefense_p) ;
                calculStatics.UpdateFall(attkdata.Fall_p);
                return StateType.Ice ;
            
            }

            // Debug.Log( " HP " + m_ClientVisual.HPRemain() + ":" + "damage"  + attkdata.Amount_injury  );
            if (m_ClientVisual.HPRemain() < -attkdata.Amount_injury){
                return opositeDir? StateType.FallingFront : StateType.FallingBack  ;
            }

            if (calculStatics.AmorLeft(attkdata.BDefense_p) > 0){
                return StateType.AmorAbsord;
            }


            if (CurrentStateViz.GetId() == StateType.Defense && opositeDir)
            {

                return  calculStatics.BdefenseLeft(attkdata.BDefense_p, opositeDir);
            }

            return (calculStatics.FallLeft(attkdata.Fall_p, opositeDir,!CoreMovement.IsGounded() , attkdata.BDefense_p ,CurrentStateViz.GetId() ));
        }

        public void idle()
        {
            CoreMovement.ResetVelocity();
            nbJump = 0;
            ChangeState(StateType.Idle);
        }

        public void OnTriggerEnter(Collider collider) {
            CurrentStateViz?.AddCollider(collider);
        }

        // public void OnTriggerStay(Collider collider) {
        //     CurrentStateViz?.OnStayCollider(collider);
        // }
        public void OnTriggerExit(Collider collider) {
            CurrentStateViz?.RemoveCollider(collider);
        }

        // public void OnCollisionEnter(Collision collider) {
        //     CurrentStateViz?.AddCollision(collider);
        // }


        public bool IsAnimating()
        {
            if (m_ClientVisual.NormalAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle_anim")) {
                return true;
            }
            m_ClientVisual.NormalAnimator.Play("Idle_anim");
            return false;


            // if (NormalAnimator.GetFloat(m_VisualizationConfiguration.SpeedVariableID) > 0.0) { return true; }

            // for (int i = 0; i < NormalAnimator.layerCount; i++)
            // {
            //     if (NormalAnimator.GetCurrentAnimatorStateInfo(i).tagHash != m_VisualizationConfiguration.BaseNodeTagID)
            //     {
            //         //we are in an active node, not the default "nothing" node.
            //         return true;
            //     }
            // }

            // return false;


        }


        


    


    
    
    
}

}
