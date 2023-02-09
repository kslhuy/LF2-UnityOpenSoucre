// //------------------------------------------------------------------------------
// // <auto-generated>
// //     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
// //     version 1.4.4
// //     from Assets/LF2_multiplayer/GamePlay/UI/Input/InputSys.inputactions
// //
// //     Changes to this file may cause incorrect behavior and will be lost if
// //     the code is regenerated.
// // </auto-generated>
// //------------------------------------------------------------------------------

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.InputSystem;
// using UnityEngine.InputSystem.Utilities;

// public partial class @InputSys : IInputActionCollection2, IDisposable
// {
//     public InputActionAsset asset { get; }
//     public @InputSys()
//     {
//         asset = InputActionAsset.FromJson(@"{
//     ""name"": ""InputSys"",
//     ""maps"": [
//         {
//             ""name"": ""Player"",
//             ""id"": ""222188b4-bcce-454e-9f99-473fed81b626"",
//             ""actions"": [
//                 {
//                     ""name"": ""Mouvement"",
//                     ""type"": ""Value"",
//                     ""id"": ""0baa33d2-166c-4623-a8ba-469a2cbbbe95"",
//                     ""expectedControlType"": ""Vector2"",
//                     ""processors"": """",
//                     ""interactions"": """",
//                     ""initialStateCheck"": true
//                 },
//                 {
//                     ""name"": ""Jump"",
//                     ""type"": ""Button"",
//                     ""id"": ""27a080a2-af76-4b6b-97ba-056265faf214"",
//                     ""expectedControlType"": ""Button"",
//                     ""processors"": """",
//                     ""interactions"": """",
//                     ""initialStateCheck"": false
//                 },
//                 {
//                     ""name"": ""Attack"",
//                     ""type"": ""Button"",
//                     ""id"": ""03c7ff06-bd1a-4cf7-a857-e6d453f4d073"",
//                     ""expectedControlType"": ""Button"",
//                     ""processors"": """",
//                     ""interactions"": """",
//                     ""initialStateCheck"": false
//                 },
//                 {
//                     ""name"": ""Defense"",
//                     ""type"": ""Button"",
//                     ""id"": ""04eac614-3fac-47a6-bd14-d3ff04d7ab2e"",
//                     ""expectedControlType"": ""Button"",
//                     ""processors"": """",
//                     ""interactions"": """",
//                     ""initialStateCheck"": false
//                 }
//             ],
//             ""bindings"": [
//                 {
//                     ""name"": """",
//                     ""id"": ""0a20c9e1-4ef9-4266-aed2-f933566671c3"",
//                     ""path"": ""<Gamepad>/leftStick"",
//                     ""interactions"": """",
//                     ""processors"": ""NormalizeVector2"",
//                     ""groups"": ""gamepad"",
//                     ""action"": ""Mouvement"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": false
//                 },
//                 {
//                     ""name"": ""ZQSD"",
//                     ""id"": ""87aa038c-1235-4afe-8864-a2e7d56b9389"",
//                     ""path"": ""2DVector"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": """",
//                     ""action"": ""Mouvement"",
//                     ""isComposite"": true,
//                     ""isPartOfComposite"": false
//                 },
//                 {
//                     ""name"": ""up"",
//                     ""id"": ""f989fa40-5f76-4566-aae4-0b07e5d7804b"",
//                     ""path"": ""<Keyboard>/w"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": ""Keyboard"",
//                     ""action"": ""Mouvement"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": true
//                 },
//                 {
//                     ""name"": ""down"",
//                     ""id"": ""9c9e07d7-9e77-471a-a356-97e1ea76556f"",
//                     ""path"": ""<Keyboard>/s"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": ""Keyboard"",
//                     ""action"": ""Mouvement"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": true
//                 },
//                 {
//                     ""name"": ""left"",
//                     ""id"": ""85d31d5e-0283-4168-a98c-0883fae215a6"",
//                     ""path"": ""<Keyboard>/a"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": ""Keyboard"",
//                     ""action"": ""Mouvement"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": true
//                 },
//                 {
//                     ""name"": ""right"",
//                     ""id"": ""0818d0b6-28be-4b48-80d3-48ea86542893"",
//                     ""path"": ""<Keyboard>/d"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": ""Keyboard"",
//                     ""action"": ""Mouvement"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": true
//                 },
//                 {
//                     ""name"": """",
//                     ""id"": ""f71090a3-4abf-475b-a32a-fb26aefa4c39"",
//                     ""path"": ""<Keyboard>/i"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": ""Keyboard"",
//                     ""action"": ""Jump"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": false
//                 },
//                 {
//                     ""name"": """",
//                     ""id"": ""5c620377-9244-4edb-bf67-336f8bd6dc29"",
//                     ""path"": ""<Keyboard>/u"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": ""Keyboard"",
//                     ""action"": ""Attack"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": false
//                 },
//                 {
//                     ""name"": """",
//                     ""id"": ""5e86b5c7-d578-42f6-a42f-5d76893b541b"",
//                     ""path"": ""<Keyboard>/o"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": ""Keyboard"",
//                     ""action"": ""Defense"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": false
//                 }
//             ]
//         }
//     ],
//     ""controlSchemes"": [
//         {
//             ""name"": ""Keyboard"",
//             ""bindingGroup"": ""Keyboard"",
//             ""devices"": [
//                 {
//                     ""devicePath"": ""<Keyboard>"",
//                     ""isOptional"": false,
//                     ""isOR"": false
//                 }
//             ]
//         },
//         {
//             ""name"": ""gamepad"",
//             ""bindingGroup"": ""gamepad"",
//             ""devices"": [
//                 {
//                     ""devicePath"": ""<Gamepad>"",
//                     ""isOptional"": false,
//                     ""isOR"": false
//                 }
//             ]
//         }
//     ]
// }");
//         // Player
//         m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
//         m_Player_Mouvement = m_Player.FindAction("Mouvement", throwIfNotFound: true);
//         m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
//         m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
//         m_Player_Defense = m_Player.FindAction("Defense", throwIfNotFound: true);
//     }

//     public void Dispose()
//     {
//         UnityEngine.Object.Destroy(asset);
//     }

//     public InputBinding? bindingMask
//     {
//         get => asset.bindingMask;
//         set => asset.bindingMask = value;
//     }

//     public ReadOnlyArray<InputDevice>? devices
//     {
//         get => asset.devices;
//         set => asset.devices = value;
//     }

//     public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

//     public bool Contains(InputAction action)
//     {
//         return asset.Contains(action);
//     }

//     public IEnumerator<InputAction> GetEnumerator()
//     {
//         return asset.GetEnumerator();
//     }

//     IEnumerator IEnumerable.GetEnumerator()
//     {
//         return GetEnumerator();
//     }

//     public void Enable()
//     {
//         asset.Enable();
//     }

//     public void Disable()
//     {
//         asset.Disable();
//     }
//     public IEnumerable<InputBinding> bindings => asset.bindings;

//     public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
//     {
//         return asset.FindAction(actionNameOrId, throwIfNotFound);
//     }
//     public int FindBinding(InputBinding bindingMask, out InputAction action)
//     {
//         return asset.FindBinding(bindingMask, out action);
//     }

//     // Player
//     private readonly InputActionMap m_Player;
//     private IPlayerActions m_PlayerActionsCallbackInterface;
//     private readonly InputAction m_Player_Mouvement;
//     private readonly InputAction m_Player_Jump;
//     private readonly InputAction m_Player_Attack;
//     private readonly InputAction m_Player_Defense;
//     public struct PlayerActions
//     {
//         private @InputSys m_Wrapper;
//         public PlayerActions(@InputSys wrapper) { m_Wrapper = wrapper; }
//         public InputAction @Mouvement => m_Wrapper.m_Player_Mouvement;
//         public InputAction @Jump => m_Wrapper.m_Player_Jump;
//         public InputAction @Attack => m_Wrapper.m_Player_Attack;
//         public InputAction @Defense => m_Wrapper.m_Player_Defense;
//         public InputActionMap Get() { return m_Wrapper.m_Player; }
//         public void Enable() { Get().Enable(); }
//         public void Disable() { Get().Disable(); }
//         public bool enabled => Get().enabled;
//         public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
//         public void SetCallbacks(IPlayerActions instance)
//         {
//             if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
//             {
//                 @Mouvement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouvement;
//                 @Mouvement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouvement;
//                 @Mouvement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouvement;
//                 @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
//                 @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
//                 @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
//                 @Attack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
//                 @Attack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
//                 @Attack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
//                 @Defense.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDefense;
//                 @Defense.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDefense;
//                 @Defense.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDefense;
//             }
//             m_Wrapper.m_PlayerActionsCallbackInterface = instance;
//             if (instance != null)
//             {
//                 @Mouvement.started += instance.OnMouvement;
//                 @Mouvement.performed += instance.OnMouvement;
//                 @Mouvement.canceled += instance.OnMouvement;
//                 @Jump.started += instance.OnJump;
//                 @Jump.performed += instance.OnJump;
//                 @Jump.canceled += instance.OnJump;
//                 @Attack.started += instance.OnAttack;
//                 @Attack.performed += instance.OnAttack;
//                 @Attack.canceled += instance.OnAttack;
//                 @Defense.started += instance.OnDefense;
//                 @Defense.performed += instance.OnDefense;
//                 @Defense.canceled += instance.OnDefense;
//             }
//         }
//     }
//     public PlayerActions @Player => new PlayerActions(this);
//     private int m_KeyboardSchemeIndex = -1;
//     public InputControlScheme KeyboardScheme
//     {
//         get
//         {
//             if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
//             return asset.controlSchemes[m_KeyboardSchemeIndex];
//         }
//     }
//     private int m_gamepadSchemeIndex = -1;
//     public InputControlScheme gamepadScheme
//     {
//         get
//         {
//             if (m_gamepadSchemeIndex == -1) m_gamepadSchemeIndex = asset.FindControlSchemeIndex("gamepad");
//             return asset.controlSchemes[m_gamepadSchemeIndex];
//         }
//     }
//     public interface IPlayerActions
//     {
//         void OnMouvement(InputAction.CallbackContext context);
//         void OnJump(InputAction.CallbackContext context);
//         void OnAttack(InputAction.CallbackContext context);
//         void OnDefense(InputAction.CallbackContext context);
//     }
// }
