using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LF2;

[CustomEditor(typeof(AvatarRegistry))]
public class AvatarRegistryEditor : Editor {
    public override VisualElement CreateInspectorGUI(){
        var root = new VisualElement();

        // var box =  new Box();
        var m_MyToggle = new Toggle("Own") { text = "Set All be Own" };
        m_MyToggle.RegisterValueChangedCallback(OnTestToggleChanged);
        root.Add(m_MyToggle);
        
        var folout = new Foldout() {
            viewDataKey = "Avatar resgiter" , text = "Full Inspector"
        };
        InspectorElement.FillDefaultInspector(folout , serializedObject , this);
        root.Add(folout);
        return root;
        // new Box
    }

    private void OnTestToggleChanged(ChangeEvent<bool> evt)
    {
        var avatarRegistry = (AvatarRegistry)target;
        foreach (var avatar in avatarRegistry.m_Avatars){
            avatar.Own = evt.newValue;
        }
    }
}