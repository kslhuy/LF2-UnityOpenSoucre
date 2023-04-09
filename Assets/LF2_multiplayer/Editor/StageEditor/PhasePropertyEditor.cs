// using UnityEngine;
// using UnityEditor;
// using UnityEngine.UIElements;
// using UnityEditor.UIElements;

// [CustomPropertyDrawer(typeof(Phase))]
// public class PhasePropertyEditor: PropertyDrawer {
//     VisualTreeAsset visualTree;

//     public override VisualElement CreatePropertyGUI(SerializedProperty property)
//     {
       
//         var root = new VisualElement();
//         visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/LF2_multiplayer/Editor/StageEditor/PhaseEditor.uxml");
//         visualTree.CloneTree(root);

//         return root;
//     }


// }
