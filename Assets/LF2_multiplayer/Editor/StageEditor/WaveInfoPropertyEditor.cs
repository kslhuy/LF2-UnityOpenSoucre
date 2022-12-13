using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(WaveInfo))]
public class WaveInfoPropertyEditor: PropertyDrawer {

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
       
        var root = new VisualElement();
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/LF2_multiplayer/Editor/StageEditor/WaveInfoEditor.uxml");
        visualTree.CloneTree(root);

        return root;
    }


}
