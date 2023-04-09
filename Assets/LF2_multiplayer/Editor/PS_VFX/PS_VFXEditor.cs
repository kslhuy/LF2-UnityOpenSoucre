using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using LF2.Utils;
using UnityEditor.UIElements;

[CustomEditor(typeof(PS_VFX))]
public class PS_VFXEditor : Editor {
    public VisualTreeAsset m_VSTree;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
    //     SerializedProperty property = serializedObject.GetIterator();

        m_VSTree.CloneTree(root);

        var button = new Button() { text = "Reset FrameChecker" };
        root.Add(button);
        button.RegisterCallback<ClickEvent>(evt  =>{
            var pS_VFX = (PS_VFX)target;
            pS_VFX.InitializeFrameStruct();
        });

        var folout = new Foldout() {
            viewDataKey = "PS_VFXFullView" , text = "Full Inspector"
        };
        InspectorElement.FillDefaultInspector(folout , serializedObject , this);
        root.Add(folout);

        return root;
    }
}