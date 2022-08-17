// using UnityEditor;
// using UnityEditor.UIElements;
// using UnityEngine.UIElements;
// using UnityEngine;
// using System.Collections.Generic;
// using LF2.Client;
// using System;

// [CustomEditor(typeof(CharacterStateSOs))]
// public class CharacterStateSOsEditor : Editor
// {
//     public VisualTreeAsset m_VSTreeXML;
//     private CharacterStateSOs m_CharacterStateSOs;


//     ScrollView m_ScrollView;


//     private void OnEnable() {

        

//     }

//     public override VisualElement CreateInspectorGUI(){
//         // Create a new VisualElement to be the root of our inspector UI
//         VisualElement myRootInspector = new VisualElement();
//         m_VSTreeXML.CloneTree(myRootInspector);
//         m_CharacterStateSOs = (CharacterStateSOs)target;
        
//         // var popup = new EnumField(m_CharacterStateSOs.CharacterType);
//         // myRootInspector.Add(popup);
//         // var serializedObject = new SerializedObject(m_CharacterStateSOs);
        
//         VisualElement AttackLogic_VizE = myRootInspector.Q("attack");
//         AttackLogic_VizE.Add(new PropertyField(serializedObject.FindProperty("StatesSO")));
        
//         m_ScrollView = new ScrollView();
//         m_ScrollView.viewDataKey = "main-scroll-view";
//         m_ScrollView.name = "main-scroll-view";
//         m_ScrollView.verticalScroller.slider.pageSize = 100;
//         AttackLogic_VizE.Add(m_ScrollView);

//         VisualElement MovementLogic_VizE = myRootInspector.Q("movement");

//         VisualElement HurtLogic_VizE = myRootInspector.Q("hurt");

//         VisualElement SpecialLogic_VizE = myRootInspector.Q("special");

//         // myRootInspector.Add(AttackLogic_VizE);
//         // foreach(StateLogicSO stateLogicSO in m_CharacterStateSOs.StatesSO){
//         //     if (stateLogicSO.Logic == LF2.StateLogic.Movement){

//         //     }
//         // }

//         // new InspectorElement(inpsectorAttackLogic )

   
//         // Attach a default inspector to the foldout


//         // Button button = myInspector.Q<Button>("TestButton");
//         // button.RegisterCallback<ClickEvent>(evt  =>{
//         //     clientCharacterVisualization.TestState();});

//         // // Return the finished inspector UI
//         return myRootInspector;

//     }



    

// }
