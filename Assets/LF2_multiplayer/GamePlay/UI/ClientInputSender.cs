using Unity.Netcode;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Assertions;
using System.Collections;

namespace LF2.Client
{

    public class ClientInputSender : NetworkBehaviour {
        

        //COMBO  


        ////// ********* NEW ****** ///
        [SerializeField] private NetworkCharacterState m_NetworkCharacter;

        private CharacterStateSOs characterStateSOs {
            get
            {
                CharacterStateSOs result;
                var found = GameDataSourceNew.Instance.AllStateCharacterByType.TryGetValue(m_NetworkCharacter.CharacterType, out result);
                // Debug.Log(result);
                Debug.AssertFormat(found, "Tried to find Character but it was missing from GameDataSource!");
                return result;
            }
        }

        #region event
        public Action<StateType> ActionInputEvent; 
        public Action<float, float> ActionMoveInputEvent; 

            
        #endregion

        private bool CommitToState ; 
        private bool _isClient;





        // COMBO

        public override void OnNetworkSpawn(){
            if (!IsClient || !IsOwner)
                {
                    enabled = false;
                    // dont need to do anything else if not the owner
                    return;
                }

            CommitToState  =  IsHost; 

            // values = (StateType[])Enum.GetValues(typeof(StateType));

        }

        public StateType ConvertToRunTimeStateType(StateType inputState){

            if (inputState == StateType.DDA1){
                return characterStateSOs.RunTimeDDA.RunTimeStateType;
            }
            else if (inputState == StateType.DDJ1){
                return characterStateSOs.RunTimeDDJ.RunTimeStateType;
            }
            else if (inputState == StateType.DUA1){
                return characterStateSOs.RunTimeDUA.RunTimeStateType;
            }
            else if (inputState == StateType.DUJ1){
                return characterStateSOs.RunTimeDUJ.RunTimeStateType;
            }else {
                return inputState;
            }
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

        public void SendMoveInput(int inputX , int inputZ )
        {
            // if (CommitToState){
            //     m_NetworkCharacter.MoveInputEventClientRPC(inputX,inputZ);
            // }
            // else   m_NetworkCharacter.MoveInputEventServerRPC(inputX,inputZ);
            
            ActionMoveInputEvent?.Invoke(inputX,inputZ);

        }

        public void SendMoveInput(Vector2 input )
        {
            // if (CommitToState){
            //     m_NetworkCharacter.MoveInputEventClientRPC((int)input.x,(int)input.y);
            // }
            // else   m_NetworkCharacter.MoveInputEventServerRPC(inputX,inputZ);
            
            ActionMoveInputEvent?.Invoke(input.x,input.y);

        }




        /// <summary>
        /// Request an State be performed. This will occur on the next FixedUpdate.
        /// </summary>
        /// <param name="action">the action you'd like to perform. </param>

            // In the furture may be we can developp this feature
        /// <param name="triggerStyle">What input style triggered this action.</param>
        public void RequestAction(StateType actionType,float inputX = 0 , float inputZ = 0 )
        {
            // Debug.Log(inputX);

            // In that time we can extend data more 
            // But now only StateType are send 
            // var data = new InputPackage(){
            //     StateTypeEnum = ConvertToRunTimeStateType(actionType),
            // };
            // Debug.Log(actionType + " convert to " + ConvertToRunTimeStateType(actionType)); 
            ActionInputEvent?.Invoke(ConvertToRunTimeStateType(actionType));

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
