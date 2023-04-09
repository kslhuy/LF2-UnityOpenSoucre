using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LF2;
using System;

[CustomEditor(typeof(FrameStructComponent))]
public class FrameStructComponentEditor : Editor {
    public override VisualElement CreateInspectorGUI()
    {
        
        var root = new VisualElement();

        var frameStructComponent = target as FrameStructComponent;

        SkillsDescription skillsDescription =  frameStructComponent.skillsDescription;
        var skillserializedObject = new SerializedObject(skillsDescription);

        FrameStruct[] _frameStructs = skillsDescription.frameChecker._frameStruct; 



        SerializedProperty property = serializedObject.FindProperty("frameChecker");

        var _scrollView = new ScrollView();



        var  m_Panel = new ListView();


        Func<VisualElement> makeItem = () =>
        {
            var box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.justifyContent =  Justify.SpaceBetween;

            box.style.height = 60;

            box.Add(new Label());
            box.Add(new Image());
            box.Add(new Button(){ text = "Set Sprite" });
            box.Add(new Button(){ text = "Set Collider" });

            return box;
        };
        
        Action<VisualElement, int> bindItem = (e, i) => {
            
            (e.ElementAt(0) as Label).text = i.ToString();

            Image _sprite =  (Image)e.ElementAt(1);
            _sprite.scaleMode = ScaleMode.ScaleToFit;
            _sprite.sprite = (Sprite)_frameStructs[i].sprite ;
    
            ((Button)e.ElementAt(2)).clicked += () => { 
                frameStructComponent.SetSpriteRender(_frameStructs[i].sprite);
            };

            ((Button)e.ElementAt(3)).clicked += () => { 
                frameStructComponent.SetCollider(i);
            };

        };   

        m_Panel.itemsSource = _frameStructs;
        m_Panel.makeItem = makeItem;
        m_Panel.bindItem = bindItem;

        // var skillsDescriptionInspector =  new InspectorElement(serializedObject);

        
        root.Add(m_Panel);


       

        // Draw defaut inspector
        
        
        var folout1 = new Foldout() {
            viewDataKey = "FrameStructComponent" , text = "Full Inspector"
        };

        folout1.Add(new InspectorElement(skillserializedObject) );

        root.Add(folout1);


        var folout2 = new Foldout(){
            text = "Origin Inspector"
        };
        InspectorElement.FillDefaultInspector(folout2 , serializedObject , this);
        root.Add(folout2);
        
        return root;
    }
}


