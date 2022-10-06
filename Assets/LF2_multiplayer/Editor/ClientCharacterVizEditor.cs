using UnityEngine;
using System.Collections;
using UnityEditor;
using LF2.Client;

[CustomEditor(typeof(ClientCharacterVisualization))]
public class ClientCharacterVizEditor : Editor
{
  // public VisualTreeAsset m_InspectorXML;
  // private ClientCharacterVisualization clientCharacterVisualization;

  // private void OnEnable() {
  //   clientCharacterVisualization = (ClientCharacterVisualization)target;
  //   // DrawDefaultInspector();
  // }

  // public override VisualElement CreateInspectorGUI()
  // {
  //   // Create a new VisualElement to be the root of our inspector UI
  //   VisualElement myInspector = new VisualElement();

  //   // Load from default reference
  //   m_InspectorXML.CloneTree(myInspector);
    
  //   // Get a reference to the default inspector foldout control
  //   VisualElement inspectorFoldout = myInspector.Q("Default_Inspector");

  //   // Attach a default inspector to the foldout
  //   InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);

  //   Button button = myInspector.Q<Button>("TestButton");
  //   button.RegisterCallback<ClickEvent>(evt  =>{
  //     clientCharacterVisualization.TestState();});
    
  //   // Return the finished inspector UI
  //   return myInspector;
    


  //   // // Get a reference to the default inspector foldout control
  //   // VisualElement inspectorFoldout = myInspector.Q("Default_Inspector");

  //   // // Attach a default inspector to the foldout
  //   // InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);

  //   // Return the finished inspector UI
  // }
#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      ClientCharacterVisualization myScript = (ClientCharacterVisualization)target;
      if(GUILayout.Button("Play State"))
      {
          myScript.TestState();
      }
    }
#endif 
}