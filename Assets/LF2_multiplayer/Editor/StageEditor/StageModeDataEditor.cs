using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(StageModeData))]
public class StageModeDataEditor: Editor {

    public VisualTreeAsset m_UXML;
    public override VisualElement CreateInspectorGUI()
    {
        
        var root = new VisualElement();

        m_UXML.CloneTree(root);

        // Draw defaut inspector
        
        // var folout = new Foldout() {
        //     viewDataKey = "SpwanManagerFullView" , text = "Full Inspector"
        // };
        // InspectorElement.FillDefaultInspector(folout , serializedObject , this);
        // root.Add(folout);
        return root;
    }

}
