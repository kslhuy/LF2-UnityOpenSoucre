// using UnityEngine;
// using UnityEditor;
// using UnityEngine.UIElements;
// using UnityEditor.UIElements;
// using LF2;

// [CustomPropertyDrawer(typeof(FrameChecker))]
// public class FrameCheckerPropertiDraw: PropertyDrawer {
//     VisualTreeAsset visualTree;

//     public override VisualElement CreatePropertyGUI(SerializedProperty property)
//     {
       
//         var root = new VisualElement();
//         visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/LF2_multiplayer/Editor/SkillDescription/FrameCheckUXML.uxml");
//         visualTree.CloneTree(root);

//         return root;
//     }


// }
