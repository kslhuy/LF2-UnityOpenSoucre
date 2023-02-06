// using UnityEngine;
// using UnityEditor;
// using LF2;
// using UnityEngine.UIElements;
// using UnityEditor.UIElements;


// [CustomEditor(typeof(StateActionAI_SO))]
// public class StateActionAI_SOEditor : Editor {
//     private VisualTreeAsset Vs_Uxml;

//     public override VisualElement CreateInspectorGUI(){
//         var root = new VisualElement();
        
//         // Load and clone a visual tree from UXML
        
//         VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/LF2_multiplayer/Editor/CreateSOWindow/StateActionAI_SOEditorUXML.uxml");
//         visualTree.CloneTree(root);

//         var mcAttacklistView = root.Q<MultiColumnListView>("AttackListView");
//         var stateActionAI = target as StateActionAI_SO;
//         mcAttacklistView.itemsSource = stateActionAI.A_AttackAvaillable;

//         // Set makeCell
//         var cols = mcAttacklistView.columns; 
//         cols["action"].makeCell = () => new ObjectField();
//         cols["disable"].makeCell = () => new Toggle();
//         cols["showScore"].makeCell = () => new Toggle();


//         // For each column, set Column.bindCell to bind an initialized cell to a data item.
//         cols["action"].bindCell = (VisualElement element, int index) =>
//             {
//                 var l = element as ObjectField;
//                 l.bindingPath = "A_AttackAvaillable.Array.data[" + index + "].A_Availlable";
//                 l.Bind(serializedObject);
//             };
//         cols["disable"].bindCell = (VisualElement element, int index) =>{
//                 var l = element as Toggle;
//                 l.bindingPath = "A_AttackAvaillable.Array.data["+ index +"].Disable";
//                 l.Bind(serializedObject);
//         };
//         cols["showScore"].bindCell = (VisualElement element, int index) =>{
//                 var l = element as Toggle;
//                 l.bindingPath = "A_AttackAvaillable.Array.data["+ index +"].ShowScore";
//                 l.Bind(serializedObject);    
//         };


//         var mcMovementlistView = root.Q<MultiColumnListView>("MovementListView");
//         mcMovementlistView.itemsSource = stateActionAI.A_MoveAvaillable;

//         // Set makeCell
//         var colMs = mcMovementlistView.columns; 
//         colMs["movement"].makeCell = () => new ObjectField();
//         colMs["disableMove"].makeCell = () => new Toggle();
//         colMs["showScoreMove"].makeCell = () => new Toggle();


//         // For each column, set Column.bindCell to bind an initialized cell to a data item.
//         colMs["movement"].bindCell = (VisualElement element, int index) =>
//             {
//                 var l = element as ObjectField;
//                 l.bindingPath = "A_MoveAvaillable.Array.data[" + index + "].A_Availlable";
//                 l.Bind(serializedObject);
//             };
//         colMs["disableMove"].bindCell = (VisualElement element, int index) =>{
//                 var l = element as Toggle;
//                 l.bindingPath = "A_MoveAvaillable.Array.data["+ index +"].Disable";
//                 l.Bind(serializedObject);
//         };
//         colMs["showScoreMove"].bindCell = (VisualElement element, int index) =>{
//                 var l = element as Toggle;
//                 l.bindingPath = "A_MoveAvaillable.Array.data["+ index +"].ShowScore";
//                 l.Bind(serializedObject);    
//         };

//         return root;
        
//     }


// }