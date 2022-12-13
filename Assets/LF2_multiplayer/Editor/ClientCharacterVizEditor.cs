using UnityEngine;
using System.Collections;
using UnityEditor;
using LF2.Client;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

[CustomEditor(typeof(ClientCharacterVisualization))]
public class ClientCharacterVizEditor : Editor
{
  private ClientCharacterVisualization clientCharacterVisualization;

  private bool clickedOne = false;

  private void OnEnable() {
    clientCharacterVisualization = (ClientCharacterVisualization)target;
    // DrawDefaultInspector();
  }

  public override VisualElement CreateInspectorGUI(){
    // Create a new VisualElement to be the root of our inspector UI
    VisualElement myInspector = new VisualElement();

    // Attach a default inspector to the foldout
    InspectorElement.FillDefaultInspector(myInspector, serializedObject, this);

    // Create a new Button with an action and give it a style class.
    var button = new Button() { text = "Play State" };
    myInspector.Add(button);
    button.RegisterCallback<ClickEvent>(evt  =>{
      clientCharacterVisualization.TestState();});

    // Create a new Button with an action and give it a style class.
    var button2 = new Button() { text = "Play State Periodic" };
    myInspector.Add(button2);
    button2.RegisterCallback<ClickEvent>(evt  =>{
      clickedOne = !clickedOne;  
      clientCharacterVisualization.TestStatePeriodic(clickedOne);});
    
    // Return the finished inspector UI
    return myInspector;
    
  }

}