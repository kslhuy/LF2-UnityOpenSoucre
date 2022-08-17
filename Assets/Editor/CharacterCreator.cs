// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UIElements;
// using UnityEditor.UIElements;
// using System;
// using LF2;

// public class CharacterCreator : EditorWindow
// {
//   public VisualTreeAsset m_EditorXML;

//   	[Header("SOs related")]
// 	string[] assetSearchFolders;

//     string[] objectsPaths;
//     private CharacterClass[] characterClass;

//     //   public StyleSheet m_EditorCSS;
//     [MenuItem("Tools/CharacterCreator")]
//     public static void ShowExample()
//     {
//         CharacterCreator wnd =  GetWindow<CharacterCreator>();
//         wnd.titleContent = new GUIContent("CharacterCreator");
//         wnd.minSize = new Vector2(800,600);
//     }
//     // private void OnEnable() {
//     //     m_EditorXML.CloneTree(rootVisualElement);
//     // }
// 	private void OnEnable()
// 	{
// 		assetSearchFolders = new string[1];
// 		assetSearchFolders[0] = "Assets/SOs";
//         m_EditorXML.CloneTree(rootVisualElement);

// 		CreatCharacterListView();
        
// 		// FindDisplaySOs();
// 	}


//     // public void CreateGUI()
//     // {
//     //     // Each editor window contains a root VisualElement object
//     //     VisualElement root = rootVisualElement;

//     //     // // VisualElements objects can contain other VisualElement following a tree hierarchy.
//     //     // VisualElement label = new Label("Hello World! From C#");
//     //     // root.Add(label);

//     //     // Import UXML.

//     //     // var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/CharacterCreator.uxml");
//     //     // VisualElement labelFromUXML = m_EditorXML.Instantiate();
//     //     // root.Add(labelFromUXML);

//     //     // // A stylesheet can be added to a VisualElement.
//     //     // // The style will be applied to the VisualElement and all of its children.
//     //     // var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/CharacterCreator.uss");
//     //     // VisualElement labelWithStyle = new Label("Hello World! With Style");
//     //     // labelWithStyle.styleSheets.Add(styleSheet);
//     //     // root.Add(labelWithStyle);
//     // }
    
//     private void CreatCharacterListView(){
//         FindAllCharactersSOs();

//         ListView characterList = rootVisualElement.Q<ListView>("list-character");

//         // ListView characterList = rootVisualElement.Query<ListView>("list-character").First();
//         characterList.makeItem = () => new Label();
//         characterList.bindItem = (element , i ) => (element as Label).text = characterList[i].name;
    
//         characterList.itemsSource = characterClass;
//         characterList.selectionType = SelectionType.Single;
//     }

//     private void FindAllCharactersSOs()
//     {
// 		var objectsGUIDs = AssetDatabase.FindAssets("t:CharacterClass", assetSearchFolders) as string[];
//         objectsPaths = new string[objectsGUIDs.Length];
//         characterClass = new CharacterClass[objectsGUIDs.Length];

//         for (int i = 0; i < objectsGUIDs.Length; i++)
// 		{
// 			objectsPaths[i] = AssetDatabase.GUIDToAssetPath(objectsGUIDs[i]);
// 			characterClass[i] = (CharacterClass)AssetDatabase.LoadAssetAtPath(objectsPaths[i], typeof(ScriptableObject));
// 			//Debug.Log(objectsGUIDs[i] + ": " + objectsPaths[i] + " - " + i);
// 		}



//     }
// }