using Unity.Netcode;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Assertions;
using System.Collections;
using LF2;

using System.Collections.Generic;

namespace LF2.Client
{

    public class ClientInputSender : NetworkBehaviour {
        

        //COMBO  


        ////// ********* NEW ****** ///
        [SerializeField] private NetworkCharacterState m_NetworkCharacter;
        // [SerializeField] private ClientCharacterVisualization m_ClientCharacter;


        public InputSerialization.DirectionalInput dirInput;



        readonly InputPackage[] m_InputPackages = new InputPackage[5];
        private InputPackage m_InputThisFrame;





        

        private CharacterStateSOs characterStateSOs ;
            // get
            // {
                
            //     // CharacterStateSOs result;
            //     // var found = GameDataSourceNew.Instance.AllStateCharacterByType.TryGetValue(m_NetworkCharacter.CharacterType, out result);
            //     // // Debug.Log(result);
            //     // Debug.AssertFormat(found, "Tried to find Character but it was missing from GameDataSource!");
            //     return m_NetworkCharacter.CharacterStateSO;
            // }
        

        #region event
        public Action<StateType> ActionInputEvent; 

        public Action<InputPackage> ActionLocalInputEvent; 

        public Action<float, float> ActionMoveInputEvent;
        private int m_InputRequestCount;


        #endregion




        private void Awake() {
            // clientStateBuffer = new CircularBuffer<StatePackage>(k_bufferSize);
            // clientInputBuffer = new CircularBuffer<InputPackage>(k_bufferSize);
            
            // serverStateBuffer = new CircularBuffer<StatePackage>(k_bufferSize);
            // serverInputQueue = new Queue<InputPackage>();
   
        }


        // COMBO

        public override void OnNetworkSpawn(){
            if (!IsClient || !IsOwner)
                {
                    enabled = false;
                    // dont need to do anything else if not the owner
                    return;
                }
            characterStateSOs = m_NetworkCharacter.CharacterStateSO;
            // values = (StateType[])Enum.GetValues(typeof(StateType));
         
        }

        public StateType ConvertToRunTimeStateType(StateType inputState){

            if (inputState == StateType.DDA1){
                return characterStateSOs.RunTimeDDA.StateLogicSOs[0].StateType;
            }
            else if (inputState == StateType.DDJ1){
                return characterStateSOs.RunTimeDDJ.StateLogicSOs[0].StateType;
            }
            else if (inputState == StateType.DUA1){
                return characterStateSOs.RunTimeDUA.StateLogicSOs[0].StateType;
            }
            else if (inputState == StateType.DUJ1){
                return characterStateSOs.RunTimeDUJ.StateLogicSOs[0].StateType;
            }else {
                return inputState;
            }
        }







        public void SendMoveInput(int inputX , int inputZ )
        {
            // if (CommitToState){
            //     m_NetworkCharacter.MoveInputEventClientRPC(inputX,inputZ);
            // }
            // else   m_NetworkCharacter.MoveInputEventServerRPC(inputX,inputZ);
            
            ActionMoveInputEvent?.Invoke(inputX,inputZ);

            dirInput = InputSerialization.ConvertInputAxisToDirectionalInput((sbyte)inputX, (sbyte)inputZ);
            Debug.Log(dirInput);

        }

        public void SendMoveInput(Vector2 input )
        {
            // if (CommitToState){
            //     m_NetworkCharacter.MoveInputEventClientRPC((int)input.x,(int)input.y);
            // }
            // else   m_NetworkCharacter.MoveInputEventServerRPC(inputX,inputZ);
            
            ActionMoveInputEvent?.Invoke(input.x,input.y);

            
            dirInput = InputSerialization.ConvertInputAxisToDirectionalInput((sbyte)input.x, (sbyte)input.y);

            Debug.Log(dirInput);

        }


        // private void SendStateInput(InputPackage action)
        // {
        //     // If we are host (= server) dont neeed to send to the server again 
        //     // else , client send to server


        //     // if (CommitToState){
        //     //    StartCoroutine(DelayActionOnHostEvent(action));
        //     //     ActionInputEvent?.Invoke(action);
        //     // }else   {
        //     //     ActionInputEvent(action);

        //     // }
        //     ActionInputEvent?.Invoke(action);
        //     // if (_isClient){
        //     //     m_NetworkCharacter.SendInputActionToServerRpc(action);
        //     // }

        // }

        void SendInput(InputPackage inputPackage)
        {
            // if (IsHost) {
            //     m_NetworkCharacter.RecvDoActionClientRPC(inputPackage);
            //     ActionLocalInputEvent?.Invoke(inputPackage);
            // }
            
            // else if (IsClient && !IsHost ) {
            //     m_NetworkCharacter.SendToServerRpc(inputPackage);
            //     ActionLocalInputEvent?.Invoke(inputPackage);
            // }

            m_NetworkCharacter.SendToServerRpc(inputPackage);
            ActionLocalInputEvent?.Invoke(inputPackage);
            // ActionInputEvent?.Invoke(action);
            // m_NetworkCharacter.RecvDoActionServerRPC(inputPackage);
        }





        
        private void FixedUpdate() {
            HandleClientTick();
        }



        void HandleClientTick() {
            if (!IsClient || !IsOwner) return;

            // var currentTick = m_ClientCharacter.NetworkTimer.CurrentTick;

            InputPackage inputPayload = new InputPackage() {
                // tick = currentTick,
                dir = m_InputThisFrame.dir,
                buttonID = m_InputThisFrame.buttonID
            };
            
            SendInput(inputPayload);

        }





        /// <summary>
        /// Request an State be performed. This will occur on the next FixedUpdate.
        /// </summary>
        /// <param name="action">the action you'd like to perform. </param>

            // In the furture may be we can developp this feature
        /// <param name="triggerStyle">What input style triggered this action.</param>
        public void RequestAction(StateType actionType,float inputX = 0 , float inputZ = 0 )
        {

            // m_InputThisFrame.tick =  m_ClientCharacter.NetworkTimer.CurrentTick;
            m_InputThisFrame.buttonID = ConvertToRunTimeStateType(actionType);
            m_InputThisFrame.dir = dirInput;

            // if (m_InputRequestCount < m_InputPackages.Length)
            // {
            //     m_InputPackages[m_InputRequestCount].tick = m_ClientCharacter.NetworkTimer.CurrentTick;
            //     m_InputPackages[m_InputRequestCount].buttonID = ConvertToRunTimeStateType(actionType);
            //     m_InputPackages[m_InputRequestCount].dir = dirInput;

            //     m_InputRequestCount++;
            // }

            // ActionInputEvent?.Invoke(ConvertToRunTimeStateType(actionType));

            // SendStateInput(data);
            
        }



        // void OnGUI()
        // {
        //     if (m_NetworkCharacter.IsOwner){

        //                         // Make a background box
        //         GUIStyle huy =new GUIStyle();
        //         huy.fontSize = 100;
        //         // GUI.Box(new Rect(50,50,500,500), "Test State");
        //         textAreaString = GUI.TextField (new Rect (100, 300, 400, 100), textAreaString,2,huy);

        //         // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        //         if(GUI.Button(new Rect(300,500,200,100), "Play State"))
        //         {
        //             int enumNB =  int.Parse(textAreaString);
        //             stateToPlay = (StateType)((enumNB) % values.Length);
        //             Debug.Log($"NB = {enumNB} =>  {stateToPlay} state");
        //             var data = new InputPackage(){
        //                 StateTypeEnum = stateToPlay,
        //             };
        //             SendStateInput(data);      
        //         }
            
        //         // Make the second button.

        //     }
        // }



    }
}
