using System.Collections.Generic;
// using Cinemachine;
using Unity.Netcode;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using System.Collections;
using DG.Tweening;

namespace LF2.Client
{
    /// <summary>
    /// <see cref="ClientCharacterVisualization"/> is responsible for displaying a character on the client's screen based on state information sent by the server.
    /// </summary>

    public class ClientCharacterVisualization : NetworkBehaviour 
    {
        [SerializeField]
        private Animator m_VisualsNormalAnimator;
        [SerializeField]
        private Animator m_VisualsInjuryAnimator;

        CharacterSwap m_CharacterSwapper;


        private ClientInputSender inputSender;

        [SerializeField] private BoxCollider _hitBox;
        [SerializeField] private BoxCollider _hurtBox;



        

        /// <summary>
        /// Returns a reference to the active Animator for this visualization
        /// </summary>
        public Animator NormalAnimator { get { return m_VisualsNormalAnimator; } }
        public Animator InjuryAnimator { get { return m_VisualsInjuryAnimator; } }
        

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

        [SerializeField] SpriteRenderer spriteRenderer;

        public VisualizationConfiguration VizAnimation;

        [Header("----- Audio Event ------")]

        [SerializeField] private AudioCueEventChannelSO _sfxEventChannel = default;
        [SerializeField] private AudioConfigurationSO _audioConfig = default;
        // End Header 


        public StateMachineNew MStateMachinePlayerViz{ get; private set; }
        private CharacterStateSOs characterStateSOs {
            get
            {
                CharacterStateSOs result;
                var found = GameDataSourceNew.Instance.AllStateCharacterByType.TryGetValue(m_NetState.CharacterType, out result);
                // Debug.Log(result);
                Debug.AssertFormat(found, "Tried to find Character but it was missing from GameDataSource!");
                return result;
            }
        }
        public StateType LastStateViz { get; private set; }
        private RectTransform UirectTransform ;

        /// Player characters need to report health changes and chracter info to the PartyHUD
        [SerializeField] SpriteRenderer TriangleUI;

        //// --- NPC ---
        private AIBrain m_aiBrain;

        public bool IsNPC => m_NetState.IsNpc;

        //// --- NPC ---
        public bool CanCommit{ get; private set; } 

        private bool HostCommit;

        public bool _IsServer{ get; private set; }
        
        
        [Header("----- Debug Mode ------")]
        public bool StateChange_After_Timer ;

        public StateType m_StateToPlay;



        // End Header




        /// <inheritdoc />
        public override void OnNetworkSpawn()
        {            
            // Only server dont have this component
            if (!NetworkManager.Singleton.IsClient )
            {
                enabled = false;
                return;
            }

            m_NetState = GetComponentInParent<NetworkCharacterState>();

            Parent = m_NetState.transform;

            m_PhysicsWrapper = m_NetState.GetComponent<PhysicsWrapper>();
            


            teamType = m_NetState.TryGetTeamType();
            Debug.Log(teamType);

            MStateMachinePlayerViz = new StateMachineNew( this,characterStateSOs );
            // For Debug
            LastStateViz = StateType.Idle;
            textMesh.text = LastStateViz.ToString();

            _hitBox.enabled = false;
            _hurtBox.enabled = true;




    

            // sync our visualization position & rotation to the most up to date version received from server

            transform.SetPositionAndRotation(m_PhysicsWrapper.Transform.position, m_PhysicsWrapper.Transform.rotation);
            
            //Use For trigger  some Event 
            // CanCommit = m_NetState.IsHOST;
            CanCommit = m_NetState.IsOwner;
            HostCommit = m_NetState.IsHOST;
            _IsServer = IsServer;

            // Player Stuff
            if (!IsNPC)
            {
                name = "AvatarGraphics" + m_NetState.OwnerClientId;
                if (Parent.TryGetComponent(out ClientAvatarGuidHandler clientAvatarGuidHandler))
                {
                    m_VisualsNormalAnimator = clientAvatarGuidHandler.graphicsAnimator;
                    // m_VisualsInjuryAnimator = clientAvatarGuidHandler.graphicsAnimator;
                }

                // ...and visualize the current char-select value that we know about
                m_CharacterSwapper = GetComponentInChildren<CharacterSwap>();
                SetAppearanceSwap();

         

                if (m_NetState.IsOwner )
                {
                    gameObject.AddComponent<CameraController>();
                    
                        // Debug.Log("h");
                    inputSender = GetComponentInParent<ClientInputSender>();
                    
                    inputSender.ActionInputEvent += OnActionInput;
                    inputSender.ActionMoveInputEvent += PerformInputMove;
                    TriangleUI.color = Color.green;
                                        
                }
                // else {
                //     // Only if not owner of input  
                //     // m_NetState.InputSendBack += OnActionInput;
                //     m_NetState.InputMoveSendBack += PerformInputMove;
                //     m_NetState.StateDataSync += PerformSyncStateFX;


                // }
            }
            // AI stuff
            else{
                // if (m_NetState.IsOwnedByServer){
                //     m_aiBrain = GetComponentInParent<AIBrain>();
                //     m_aiBrain.ActionInputEvent += OnActionInput;
                //     m_aiBrain.ActionMoveInputEvent += PerformInputMove;

                // }
                
                    m_aiBrain = GetComponentInParent<AIBrain>();
                    m_aiBrain.ActionInputEvent += OnActionInput;
                    m_aiBrain.PerformStateEvent += PerformSyncStateFX;
                    
                    m_aiBrain.ActionMoveInputEvent += PerformInputMove;
                    TriangleUI.gameObject.SetActive(false);


            }
            if (!m_NetState.IsOwner){
                m_NetState.InputMoveSendBack += PerformInputMove;
                m_NetState.StateDataSync += PerformSyncStateFX;
                TriangleUI.color = Color.red;
            }

        }

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

        // Remote Client Receive Input So Do Aniticipate like the true player
        private void PerformInput(InputPackage data)
        {
            MStateMachinePlayerViz.DoAnticipate(ref data);
        }
        // Remote Client Receive Input So Do Aniticipate like the true player
        private void PerformInputMove(float inputX, float inputZ)
        {
            // Debug.Log($"OnActionInput = {inputX}");
            MStateMachinePlayerViz.SaveMoveInput(ref inputX, ref inputZ);
        }






        // HUY extend late :  Co the su dung lam chieu tang' hinh' cua Rudolf 
        void SetAppearanceSwap()
        {
            m_CharacterSwapper.SwapToModel();
        }

        // Do anticipate State :  play Animation first ,  change state but not call Enter()
        // Owner Only
        private void OnActionInput(InputPackage data)
        {
            // Debug.Log("perform");
            MStateMachinePlayerViz.DoAnticipate(ref data);
        }

        
        public void PerformSyncStateFX(StateType stateData)
        {
            // That event do State receive from Server .
            // Debug.Log($"data  = {data}" );

            MStateMachinePlayerViz.PerformSyncStateFX(ref stateData);
        }

        public void PlayAudio(AudioCueSO audioCueSO , Vector3 pos = default){
            _sfxEventChannel.RaisePlayEvent(audioCueSO , _audioConfig,pos);
        }




        private void OnNetworkDeswpan()
        {
            if (m_NetState)
            {
 
                if (m_NetState.IsOwner)
                {
                    
                        inputSender.ActionInputEvent -= OnActionInput;

                }else{
                        m_NetState.InputSendBack -= PerformInput;
                        m_NetState.StateDataSync -= PerformSyncStateFX;
                }
                
            }

        }





        private void FixedUpdate() 
        {
            // Debug.Log(MStateMachinePlayerViz);
            MStateMachinePlayerViz.OnUpdate();
            if (LastStateViz !=  MStateMachinePlayerViz.CurrentStateViz.GetId()){
                LastStateViz = MStateMachinePlayerViz.CurrentStateViz.GetId();
                textMesh.text = LastStateViz.ToString();
            }
        }


        // ID 300 = Play sound
        public void OnAnimEvent(int id)
        {
            //if you are trying to figure out who calls this method, it's "magic". The Unity Animation Event system takes method names as strings,
            //and calls a method of the same name on a component on the same GameObject as the Animator. See the "attack1" Animation Clip as one
            //example of where this is configured.

            MStateMachinePlayerViz.OnAnimEvent(id);
        }

        public void TestState(){
            Debug.Log("call");
            MStateMachinePlayerViz.AnticipateState(m_StateToPlay);
        }




        public void OpenHitBox(){
            _hitBox.enabled = true;
        }

        
        public void CloseHitBox(){
            _hitBox.enabled = false;
        }

        
        public void OpenHurtBox(){
            _hurtBox.enabled = true;
        }

        
        public void CloseHurtBox(){
            _hurtBox.enabled = false;
        }



        private void OnTriggerEnter(Collider collider) {
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


        private void OnTriggerExit(Collider collider) {
            if (collider.CompareTag("HurtBox")){
                MStateMachinePlayerViz?.OnTriggerExit(collider);
            }
            if (collider.CompareTag("Projectile")){
                MStateMachinePlayerViz?.OnTriggerExit(collider);
            }
        }

        public void ReceiveHP(AttackDataSend attackData)
        {
            if (m_NetState.LifeState == LifeState.Fainted ) return;
            if (CanCommit) m_NetState.HPChangeServerRpc(attackData.Amount_injury);
            if (attackData.Amount_injury > 0) return;

            MStateMachinePlayerViz.OnGameplayActivityVisual(ref attackData);
        }

        public void MPChange(int mana)
        {
            if (CanCommit) m_NetState.MPChangeServerRpc(mana);

        }

        public bool IsDamageable()
        {
            return m_NetState.LifeState == LifeState.Alive;
        }
        public int MPRemain()
        {
            return m_NetState.MPPoints;
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
            OpenHurtBox();

            if (CanCommit) m_NetState.LifeStateChangeServerRpc(LifeState.Alive);

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











    }
}
