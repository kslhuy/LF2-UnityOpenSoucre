using Unity.Netcode;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Assertions;
using System.Collections;

namespace LF2.Client
{

    public class ClientInputSender : NetworkBehaviour ,InputSys.IPlayerActions{
        
        #region Movement (Keyborad not importance)
            
        public Vector2 RawMovementInput { get ; private set;} // For keyborad

        // JUMP
        public bool JumpInput{get;private set;}
        private float jumpInputStartTime;
        // JUMP

        //RUN
        private float lastHoldRightTime;
        private float lastHoldLeftTime;
        public bool canRun {get ; private set;}
        private int countTime;
        //RUN

        Vector2 direction;
        // public bool AttackInput{get;private set;}
        // public bool DefenseInput{get;private set;}     


        #endregion

        //COMBO  


        ////// ********* NEW ****** ///
        [SerializeField] private NetworkCharacterState m_NetworkCharacter;

        #region event
        public Action<InputPackage> ActionInputEvent; 
        public Action<float, float> ActionMoveInputEvent; 

            
        #endregion

        private bool CommitToState ; 


        // [SerializeField]
        // CharacterClassContainer m_CharacterClassContainer;

        // /// <summary>
        // /// Convenience getter that returns our CharacterData
        // /// </summary>
        // CharacterClass CharacterData => m_CharacterClassContainer.CharacterClass;

        private bool AttackPressed ;
        private float AttackPressedStartTime; 
        private const float m_InputHoldTime = 0.2f;


        private InputSys gameInput;


        public StateType stateToPlay;

        private string textAreaString = "0"; 

        private StateType[] values ;




        // COMBO

        public override void OnNetworkSpawn(){
            if (!IsClient || !IsOwner)
                {
                    enabled = false;
                    // dont need to do anything else if not the owner
                    return;
                }

            CommitToState  =  IsHost; 
            values = (StateType[])Enum.GetValues(typeof(StateType));

        }


        private void OnEnable()
        {
            if (gameInput == null)
            {
                gameInput = new InputSys();
                gameInput.Player.SetCallbacks(this);
            }

            EnableGameplayInput();
        }

        private void OnDisable()
        {
            DisableAllInput();
        }

        public void EnableGameplayInput()
        {

            gameInput.Player.Enable();
        }

        public void DisableAllInput()
        {
            gameInput.Player.Disable();

        }



        private void SendStateInput(InputPackage action)
        {
            // If we are host (= server) dont neeed to send to the server again 
            // else , client send to server

            // if (CommitToState){
            //     m_NetworkCharacter.ActionInputEventClientRPC(action);
            // }else   m_NetworkCharacter.ActionInputEventServerRPC(action);

            // Debug.Log("SendStateInput");
            if (CommitToState){
               StartCoroutine(DelayActionOnHostEvent(action));
            }else   ActionInputEvent(action);

            // StartCoroutine(DelayActionOnHostEvent(action));
        }

        private void SendMoveInput(float inputX , float inputZ )
        {
            if (CommitToState){
                m_NetworkCharacter.MoveInputEventClientRPC(inputX,inputZ);
            }else   m_NetworkCharacter.MoveInputEventServerRPC(inputX,inputZ);
            
            ActionMoveInputEvent?.Invoke(inputX,inputZ);

        }


        public void OnMoveInputUI(Vector2 inputUI){
            
            // StateType stateType ;
            // if (inputUI.x != 0 || inputUI.y != 0){
            //     stateType = StateType.Move; 
            // } 
            // else{
            //     stateType = StateType.Idle; 
            // }
            // Debug.Log(stateType);

            SendMoveInput(inputUI.x ,inputUI.y );

                       
        }

        #region KEyboard (Not importance)

        
        public void OnMouvement(InputAction.CallbackContext context)
        {
            RawMovementInput = context.ReadValue<Vector2>();
            // StateType stateType ;
            // float timeSample = Time.time;

            // if (RawMovementInput.x != 0 || RawMovementInput.y != 0){
            //     stateType = StateType.Move; 
            // } 
            // else{
            //     stateType = StateType.Idle; 
            // }

            if (context.started){
                SendMoveInput(RawMovementInput.x ,RawMovementInput.y );

            }
            if (context.performed){
                SendMoveInput(RawMovementInput.x ,RawMovementInput.y );

            }
            if (context.canceled){
                SendMoveInput(RawMovementInput.x ,RawMovementInput.y );

            }
        }

     
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started){
                JumpInput = true;
                jumpInputStartTime = Time.time;
                // IF some character can jump different with other , 
                // Need to specifie in CharacterData.Skill or .Jump (specific)
                // Debug.Log(inputX);
                RequestAction(StateType.Jump) ;
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started){
                // Debug.Log("OnAttackInput");
                // Same with Jump
                // RequestAction(StateType.Attack);
                if (AttackPressed){
                    if ( Time.time >= AttackPressedStartTime + m_InputHoldTime){
                        AttackPressed = false;
                    }
                } 
                if (!AttackPressed) {
                    int nbAimation = UnityEngine.Random.Range(1,3);
                    // Debug.Log((nbAimation)); 
                    AttackPressed = true;
                    AttackPressedStartTime = Time.time;                

                    RequestAction(StateType.Attack,0,0,nbAimation);

                }
            }
      
        }

        public void OnDefense(InputAction.CallbackContext context)
        {
            if (context.started){
                // DefenseInput = true;
                RequestAction(StateType.Defense);
            }
       
        }


        #endregion


        /// <summary>
        /// Request an State be performed. This will occur on the next FixedUpdate.
        /// </summary>
        /// <param name="action">the action you'd like to perform. </param>

            // In the furture may be we can developp this feature
        /// <param name="triggerStyle">What input style triggered this action.</param>
        public void RequestAction(StateType actionType,float inputX = 0 , float inputZ = 0 , int nbAniamtion = 0, ulong targetId = 0 )
        {
            // Debug.Log(inputX);

            // In that time we can extend data more 
            // But now only StateType are send 
            var data = new InputPackage(){
                StateTypeEnum = actionType,
            };
            // Debug.Log(data.InputX);
            if (nbAniamtion != 0){
                data.NbAnimation = (Int16)nbAniamtion;
            }
            SendStateInput(data);
            
        }


        IEnumerator DelayActionOnHostEvent (InputPackage input){
            yield return new WaitForSeconds(.02f);
            ActionInputEvent?.Invoke(input);

        }

        void OnGUI()
        {
            if (m_NetworkCharacter.IsOwner){

                                // Make a background box
                GUIStyle huy =new GUIStyle();
                huy.fontSize = 100;
                // GUI.Box(new Rect(50,50,500,500), "Test State");
                textAreaString = GUI.TextField (new Rect (100, 300, 400, 100), textAreaString,2,huy);

                // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
                if(GUI.Button(new Rect(300,500,200,100), "Play State"))
                {
                    int enumNB =  int.Parse(textAreaString);
                    stateToPlay = (StateType)((enumNB) % values.Length);
                    Debug.Log($"NB = {enumNB} =>  {stateToPlay} state");
                    var data = new InputPackage(){
                        StateTypeEnum = stateToPlay,
                    };
                    SendStateInput(data);      
                }
            
                // Make the second button.

            }
        }



    }
}
