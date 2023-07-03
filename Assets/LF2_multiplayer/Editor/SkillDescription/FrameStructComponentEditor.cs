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

        // var _scrollView = new ScrollView();

        VisualElement boxSetHurtBox = new VisualElement(); 
        IntegerField frameDesir = new IntegerField();
        Button btn1 = new Button(() => {
            frameStructComponent.SetHurtBoxFrameNowSame(frameDesir.value);
        }){ text = "Set HurtBox same " + frameDesir.value};

        boxSetHurtBox.Add(frameDesir);                
        boxSetHurtBox.Add(btn1);

        root.Add(boxSetHurtBox);

        var  m_Panel = new ListView();


        Func<VisualElement> makeItem = () =>
        {
            var box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.justifyContent =  Justify.SpaceBetween;

            box.style.height = 60;

            box.Add(new Label());
            box.Add(new Image());
            box.Add(new Button(){ text = "Show Sprite" });
            box.Add(new Button(){ text = "Set HitBox" });
            box.Add(new Button(){ text = "Set HurttBox" });


            return box;
        };
        
        Action<VisualElement, int> bindItem = (e, i) => {
            
            (e.ElementAt(0) as Label).text = i.ToString();

            Image _sprite =  (Image)e.ElementAt(1);
            _sprite.scaleMode = ScaleMode.ScaleToFit;
            _sprite.sprite = (Sprite)_frameStructs[i].sprite ;
    
            ((Button)e.ElementAt(2)).clicked += () => { 
                frameStructComponent.SetSpriteRender(_frameStructs[i].sprite , _frameStructs[i].HitBox , _frameStructs[i].HurtBox);
            };

            ((Button)e.ElementAt(3)).clicked += () => { 
                frameStructComponent.SetHitBox(i);
            };

            ((Button)e.ElementAt(4)).clicked += () => { 
                frameStructComponent.SetHurtBox(i);
            };

        };   


        /// Here to show the images of the in each CharacterStateSOs 
        // m_Panel.itemsSource = _frameStructs;
        // m_Panel.makeItem = makeItem;
        // m_Panel.bindItem = bindItem;
        // root.Add(m_Panel);



        Button btnAniamtionEvent = new Button(() => {
            frameStructComponent.SetHitBoxSame();
        }){ text = "SetHitBoxSame "};


        root.Add(btnAniamtionEvent);
       

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


