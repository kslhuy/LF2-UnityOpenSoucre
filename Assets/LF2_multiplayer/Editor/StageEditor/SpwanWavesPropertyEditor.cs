using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(SpwanWaves))]
public class SpwanWavesPropertyEditor: PropertyDrawer {

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
       
        var root = new VisualElement();
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/LF2_multiplayer/Editor/StageEditor/SpwanWavesEditor.uxml");
        visualTree.CloneTree(root);

        return root;
    }


}
