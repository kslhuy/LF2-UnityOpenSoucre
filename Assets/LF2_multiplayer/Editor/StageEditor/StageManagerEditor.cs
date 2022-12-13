using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(StageManager))]
public class StageManagerEditor: Editor {
    public override VisualElement CreateInspectorGUI()
    {
        
        var root = new VisualElement();

        
        // Draw defaut inspector
        
        var folout = new Foldout() {
            viewDataKey = "SpwanManagerFullView" , text = "Full Inspector"
        };
        InspectorElement.FillDefaultInspector(folout , serializedObject , this);
        root.Add(folout);
        return root;
    }

}
