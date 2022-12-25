// using UnityEngine;
// using UnityEditor;
// using UnityEngine.UIElements;
// using UnityEditor.UIElements;
// using LF2;

// [CustomEditor(typeof(FrameChecker))]
// public class FrameCheckerEditor: Editor {

//     public override VisualElement CreateInspectorGUI()
//     {
//         // Create a VisualElement to hold the UI elements
//         var root = new VisualElement();

//         // Create a button to trigger the Init function
//         var initButton = new Button(Init);
//         initButton.text = "Init";
//         root.Add(initButton);

//         return root;
//     }

//     private void Init()
//     {
//         FrameChecker frameChecker = (FrameChecker)target;
//         frameChecker.initialize();
//     }


// }
