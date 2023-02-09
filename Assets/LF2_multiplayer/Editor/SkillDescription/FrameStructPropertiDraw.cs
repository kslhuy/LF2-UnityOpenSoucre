using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LF2;

[CustomPropertyDrawer(typeof(FrameStruct))]
public class FrameStructPropertiDraw: PropertyDrawer {
    VisualTreeAsset visualTree;
    // VisualElement left;
    // VisualElement right;
    // VisualElement mid;
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        
        var root = new VisualElement();
        visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/LF2_multiplayer/Editor/SkillDescription/FrameCheckUXML.uxml");
        visualTree.CloneTree(root);
        // left = root.Q<VisualElement>("left");
        // right = root.Q<VisualElement>("right");
        var mid = root.Q<VisualElement>("mid");

        // var splitView = new TwoPaneSplitView(0, 100, TwoPaneSplitViewOrientation.Horizontal);
        
        // root.Add(splitView);

        // var leftPane = new VisualElement();
        // splitView.Add(leftPane);
        // var rightPane = new VisualElement();
        // splitView.Add(rightPane);

        // leftPane.Add(new PropertyField(property.FindPropertyRelative("SpwanObject")));
        // leftPane.Add(new PropertyField(property.FindPropertyRelative("Sound")));
        // leftPane.Add(new PropertyField(property.FindPropertyRelative("wait")));
        // leftPane.Add(new PropertyField(property.FindPropertyRelative("NextFrame")));
        // leftPane.Add(new PropertyField(property.FindPropertyRelative("haveInteraction")));
        // leftPane.Add(new PropertyField(property.FindPropertyRelative("time")));


        // rightPane.Add(new PropertyField(property.FindPropertyRelative("dvx")));
        // rightPane.Add(new PropertyField(property.FindPropertyRelative("dvy")));
        // rightPane.Add(new PropertyField(property.FindPropertyRelative("dvz")));

        // rightPane.Add(new PropertyField(property.FindPropertyRelative("pressed_A")));
        // rightPane.Add(new PropertyField(property.FindPropertyRelative("pressed_J")));
        // rightPane.Add(new PropertyField(property.FindPropertyRelative("pressed_D")));


        // root.Add(splitView);
        var sprite =  property.FindPropertyRelative("sprite");
        // Add a new Image control and display the sprite
        // mid.Add(new PropertyField(sprite));
        
        var spriteImage = new Image();
        spriteImage.scaleMode = ScaleMode.ScaleToFit;
        spriteImage.sprite = (Sprite)sprite.objectReferenceValue ;

        mid.Add(spriteImage);

        // register Event 
        var spriteImgObjField = root.Q<ObjectField>("spriteImg");
        spriteImgObjField.RegisterCallback<ChangeEvent<Object> , VisualElement>(
            ImageChanged,mid);
    

        return root;
    }

    private void ImageChanged(ChangeEvent<UnityEngine.Object> evt, VisualElement midPanel)
    {
        midPanel.Clear();
        var t = evt.newValue;
        // Debug.Log(t);

        if(t == null)
            return;
        var spriteImage = new Image();
        spriteImage.scaleMode = ScaleMode.ScaleToFit;
        
        spriteImage.sprite = (Sprite)t;

        midPanel.Add(spriteImage);
        
    }
}
