using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using System;
using TMPro;

namespace LF2.Client
{

    public class CustomList<KeyPressedType> : List<KeyPressedType>{
        public Action<KeyPressedType> listChangedEvent;
        public bool isButtonDefensePressed;

        public new void Add(KeyPressedType item){
            if (isButtonDefensePressed){
                base.Add(item);
                listChangedEvent?.Invoke(item);    
            }
        }
    }

    

    public class KeyBoardAction : MonoBehaviour , InputSys.IPlayerActions{



        /// <summary>
        /// Cached reference to local player's net state.
        /// We find the Sprites to use by checking the Skill1, Skill2, and Skill3 members of our chosen CharacterClass
        /// </summary>
        NetworkCharacterState m_NetState;



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



        private bool AttackPressed ;
        private float AttackPressedStartTime; 
        private const float m_InputHoldTime = 0.2f;


        // Run 
        private float timeLastTap;
        private int numberTap;

        #endregion


        private InputSys gameInput;
        [SerializeField] ClientInputSender clientInputSender;

        private CustomList<KeyPressedType> keyPresseds = new CustomList<KeyPressedType>();
        [SerializeField] List<ComboKey> avilableCombos; //All the Avilable Moves
        [SerializeField] TextMeshPro controlsTestText ;



        private void OnEnable()
        {
            if (gameInput == null)
            {
                gameInput = new InputSys();
                gameInput.Player.SetCallbacks(this);
            }

            EnableGameplayInput();
            keyPresseds.listChangedEvent += keyPressedsChanged_Callback;
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



        #region KEyboard (Not importance)

        
        public void OnMouvement(InputAction.CallbackContext context)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            // // Debug.Log("dir keyboard" + dir);
            if (context.started){
                if (numberTap%2 == 0) numberTap = 0;
                numberTap++;
                if (numberTap == 2 && Time.time - timeLastTap <= 0.2  ) {
                    clientInputSender.RequestAction(StateType.Run);
                    return;
                }
                timeLastTap = Time.time; 
                clientInputSender.SendMoveInput(dir );
                if (dir.x != 0 ) {
                    keyPresseds.Add(KeyPressedType.Left_Right);
                }
                if (dir.y > 0 ){
                    keyPresseds.Add(KeyPressedType.Up);
                }else if (dir.y < 0){
                    keyPresseds.Add(KeyPressedType.Down);
                }
            }
            if (context.performed){
                clientInputSender.SendMoveInput(dir );
            }
            if (context.canceled){
                clientInputSender.SendMoveInput(dir );
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
                keyPresseds.Add(KeyPressedType.Jump);
                clientInputSender.RequestAction(StateType.Jump) ;
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

                    clientInputSender.RequestAction(StateType.Attack,0,0,nbAimation);

                }
                keyPresseds.Add(KeyPressedType.Attack);
            }
      
        }

        public void OnDefense(InputAction.CallbackContext context)
        {
            if (context.started){
                // DefenseInput = true;
                keyPresseds.isButtonDefensePressed = true;
                keyPresseds.Add(KeyPressedType.Defense);
                clientInputSender.RequestAction(StateType.Defense);
            }
       
        }


        #endregion

        
        public void keyPressedsChanged_Callback(KeyPressedType keyPressedType){
            // if (keyPresseds.Count > 1 && keyPresseds.)
            PrintControls();
            if (keyPresseds.Count >= 3){
                foreach (ComboKey combo in avilableCombos){
                    if (combo.isComboAvilable(keyPresseds)){
                        // 2 parameter : name class of Scriptable Object
                        //                      Function return type of skill 
                        // Debug.Log(move.GetTypeOfSkill());
                        // Debug.Log("Requeset  ");
                        clientInputSender.RequestAction(combo.GetTypeOfSkill());
                        break;
                    }
                }
                keyPresseds.Clear();
                keyPresseds.isButtonDefensePressed = false;
            }
        }

            //Printing Keys just for testing
        public void PrintControls() {
            controlsTestText.text = " Keys Pressed :";
            foreach (KeyPressedType kcode in keyPresseds)
                controlsTestText.text += kcode + ",";
        }


    }
}
