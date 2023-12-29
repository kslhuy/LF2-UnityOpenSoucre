using System.Collections.Generic;
// using Cinemachine;
using Unity.Netcode;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using System.Collections;
using LF2.ObjectPool;
using LF2.Utils;

namespace LF2.Client
{
    /// <summary>
    /// <see cref="ClientCharacterVisualization"/> is responsible for displaying a character on the client's screen based on state information sent by the server.
    /// </summary>

    public class ClientCharacterVisualization : NetworkBehaviour 
    {
        [SerializeField]
        private Animator m_VisualsNormalAnimator;

        CharacterSwap m_CharacterSwapper;


        private ClientInputSender inputSender;

        [SerializeField] private BoxCollider _hitBox;
        [SerializeField] private BoxCollider _hurtBox;

        // public BoxCollider HitBox => _hitBox;

        private Vector3 _InitSizeHurtBox;
        private Vector3 _InitCenterHurtBox;





        

        /// <summary>
        /// Returns a reference to the active Animator for this visualization
        /// </summary>
        public Animator NormalAnimator { get { return m_VisualsNormalAnimator; } }
        

        public bool CanPerformActions { get { return m_NetState.CanPerformActions; } }

        public Transform Parent { get; private set; }

        PhysicsWrapper m_PhysicsWrapper;

        public PhysicsWrapper PhysicsWrapper => m_PhysicsWrapper ;

        [SerializeField]
        public ClientCharacterMovement coreMovement  ;

        [SerializeField] public InteractionZone InteractionZone;



        public NetworkCharacterState m_NetState;

        public TeamType teamType{get ; private set;} 
        // public ulong NetworkObjectId => m_NetState.NetworkObjectId;

        public TextMeshPro textMesh;

        [field:SerializeField] public SpriteRenderer spriteRenderer {get ; private set;}

        public VisualizationConfiguration VizAnimation;

        [Header("----- Audio Event ------")]

        [SerializeField] private AudioCueEventChannelSO _sfxEventChannel = default;
        [SerializeField] private AudioConfigurationSO _audioConfig = default;

        [SerializeField] private AudioManagerHits _audioHits;

        [Header("----- DamgePopUp Event ------")]

        [SerializeField] private DamagePopupEventChannelSO _damagePopUpChannel;
        // End Header 

        private Coroutine  playState_Coro;


        public StateMachineNew MStateMachinePlayerViz{ get; private set; }
        private CharacterStateSOs characterStateSOs {
            get
            {
                return m_NetState.CharacterStateSO;
                // CharacterStateSOs result;
                // var found = GameDataSourceNew.Instance.AllStateCharacterByType.TryGetValue(m_NetState.CharacterType, out result);
                // // Debug.Log(result);
                // Debug.AssertFormat(found, "Tried to find Character but it was missing from GameDataSource!");
                // return result;
            }
        }
        public StateType LastStateViz { get; private set; }
        private RectTransform UirectTransform ;

        /// Player characters need to report health changes and chracter info to the PartyHUD
        // [SerializeField] SpriteRenderer TriangleUI;

        //// --- NPC ---
        private AIBrain m_aiBrain;

        public bool IsNPC => m_NetState.IsNpc;

        //// --- NPC ---
        public bool Owner{ get; private set; } 

        private bool HostCommit;

        public bool _IsServer{ get; private set; }
        
        
        [Header("----- Debug Mode ------")]
        public bool StateChange_After_Timer ;

        public StateType m_StateToPlay;

        public bool debugUseMana = false;

        public float debug_timedelay = 2;







        // End Header

        // Netcode client specific
        CircularBuffer<StatePackage> LocalStateBuffer;
        // CircularBuffer<StatePackage> RemoteStateBuffer;

        CircularBuffer<InputPackage> LocalInputBuffer;
        CircularBuffer<InputPackage> RemoteInputBuffer;


        StatePackage lastServerState;
        StatePackage lastProcessedState;

        const float k_serverTickRate = 60f; // 60 FPS
        const int k_bufferSize = 1024;


        InputPackage m_LastRemoteInputRecieved ;






        /// <inheritdoc />
        public override void OnNetworkSpawn()
        {            
            // Only server dont have this component
            if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
                return;
            }

            if(IsOwner && IsHost ){
                enabled = false;
                return;
            }


            m_NetState = GetComponentInParent<NetworkCharacterState>();

            Parent = m_NetState.transform;

            m_PhysicsWrapper = m_NetState.GetComponent<PhysicsWrapper>();
            


            teamType = m_NetState.TryGetTeamType();
            // Debug.Log(teamType);

            MStateMachinePlayerViz = new StateMachineNew( this,characterStateSOs );
            // For Debug
            LastStateViz = StateType.Idle;
            // textMesh.text = coreMovement.GetFacingDirection().ToString()  ;
            // LastStateViz.ToString()
            textMesh.text = LastStateViz.ToString();

            _InitSizeHurtBox = _hurtBox.size;
            _InitCenterHurtBox = _hurtBox.center;

            // sync our visualization position & rotation to the most up to date version received from server

            transform.SetPositionAndRotation(m_PhysicsWrapper.Transform.position, m_PhysicsWrapper.Transform.rotation);
            
            //Use For trigger  some Event 
            // Owner = m_NetState.IsHOST;
            Owner = m_NetState.IsOwner;
            HostCommit = IsHost;
            _IsServer = IsServer;

            
            // Player Stuff
            if (!IsNPC)
            {
                name = "AvatarGraphics" + m_NetState.OwnerClientId;
                if (Parent.TryGetComponent(out ClientAvatarGuidHandler clientAvatarGuidHandler))
                {
                    m_VisualsNormalAnimator = clientAvatarGuidHandler.graphicsAnimator;
                }


                if (m_NetState.IsOwner )
                {
                    gameObject.AddComponent<CameraController>();

                    // --- Test new Netcode here
                    LocalStateBuffer = new CircularBuffer<StatePackage>(k_bufferSize);
                    LocalInputBuffer = new CircularBuffer<InputPackage>(k_bufferSize);
                }
                else{
                    // RemoteStateBuffer = new CircularBuffer<StatePackage>(k_bufferSize);
                    RemoteInputBuffer = new CircularBuffer<InputPackage>(k_bufferSize);
                }

                



            }


            // ----  Test new Netcode here
            if (!IsHost){
                m_NetState.ServerStateRecevie += NewServerState;
            }

            inputSender.ActionLocalInputEvent += AddLocalInput;





            // ...and visualize the current char-select value that we know about
            // m_CharacterSwapper = GetComponentInChildren<CharacterSwap>();
            // SetAppearanceSwap();
            StartCoroutine(Coro_WaitToStart(1));
            

            // ------------- Object Polling 
            if (m_NetState.CharacterClass.objectPollingRegistry){
                for (int i = 0 ; i < m_NetState.CharacterClass.objectPollingRegistry.PooledPrefabsList.Count ; i++){
                    NetworkObjectPool.Singleton.AddPrefab(m_NetState.CharacterClass.objectPollingRegistry.PooledPrefabsList[i].Prefab , m_NetState.CharacterClass.objectPollingRegistry.PooledPrefabsList[i].PrewarmCount ) ;
                }
            }
        }

        private void NewServerState(StatePackage package)
        {
            lastServerState = package;
        }

        private void AddRemoteInput(InputPackage inputpackage)
        {

            //----------- No Sense Bla bla  ------------
            // Prevent call 2 time in Client side
            // Because : Client (Not Host) already trigger ActionLocalInputEvent before the ClientPlayInputRecvie sended by server 
            // ----- 
            // Why call 2 time :  because ClientRPC will send to all Client 
            // -----
            // Why need ActionLocalInput event in client side : beacause the unity netcode use architechture dedicated to server side 
            //      Basicly , we need to send message to server (by ServerRPC) and then in server send back to all client (by ClientRPC)
            //      So the time client side recevied the message will be delay 
            //      So use ActionLocalInput that , clien call execute immedialy (can call that a prediction)
            //----------- No Sense Bla bla ------------

            // clientInputBuffer in The Remote Side (for RollBack or Reconcilliation) 
                      
            var bufferIndex = inputpackage.tick % 1024;
            // m_LastRemoteInputRecieved = inputpackage; 
            RemoteInputBuffer.Add(inputpackage ,bufferIndex);
        }

        
        private void AddLocalInput(InputPackage inputpackage){
            // clientInputBuffer in The Local Side 

            var bufferIndex = inputpackage.tick % 1024;
            LocalInputBuffer.Add(inputpackage ,bufferIndex);
            // MStateMachinePlayerViz.ShouldChangeState(ref inputpackage.buttonID);

        }

        private void PredictRemoteInputs(int localFrameAdvantage){
            if (localFrameAdvantage <= 0 ) return;
            InputPackage predictedRemote = new InputPackage(){
                buttonID = lastServerState.StateTypeEnum,
                TargetIds = lastServerState.TargetIds,
            };
            for (int i = 1; i <= localFrameAdvantage; ++i) //fill in missing remote inputs
            {

                //update until we are back at current frame
                predictedRemote.tick = (ushort)(lastServerState.tick + i);
                if (lastServerState.StateTypeEnum != StateType.Move) predictedRemote.buttonID = StateType.Idle; 
                AddRemoteInput(predictedRemote);
            }
        }



        public IEnumerator Coro_WaitToStart(float time){
            yield return new WaitForSeconds(time); 

            // IS player 
            if (!IsNPC){
                if (m_NetState.IsOwner ){
                    inputSender = GetComponentInParent<ClientInputSender>();
                    inputSender.ActionInputEvent += OnActionInput;
                    inputSender.ActionMoveInputEvent += PerformInputMove;
                }

            }

            if (!m_NetState.IsOwner){
                m_NetState.StateDataSync += PerformSyncStateFX;
 
                m_NetState.SyncEndAnimation += PlayEndAnimationFX;
                // TriangleUI.color = Color.red;
            }
            else{
                m_NetState.RecvHPClient += ReceiveHP;
            }
            yield return new WaitForSeconds(2); 

            if(IsNPC){
                m_aiBrain = GetComponentInParent<AIBrain>();
                m_aiBrain.ActionInputEvent += OnActionInput;
                m_aiBrain.PerformStateEvent += PerformSyncStateFX;
                
                m_aiBrain.ActionMoveInputEvent += PerformInputMove;
                // TriangleUI.gameObject.SetActive(false);


            }



        }


        private void OnNetworkDeswpan()
        {
            if (m_NetState)
            {
 
                if (m_NetState.IsOwner)
                {
                    
                        inputSender.ActionInputEvent -= OnActionInput;

                }else{
                        // m_NetState.InputSendServer -= PerformInput;
                        // m_NetState.InputSendBack -= PerformInput;
                        m_NetState.StateDataSync -= PerformSyncStateFX;
                        m_NetState.SyncEndAnimation -= PlayEndAnimationFX;

                }
                m_NetState.RecvHPClient -= ReceiveHP;

                // ----  Test new Netcode here

                if (!IsOwner){
                    m_NetState.ClientPlayInputRecvie -= AddRemoteInput;
                }
                inputSender.ActionLocalInputEvent -= AddLocalInput;


            }

            

        }
        




        // Remote Client Receive Input So Do Aniticipate like the true player
        
        // private void PerformInput(StateType data)
        // {
        //     MStateMachinePlayerViz.ShouldChangeState(ref data);
        // }
        
        // Remote Client Receive Input So Do Aniticipate like the true player
        private void PerformInputMove(float inputX, float inputZ)
        {
            // Debug.Log($"OnActionInput = {inputX}");
            MStateMachinePlayerViz.SaveMoveInput( (sbyte)inputX,  (sbyte) inputZ);
        }





        // TODO : Maybe dont need that , use Avatar directly  
        // HUY extend late :  Co the su dung lam chieu tang' hinh' cua Rudolf 

        void SetAppearanceSwap()
        {
            m_CharacterSwapper.SwapToModel();
        }

        // Do anticipate State :  play Animation first ,  change state but not call Enter()
        // Owner Only
        private void OnActionInput(StateType data)
        {
            // Debug.Log("perform");
            MStateMachinePlayerViz.ShouldChangeState(ref data);
        }


        public void PerformInnerSyncStateFX(byte stateIndex)
        {
            // That event do State receive from Server .
            // Debug.Log($"data  = {data}" );

            // MStateMachinePlayerViz.PerformInnerSyncStateFX(ref stateData);
        }

        public void PerformSyncStateFX(StateType stateData , SkillNumber skillNumber )
        {
            // That event do State receive from Server .
            // Debug.Log($"data  = {data}" );

            MStateMachinePlayerViz.PerformSyncStateFX(ref stateData , ref skillNumber);
        }

        
        private void PlayEndAnimationFX(StateType stateType)
        {
            MStateMachinePlayerViz.PlayEndAnimationFX(ref stateType);

        }

        #region TriggerEventSO
            

        public void PlayAudio(AudioCueSO audioCueSO , Vector3 pos = default){
            _sfxEventChannel.RaisePlayEvent(audioCueSO , _audioConfig,pos);
        }
        public void PlayAudioHit(int nbEffect){
            if (nbEffect == 7) return;
            _sfxEventChannel.RaisePlayEvent(_audioHits.AudioByEffectHits[nbEffect].Sound_Hit ,_audioConfig, m_PhysicsWrapper.Transform.position);
        }

        public void DamgePopUp(Vector3 position , int damageAmount){
            _damagePopUpChannel.RaisePopUpEvent(position , damageAmount);
        }
        #endregion





        public void Tick(int currenFrame) {
            if(IsOwner && IsHost ){
                return;
            }

            InputPackage inputThisFrame;
            if (IsOwner){
                inputThisFrame = LocalInputBuffer.Get(currenFrame);
            }else{
                PredictRemoteInputs(currenFrame - lastServerState.tick);
                inputThisFrame = RemoteInputBuffer.Get(currenFrame);
            }

            MStateMachinePlayerViz.OnUpdate();
            if (LastStateViz !=  MStateMachinePlayerViz.CurrentStateViz.GetId()){
                LastStateViz = MStateMachinePlayerViz.CurrentStateViz.GetId();
                textMesh.text = LastStateViz.ToString();
                // textMesh.text = coreMovement.GetFacingDirection().ToString();
            } 

            // Need move this code to above or below MStateMachinePlayerViz.OnUpdate();
            // Dont decide yet 

            (sbyte inputX, sbyte inputZ) = InputSerialization.ConvertDirectionalInputToAxis(inputThisFrame.dir);
            MStateMachinePlayerViz.SaveMoveInput(inputX , inputZ);
            MStateMachinePlayerViz.ShouldChangeState(ref inputThisFrame.buttonID);

        }


        // public void TickServer(InputPackage inputThisFrame ) {
            
        //     MStateMachinePlayerViz.OnUpdate();
        //     if (LastStateViz !=  MStateMachinePlayerViz.CurrentStateViz.GetId()){
        //         LastStateViz = MStateMachinePlayerViz.CurrentStateViz.GetId();
        //         textMesh.text = LastStateViz.ToString();
        //         // textMesh.text = coreMovement.GetFacingDirection().ToString();
        //     } 

        //     // Need move this code to above or below MStateMachinePlayerViz.OnUpdate();
        //     // Dont decide yet 

        //     (sbyte inputX, sbyte inputZ) = InputSerialization.ConvertDirectionalInputToAxis(inputThisFrame.dir);
        //     MStateMachinePlayerViz.SaveMoveInput(inputX , inputZ);
        //     MStateMachinePlayerViz.ShouldChangeState(ref inputThisFrame.buttonID);

        // }

        // void HandleServerTick() {
        //     if (!IsServer) return;
             
        //     var bufferIndex = -1;
        //     InputPackage inputPayload = default;
        //     while (serverInputQueue.Count > 0) {
        //         inputPayload = serverInputQueue.Dequeue();
                
        //         bufferIndex = inputPayload.tick % k_bufferSize;
        //         if (!IsOwner) TickServer(inputPayload);
        //         SaveGameState(inputPayload.tick , local: false);
        //         // serverStateBuffer.Add(statePayload, bufferIndex);
        //     }
            
        //     if (bufferIndex == -1) return;
        //     // HandleExtrapolation(serverStateBuffer.Get(bufferIndex), CalculateLatencyInMillis(inputPayload));
        //     SendToClientRpc(serverStateBuffer.Get(bufferIndex));
        // }





        // private void FixedUpdate() 
        // {
        //     // Debug.Log(MStateMachinePlayerViz);
        //     // if (m_NetState.LifeState != LifeState.Dead){    
        //     MStateMachinePlayerViz.OnUpdate();
        //     if (LastStateViz !=  MStateMachinePlayerViz.CurrentStateViz.GetId()){
        //         LastStateViz = MStateMachinePlayerViz.CurrentStateViz.GetId();
        //         textMesh.text = LastStateViz.ToString();
        //         // textMesh.text = coreMovement.GetFacingDirection().ToString();
        //     }
        //     // }
        //     // UpdateSizeHurtBox();
        // }


        // ID 300 = Play sound
        public void OnAnimEvent(int id)
        {
            //if you are trying to figure out who calls this method, it's "magic". The Unity Animation Event system takes method names as strings,
            //and calls a method of the same name on a component on the same GameObject as the Animator. See the "attack1" Animation Clip as one
            //example of where this is configured.

            MStateMachinePlayerViz.OnAnimEvent(id);
        }



        public void OnTriggerEnter(Collider collider) {
            if (collider.CompareTag("HurtBox")){
                MStateMachinePlayerViz?.OnTriggerEnter(collider);
            }
            if (collider.CompareTag("Projectile")){
                MStateMachinePlayerViz?.OnTriggerEnter(collider);
            }
        }

        // private void OnTriggerStay(Collider collider) {
        //     if (collider.CompareTag("HurtBox")){
        //         MStateMachinePlayerViz?.OnTriggerStay(collider);
        //     }
        // }


        public void OnTriggerExit(Collider collider) {
            if (collider.CompareTag("HurtBox")){
                MStateMachinePlayerViz?.OnTriggerExit(collider);
            }
            if (collider.CompareTag("Projectile")){
                MStateMachinePlayerViz?.OnTriggerExit(collider);
            }
        }

        public void ReceiveHP(AttackDataSend attackData)
        {
            // Debug.Log("attackData" + attackData.Direction);
            if (m_NetState.LifeState != LifeState.Alive ) return;
            
            // Owner call this function 
            MStateMachinePlayerViz.OnHurtReponder(ref attackData);
        
        }

        public void MPChange(int mana)
        {
            if (Owner) m_NetState.MPChangeServerRpc(mana);

        }

        public int MPRemain()
        {
            return m_NetState.MPPoints;
        }

        public int HPRemain()
        {
            return m_NetState.HPPoints;
        }
        public bool IsDamageable(TeamType teamAttacker)
        {
            // 1 Check if Alive 
            if (!( m_NetState.LifeState == LifeState.Alive)) return false;
            // 2 Check Diff Team 
            if (teamAttacker == TeamType.INDEPENDANT ||  teamAttacker != teamType) return true;
            // 1 or 2 false
            return false;

            
        }

        #region HurtBox
        
        public void UpdateSizeHurtBox(bool disableHurtBox = false){
            if (disableHurtBox) {
                _hurtBox.enabled = false;
                return;
            }
            _hurtBox.enabled = true;
            Vector3 newSize = new Vector3(spriteRenderer.sprite.bounds.size.x , spriteRenderer.sprite.bounds.size.y , 18 ); 
            _hurtBox.size = newSize;
            _hurtBox.center = spriteRenderer.sprite.bounds.center;
            // Debug.Log(spriteRenderer.sprite.bounds.size);
        }

        public void InitializeSizeHurtBox(){
            
            _hurtBox.size = _InitSizeHurtBox;
            _hurtBox.center = _InitCenterHurtBox;
            
            // Debug.Log(spriteRenderer.sprite.bounds.size);
        }

        public Vector3 GetCenterHitBox(){
            return _hitBox.bounds.center;
            // Debug.Log(spriteRenderer.sprite.bounds.size);
        }

        public void SetHitBox(bool setting){
            _hitBox.enabled = setting;
            // Debug.Log("hitbox now is enable " + _hitBox.enabled);
            // Debug.Log(spriteRenderer.sprite.bounds.size);
        }
    #endregion


        public void ActiveHitLag(float speed , float delay)
        {
            StartCoroutine(Coro_ActiveHitLag(speed,delay));
        }

        public IEnumerator Coro_ActiveHitLag(float speed , float delay)
        {
            NormalAnimator.speed = speed;
            yield return new WaitForSeconds(delay);
            NormalAnimator.speed = 1;

        }
        
        public StateType GetStateType()
        {
            return MStateMachinePlayerViz.CurrentStateViz.GetId();
        }
        public void FlashCharacter(int nbflash, float delay = 0.2f ,bool disable = false){
            StartCoroutine(CoroStartFlashCharacter(nbflash)); 
        }
        public IEnumerator CoroStartFlashCharacter(int nbflash, float delay = 0.1f ,bool disable = false){

            for (int loop = 0; loop < nbflash; loop++) {            
                // cycle through all sprites
                if (disable) {
                    // for disabling
                    spriteRenderer.enabled = false;
                } else {
                    // for changing the alpha
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.4f);
                }
    
                // delay specified amount
                yield return new WaitForSeconds(delay);
    
                // cycle through all sprites
                if (disable) {
                    // for disabling
                    spriteRenderer.enabled = true;
                } else {
                    // for changing the alpha
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
                }
                // delay specified amount
                yield return new WaitForSeconds(delay);
            }
            // Debug.Log("huy");

            if (Owner) m_NetState.LifeStateChangeServerRpc(LifeState.Alive);

        }
        public void VibrationHitLag(int direction){
            // transform.position.x    = Mathf.Sin(Time.time * speed) * amount;
            StartCoroutine(Shake(0.2f,direction*20f));
            // transform =  transform.DOShakePosition (duration , intensity, 10 , 0);
        }
        private IEnumerator Shake(float time , float distance)
        {
            float _timer = 0f;
            Vector3 _startPos = transform.position;
            Vector3 _randomPos;
            while (_timer < time)
            {
                _timer += Time.deltaTime;

                _randomPos = _startPos +(_timer * distance)*Vector3.right; 

                transform.position = _randomPos;

                yield return null;
            }

            transform.position = _startPos;
        }

#region DEBUG FUNCTION
        public void TestState(){
            // Debug.Log("Test State");
            MStateMachinePlayerViz.AnticipateState(m_StateToPlay);
        }
        public void TestStatePeriodic(bool clickOne ){
            if (clickOne && playState_Coro == null) {
                playState_Coro = StartCoroutine(PlayState_Coro(debug_timedelay));
            }
            else if (!clickOne && playState_Coro != null){
                StopCoroutine(playState_Coro)  ;          
                playState_Coro = null;
            }
        }

        private IEnumerator PlayState_Coro(float timedelay){
            while(true){
                MStateMachinePlayerViz.AnticipateState(m_StateToPlay);
                yield return new WaitForSeconds(timedelay);
            }
        }

        public void SaveGameState(int currentTick , bool local)
        {
            if(IsOwner && IsHost ){
                return;
            }
            var bufferIndex = currentTick % 1024;
            StatePackage stateThisFrame = new StatePackage() {
                tick = currentTick,
                TargetIds = this.NetworkObjectId,
                StateTypeEnum = MStateMachinePlayerViz.CurrentStateViz.GetId(),
                Position = transform.position,
                Velocity = coreMovement.RB.velocity,
                Rotation_Y = (sbyte)coreMovement.FacingDirection
            };
            LocalStateBuffer.Add(stateThisFrame ,bufferIndex );
        }

        public void HandleRollbacks()
        {

            // GameState g = null;
            // lock (GameStateDictionary)
            // {
            //     GameStateDictionary.TryGetValue(RollbackFrames.Min(), out g);
            //     RollbackFrames.Clear();
            // }
            // if (g == null) return current;
            // while (g.frameID < current.frameID)
            // {
            //     FrameInputDictionary.TryGetValue((ushort)g.frameID, out InputSerialization.FrameInfo f);
            //     g = g.Tick(f);
            // }
            // rollbackCount++;
            // return g;
    
        }



        #endregion












    }
}
