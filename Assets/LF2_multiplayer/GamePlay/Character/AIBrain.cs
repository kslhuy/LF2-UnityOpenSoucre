using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;
using System.Collections;

namespace LF2.Client
{
    /// <summary>
    /// Handles enemy AI. Contains StateAction that handle some of the details,
    /// and has various utility functions that are called by those StateAction
    /// </summary>

    public class AIBrain : NetworkBehaviour {
        

        public StateAction bestAction{get ; set ;}
        public StateAction bestMove{get ; set ;}

        // Event Send to ClientCharacterVisualiztion
        public Action<float, float> ActionMoveInputEvent { get; internal set; }

        public Action<StateType> ActionInputEvent { get; internal set; }
        public Action<StateType> PerformStateEvent { get; internal set; }

        private bool CollectEnemy ;
        
        private StateActionAI_SO stateActionAI_SO;
        private float timerCollectEnemy;

        public ClientCharacterVisualization TargetEnemy{get ; private set ;}
        

        public ClientCharacterVisualization Self;



        private List<ClientCharacterVisualization> m_HatedEnemies= new List<ClientCharacterVisualization>();

        private Transform self_transform;

        [SerializeField] Transform m_SenseTf;

        private int m_ExtendCheckBord;

        



        [Header("----- AI Debug ------")]

        
        [SerializeField]
        [Tooltip("If set to false, an NPC character will be denied its brain (won't attack or chase players)")]
        private bool m_BrainEnabled = true;

        [SerializeField] BoolEventChannelSO Event_ToggleAIBrain;
        
        [SerializeField]
        [Tooltip("If set to false, an NPC character will be denied its brain (won't attack or chase players)")]
        private bool AIDebugMode ;
        private bool _enableAsynAI;

        [SerializeField] private bool ShowScoreAll ;

        // Condition for Swiching mode Chase
        [SerializeField] 
        [Tooltip("If one enemy (PCs) is below this theshorld , so play mode chase enemy lowest HP ")]
        private int LowHP_Threshold = 250;
        [SerializeField] [Range(1,3)] private int _MethodeChoseFoe = 1;









        public struct VariablesNeed{
            public int xdistance  ;     //x distance (left > 0 > right)
            public int ydistance   ;   //y distance (above > 0 > below)
            public int zdistance  ;   //z distance
            public float xdir_distane   ; //directional distance
            public int target_mp   ;
            public int self_mp   ;
            public int zdistanceAbs;
        }

        public VariablesNeed variables;

        private Dictionary<StateType, float> m_LastUsedTimestamps;
        // private StateType stateToPlay;
        // private StateType[] values ;






        // private NPCController npc;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                //this component is not needed on the host (or dedicated server), because ServerCharacterMovement will directly
                //update the character's position.
                this.enabled = false;
            }            
            // values = (StateType[])Enum.GetValues(typeof(StateType));
            if (Event_ToggleAIBrain != null)  Event_ToggleAIBrain.ActionBool += ToggleAIBrain;

            self_transform = transform;
            m_LastUsedTimestamps = new Dictionary<StateType, float>();

            stateActionAI_SO = Self.m_NetState.CharacterClass.StateAI ;
            timerCollectEnemy = Time.time; 


        }

        private void ToggleAIBrain(bool toggle)
        {
            m_BrainEnabled = !m_BrainEnabled;
        }

        private void Update() {
            // collect enemy first time
            // every 5s , 
            // TODO : Need to make a event to trgger this when we spawn a new player , That will be better performant. 
            if (!CollectEnemy || timerCollectEnemy  + 5 < Time.time )  {
                CollectEnemy = true;
                TotalFoes();
                // Debug.Log($"m_HatedEnemies = {m_HatedEnemies}");
                }


            if (m_BrainEnabled && CollectEnemy && Self.m_NetState.LifeState != LifeState.Dead ){
                ////////  Research  enemy (Foe)   ////////
                var _selectedFoe = ChooseFoe();

                // Case all Player Are Died , So do nothing  

                if (_selectedFoe == null )      return;
               
                if ( TargetEnemy != _selectedFoe) TargetEnemy = _selectedFoe;
                
                variables = new VariablesNeed{
                    // distance between self + target
                    xdistance = (int)(_selectedFoe.transform.position.x - transform.position.x) ,
                    ydistance = Math.Abs((int)(_selectedFoe.transform.position.y - transform.position.y))  ,
                    zdistance = (int)(_selectedFoe.transform.position.z - transform.position.z),

                };
                // Debug.Log(variables.xdistance);
                variables.xdir_distane = variables.xdistance*Self.coreMovement.FacingDirection;
                variables.zdistanceAbs = Math.Abs(variables.zdistance);

                ///////    AI Here ////
                
                // // ++ Normal Logic //         // Not use : deprecated

                // if (egoAI() == 0 ){
                //     approach_opponent(_selectedFoe);
                //     if(opponent_close()){
                //         attack();
                //         }

                // }

                // // ++ New Logic with SOs //
                            // --- run with Asyn --- // 

                if (!_enableAsynAI){
                    _enableAsynAI = true;
                    RunBrain();
                }

                            // --- run in Normal Update Loop  --- // 
                // if (DecideBestAttack(A_AttackAvaillable) == 0 ){
                //     DecideBestMove(A_MoveAvaillable).Execute(this);
                //     }
                // else{
                //     bestAction.Execute(this);
                // }
                
            }
            
        }
#region Run Brain Asyn
        public async void RunBrain (){
            await EgoAI();
            _enableAsynAI = false;
        }

        private async Task EgoAI(){
            // LAm debug mode , Ko cho chuyen state nhung van de AI suy nghi chon state 
            // Tao mot dieu kien , ko cho chuyen state khi het thoi gian . Chi chuyen state khi call Change State or Aniticipate State 
                        
            if (DecideBestAttack(stateActionAI_SO.A_AttackAvaillable ) == 0 ){
                if (AIDebugMode){
                    Debug.Log($"Best Move {DecideBestMove(stateActionAI_SO.A_MoveAvaillable)} ");
                    return;
                }
                DecideBestMove(stateActionAI_SO.A_MoveAvaillable).Execute(this);
            }
            else{
                if (AIDebugMode){
                    Debug.Log($"Best Action {bestAction} ");
                    return;
                }
                bestAction.Execute(this);
            }
            await Task.Yield();


        }
#endregion

#region   Not use : deprecated

        // Not use : deprecated
        private void approach_opponent(ClientCharacterVisualization _selectedFoe)
        {
             // if we are not too close
            if(!range(0,5,variables.zdistanceAbs)||!range(0,65,variables.xdir_distane)){
                // if we are far engouh to run 
                if(Self.MStateMachinePlayerViz.CurrentStateViz.GetId() == StateType.Move &&!range(0,300,variables.xdir_distane) && facing_towards()){
                    Self.MStateMachinePlayerViz.ChangeState(StateType.Run);

                }
                //  ( we move )
                else {
                    var dir = new Vector2(variables.xdistance , variables.zdistance).normalized;
                    
                    ActionMoveInputEvent?.Invoke(dir.x , dir.y);
                    // Self.MStateMachinePlayerViz.ChangeState(StateType.Move);
                }
            }
        }

        // Not use : deprecated

        private int egoAI(){
            
            // DDJ
            // Checck in the range , 
            if(range(0,80,variables.xdir_distane)&&variables.zdistanceAbs<=17 ){Do(StateType.DDJ1);return 1;} // 

             //no mp combo

            //  variables.xdir_distane >= 0 : Not to close , in  right direction (forward enemy ) 
            // variables.zdistance > 60 +  variables.zdistance < 90 : Not too far  
            if ( variables.xdir_distane >= 0 &&  variables.zdistanceAbs <= 18){
                if (Self.MStateMachinePlayerViz.CurrentStateViz.GetId() == StateType.Idle ) {
                    Do(StateType.Attack);
                    Debug.Log("no mp combo"); 
                    return 1;}
            }

              //blasts
            if ( variables.xdir_distane >= 500 && variables.zdistanceAbs <= 40 && Self.MStateMachinePlayerViz.CurrentStateViz.GetId() == StateType.Idle ){Do(StateType.DDA1);}
            else if (variables.xdir_distane > 600 && variables.zdistanceAbs <= 40 && Self.MStateMachinePlayerViz.CurrentStateViz.GetId() == StateType.DDA1){Do(StateType.Attack);}
            else if (Self.MStateMachinePlayerViz.CurrentStateViz.GetId() == StateType.DDA1){return 1;}             
            return 0;
        }
        private void attack()
        {
            Do(StateType.Attack);
        }
    
#endregion




#region Helper Function
    
        public void Do(StateType state)
        {
            if (IsReusable(state)) {
                SendInput(state);
                m_LastUsedTimestamps[state] = Time.time;
            }
            // remember the moment when we successfully used this Action!

        }

        public bool IsReusable( StateType state){
            var _stateQueue = Self.MStateMachinePlayerViz.GetState(state);
            float reuseTime = _stateQueue.stateData.ReuseTimeSeconds;

            if (reuseTime > 0
                && m_LastUsedTimestamps.TryGetValue(state, out float lastTimeUsed)
                && Time.time - lastTimeUsed < reuseTime)
            {
                // we've already started one of these too recently
                return false;              // ... so it's important not to try to do anything more here
            }
            return true;
        }

        public void PerformStateDirect(StateType state){
            // if (IsReusable(state)) {
            // }
            PerformStateEvent?.Invoke(state );
            // SendInput(state);
            m_LastUsedTimestamps[state] = Time.time;
        }

        // NEed to split it out => Do it in separate Action State (A_Attack1 , A_Attack2 , etc ..  )
        private void SendInput( StateType state)
        {
            if (state == StateType.Attack)
            {
                int nbAimation = UnityEngine.Random.Range(1, 3);
                ActionInputEvent?.Invoke(state);
            }
            else
            {
                ActionInputEvent?.Invoke(state);

            }
        }

        public bool opponent_close(){
        //true if opponent in range
        //pass object number and ranges
            return (range(0,80,Math.Abs(variables.xdistance)))&&range(0,15,Math.Abs(variables.zdistance))?true:false;
        }

        public bool range(int min, int max, float i){
            //true if i is between min and max
            //make frame use the same form
            return (i>=min&&i<=max)?true:false;
        }
        public bool rangeAbs(int min, int max, float i){
            //true if i is between min and max
            //make frame use the same form
            return (Mathf.Abs(i)>=min&&Mathf.Abs(i)<=max)?true:false;
        }
        public bool facing_towards(){
            //true if self faces target
            //add variable objects
            return ((Self.coreMovement.FacingDirection)*variables.xdistance >0 ) ?true:false;
        }

 
        public bool facing_against(){
        //true if self and target face opposite directions
        //add variable objects
        return (Self.coreMovement.FacingDirection!=TargetEnemy.coreMovement.FacingDirection)?true:false;
        }
#endregion

    
    public void WalkRandomly(){
        //TODO : Add simple pathfinding here 
    }


#region  GetEnemy

// Refer to this logic 

    // number here is just an matrix 

   //1,0: closest opponent
   //1,1: closest opponent distance
   //2,0: second closest opponent
   //2,1: second closest opponent distance
   //3,0: weakest opponent
   //3,1: weakest opponent distance

   //4: closest boss
   //5: closest item
   //6: closest milk
   //7: closest beer

        protected void TotalFoes()
        {
            //  iterate all over the players have class NBplayer (even NPCs or PCs)
            // Debug.Log($"NbPlayer = {NbPlayer.GetPlayer().Count}"); 
            foreach (var character in NbPlayer.GetCharacter())
            {
                // Flitre out the NPCs , only take PCs
                if (IsAppropriateFoe(character) )
                {
                    Hate(character);

                }
            }
        }
    
        /// <summary>
        /// Picks the most appropriate foe for us to attack right now, or null if none are appropriate
        /// </summary>
        /// <returns></returns>
        private ClientCharacterVisualization ChooseFoe()
        {
            switch (_MethodeChoseFoe) {
                case 1 : 

                    // Closet Position methode 
                    ClientCharacterVisualization _selectedFoe = null;
                    float closestDistanceSqr = int.MaxValue;
                    Vector3 myPosition = self_transform.position;

                    
                    foreach (var foe in GetHatedEnemies())
                    {
                        if (foe.m_NetState.HPPoints < LowHP_Threshold){
                            _MethodeChoseFoe = 2;
                            // Debug.Log(" Switch to Chase enemy lowest HP ");
                        }
                        Vector3 closetFoePosition =  foe.PhysicsWrapper.Transform.position;

                        float distanceSqr = (myPosition - closetFoePosition).sqrMagnitude;
                        if (distanceSqr < closestDistanceSqr)
                        {
                            closestDistanceSqr = distanceSqr;
                            _selectedFoe = foe;
                        }
                    }
                    return _selectedFoe;

                case 2 :
                    // Lowest HP methode 
                    int lowestHP = 1000 ;
                    ClientCharacterVisualization foeLowestHP = null;
                    foreach (var foe in GetHatedEnemies()){
                        if (foe.m_NetState.HPPoints < lowestHP){
                            lowestHP = foe.m_NetState.HPPoints;
                            foeLowestHP = foe;
                        }
                    }
                    return foeLowestHP;

                default :
                    var foes = GetHatedEnemies();
                    return foes[0];
            }

        }

        /// <summary>
        /// Return the raw list of hated enemies -- treat as read-only!
        /// </summary>
        public List<ClientCharacterVisualization> GetHatedEnemies()
        {
            List<ClientCharacterVisualization> _temporaryHateEneimes = new List<ClientCharacterVisualization>();
            // first we clean the list -- remove any enemies that have disappeared (became null), are dead, etc.
            for (int i = m_HatedEnemies.Count - 1; i >= 0; i--)
            {
                if (IsAppropriateFoe(m_HatedEnemies[i]))
                {
                    _temporaryHateEneimes.Add(m_HatedEnemies[i]);
                    // Debug.Log(_temporaryHateEneimes);
                }
            }
            // Debug.Log(_temporaryHateEneimes);
            return _temporaryHateEneimes;
        }

        public bool IsAppropriateFoe(ClientCharacterVisualization potentialFoe)
        {
            if (potentialFoe == null ||
                potentialFoe.m_NetState.IsNpc ||
                potentialFoe.m_NetState.LifeState != LifeState.Alive ||
                potentialFoe.m_NetState.IsStealthy.Value)
            {
                return false;
            }

            // Also, we could use NavMesh.Raycast() to see if we have line of sight to foe?
            return true;
        }


        /// <summary>
        /// Notify the AIBrain that we should consider this character an enemy.
        /// </summary>
        /// <param name="character"></param>
        public void Hate(ClientCharacterVisualization character)
        {
            if (!m_HatedEnemies.Contains(character))
            {
                m_HatedEnemies.Add(character);
            }
        }

        
#endregion

    

#region CalculAIPoint
    

        // Loop Throgh all the availlable actions
        // Give me the highest scoring action
        public int DecideBestAttack(StateAction[] actionAvailable){
            float score = 0f;
            int nextBestActionIndex = 0;

            for (int i =0 ; i< actionAvailable.Length ; i++){
                Debug.Log($"{actionAvailable[i]} have  {ScoreAction(actionAvailable[i])} score");
                if (ScoreAction(actionAvailable[i]) > score){
                    nextBestActionIndex = i;
                    score = actionAvailable[i].Score;
                }
            }
            bestAction = actionAvailable[nextBestActionIndex];
            if (score < 0.3){
                // Debug.Log($"Score Low {score} for bestAction {bestAction}  ");
                return 0;   
            }else
            {
                return 1; 
            }
            
        }

        // Debug Only
        // Loop Throgh all the availlable actions
        // Give me the highest scoring action
        public int DecideBestAttack(StateActionDebug[] action){
            float score = 0f;
            int nextBestActionIndex = 0;

            for (int i =0 ; i< action.Length ; i++){
#if UNITY_EDITOR
                if (action[i].Disable) continue;
                if (ShowScoreAll){
                    if (action[i].ShowScore){
                        Debug.Log($"{action[i].A_Availlable.TypeName} have  {ScoreAction(action[i].A_Availlable)} score");
                    }
                }
#endif
                if (ScoreAction(action[i].A_Availlable) > score){
                    nextBestActionIndex = i;
                    score = action[i].A_Availlable.Score;
                }
            }
            bestAction = action[nextBestActionIndex].A_Availlable;
            if (score < 0.3){
                // Debug.Log($"Score Low {score} for bestAction {bestAction} , So Not Run");
                return 0;   
            }else
            {
                return 1; 
            }
            
        }

        // approach_opponent : Move toward or Move_away

        public int DecideBestMove(StateAction[] moveAvailable ){
            float score = 0f;
            int nextBestActionIndex = 0;

            for (int i =0 ; i< moveAvailable.Length ; i++){
                // Debug.Log($"{moveAvailable[i]} have  {ScoreAction(moveAvailable[i])} score");
                if (ScoreAction(moveAvailable[i]) > score){
                    nextBestActionIndex = i;
                    score = moveAvailable[i].Score;
                }
            }

            bestMove = moveAvailable[nextBestActionIndex];
            if (score < 0.2){
                // Debug.Log($"Score Low {score} for bestAction {bestAction} , So Not Run");
                return 0;   
            }else
            {
                return 1; 
            }
        }

        // Debug Only
        // if Diable all Action , So the return always is the first action of Move (see in AI editor) 
        public StateAction DecideBestMove(StateActionDebug[] actions ){
            float score = 0f;
            int nextBestActionIndex = 0;

            for (int i =0 ; i< actions.Length ; i++){
                // Debug.Log($"{moveAvailable[i]} have  {ScoreAction(moveAvailable[i])} score");
#if UNITY_EDITOR
                if (actions[i].Disable) continue;
                if (ShowScoreAll){
                    if (actions[i].ShowScore){
                        Debug.Log($"{actions[i].A_Availlable.TypeName} have  {ScoreAction(actions[i].A_Availlable)} score");
                    }
                }
#endif
                if (ScoreAction(actions[i].A_Availlable) > score){
                    nextBestActionIndex = i;
                    score = actions[i].A_Availlable.Score;
                }
            }
            return bestMove = actions[nextBestActionIndex].A_Availlable;
            // if (score < 0.2){
            //     // Debug.Log($"Score Low {score} for bestAction {bestAction} , So Not Run");
            //     return 0;   
            // }else
            // {
            //     return 1; 
            // }
        }
 


        // Loop throgh all the considerations of the action 
        // Score all the consideration 
        // Average the consideration scores  == > overall action score
        public float ScoreAction(StateAction action){
            float score_Main = 1f;
            float score_Sup = 0;
            int totalConsideraion = action.considerations.Length; 
#if UNITY_EDITOR	
            if (totalConsideraion == 0) {
                Debug.Log($"Miss Consideration in action {action}");
                return 0;
                }
#endif
            int counterNbSubConsideration = 0;  
            for (int c = 0 ; c < totalConsideraion ; c++){
                if (!IsReusable(action.TypeName)) return action.Score = 0;
                if (action.considerations[c].isSubConsideration){
                    counterNbSubConsideration++ ;
                    score_Sup += action.considerations[c].ScoreConsideration(this);
                    continue;
                }
                float considerationScore = action.considerations[c].ScoreConsideration(this);
                // if one consideration have 0 Score , that mean multiplie by 0 is 0 
                // So dont need to waste time more . 
                // Dont need check Sup Score also , 
                // because Main Score = 0 that mean no chance to play the Action 
                if (considerationScore == 0) return action.Score = 0;

                score_Main *= MakeUpScore(considerationScore , totalConsideraion) ;

            }
            // Add more Score_Sup in to ScoreMain and do some normalization (0,1)
            // Score_Sup is a bonus ,
            // this kind of "Sup Consideration" design for some special case 
            // (some thing like if ennemy is Davis do DDJ , so me (John) Do some thing to counter him )
            score_Sup = counterNbSubConsideration > 0 ? (score_Sup / counterNbSubConsideration) : 0;
            float score_Total = score_Main + score_Sup; 
            score_Main = score_Total/(score_Total+1); 

            return action.Score = score_Main;
        }

        private float MakeUpScore(float score_Main , int nbTotConsideration ){
            // Make up one score of a Consideration , so overrall can be higher 
            // Action More Considerations so more difficulty to perform than Action have less consideration 
            float originalScore = score_Main;
            float modFactor = 1 - (1/nbTotConsideration);

            float makeupValue = (1 - originalScore)*modFactor;

            return originalScore + (makeupValue * originalScore);
        } 

#endregion

//                 // Loop throgh all the considerations of the action 
//         // Score all the consideration 
//         // Average the consideration scores  == > overall action score
//         public float ScoreAction(StateAction action){
//             float score = 1f;
// #if UNITY_EDITOR	
//             if (action.considerations.Length == 0) {
//                 Debug.Log($"Miss Consideration in action {action}");
//                 return 0;
//                 }
// #endif

//             for (int c = 0 ; c < action.considerations.Length ; c++){
//                 float considerationScore = action.considerations[c].ScoreConsideration(this);
//                 score *= considerationScore;

//                 if (score == 0){
//                     action.Score = 0;
//                     return action.Score;
//                 }
//             }

//             // Aveaging scheme of overalle score
//             // Action More Considerations so more difficulty to perform than Action have less consideration 
//             float originalScore = score;
//             float modFactor = 1 - (1/action.considerations.Length);

//             float makeupValue = (1 - originalScore)*modFactor;

//             action.Score = originalScore + (makeupValue * originalScore);

//             return action.Score;
//         }

        /// <summary>
        /// Returns true if it be appropriate for us to murder this character, starting right now!
        /// </summary>

    }
}


