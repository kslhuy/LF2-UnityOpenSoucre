using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;
using LF2.Client;
using UnityEditor.UIElements;

public class StateMachineWindowEditor : EditorWindow {

    private string[] assetSearchFolders =new[] {"Assets/LF2_multiplayer/GamePlay/Game/NewStateMachine/CharacterStateSOs"};
    private string[] assetFoldersStateLogicSO=new[] {""};
    private string pathHead =  "Assets/LF2_multiplayer/GamePlay/Game/NewStateMachine/";
    string[] objectsGUIDs;
    string[] objectsPaths;
    ScriptableObject[] objects;
    StateLogicSO[] StateLogicSO_Asset;
    string[] StateLogicSO_GUIDs;
    string[] StateLogicSO_Paths;




    public VisualTreeAsset Vs_Uxml; 
    private ListView m_RightPane;
    private ListView m_LeftPane;
    private ScrollView m_MidPane;
    private Label aleart;
    // private CharacterStateSOs m_CharacterStateSOs;

    [MenuItem("Tools/StateMachineWindow")]
    private static void ShowWindow() {
        var window = GetWindow<StateMachineWindowEditor>();
        window.minSize = new Vector2(800, 300);
        window.titleContent = new GUIContent("StateMachineWindow");

    }

    public void CreateGUI()
    {

        // Create a new VisualElement to be the root of our inspector UI
        var root = rootVisualElement;

        // Load and clone a visual tree from UXML
        Vs_Uxml.CloneTree(root);
        aleart = root.Q<Label>("Avertissement");
    
        GetAssetData();
        m_LeftPane =  root.Query<ListView>("left");
        m_RightPane = root.Query<ListView>("right");
        m_MidPane = root.Query<ScrollView>("mid");
        m_LeftPane.selectionType = SelectionType.Single;
        m_RightPane.selectionType = SelectionType.Single;;


        Func<VisualElement> makeItem = () =>
        {
            var box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.justifyContent =  Justify.SpaceBetween;
            box.style.flexGrow = 1f;
            box.style.flexShrink = 0f;
            box.style.flexBasis = 0f;
            box.Add(new Label());
            box.Add(new Button(() => {
                EditorUtility.FocusProjectWindow();
                if (m_LeftPane.selectedItem == null){
                    aleart.text = "You need to selecte the item CharacterState first ! ";
                    return;
                }
				EditorGUIUtility.PingObject(m_LeftPane.selectedItem as UnityEngine.Object);
                aleart.text = "OK !";
            }) {text = "Locate"});
            return box;
        };

        Action<VisualElement, int> bindItem = (e, i) => (e.ElementAt(0) as Label).text = objects[i].name;


        // Initialize the list view with all sprites' names
        m_LeftPane.itemsSource = objects;
        m_LeftPane.makeItem = makeItem;
        m_LeftPane.bindItem = bindItem;
        // m_LeftPane.bindItem = (item, index) => { (item as Label).text = objects[index].name; };

        m_LeftPane.onItemsChosen += OnStateCharterSOSelectionChange;
        // m_RightPane.onSelectionChange += OnStateSOSelectionChange;
        m_RightPane.onSelectionChange += OnStateSOSelectionChange;





    }


    private void OnStateCharterSOSelectionChange(IEnumerable<object> selectedItems)
    {
        // Clear all previous content from the pane
        m_RightPane.Clear();
        var selectedSO = selectedItems.First() as CharacterStateSOs;
        m_LeftPane.ClearSelection();
        // m_RightPane.ClearSelection();
        // Debug.Log(AssetDatabase.GetAssetPath(selectedSO.StatesSO[0].GetInstanceID()));

        GetAssetStatLogicSO(selectedSO.CharacterType.ToString());





    }
    


    private void OnStateSOSelectionChange(IEnumerable<object> selectedItems)
    {
        m_MidPane.Clear();
        // AssetDatabase.GetAssetPath(selectedItems.First());
        var selectedStateLogicSO =  selectedItems.First() as StateLogicSO; 
        m_MidPane.Add(new InspectorElement(selectedStateLogicSO)); 
        // var serializedObject = new SerializedObject(selectedStateLogicSO);
        // SerializedProperty property = serializedObject.GetIterator();
        // property.NextVisible(true); // Expand the first child.
        // do
        // {
        //     // Create the UIElements PropertyField.
        //     var uieDefaultProperty = new PropertyField(property);

        //     m_MidPane.Add(uieDefaultProperty);
        // }
        // while (property.NextVisible(false));
        // m_MidPane.Bind(serializedObject);


    }

    private void GetAssetData()
    {

        objectsGUIDs = AssetDatabase.FindAssets("t:ScriptableObject", assetSearchFolders) as string[];

        objectsPaths = new string[objectsGUIDs.Length];
        objects = new ScriptableObject[objectsGUIDs.Length];


        for (int i = 0; i < objectsGUIDs.Length; i++)
        {
            objectsPaths[i] = AssetDatabase.GUIDToAssetPath(objectsGUIDs[i]);
            objects[i] = (ScriptableObject)AssetDatabase.LoadAssetAtPath(objectsPaths[i], typeof(ScriptableObject));
        }
    }
    private void GetAssetStatLogicSO(string path)
    {
        // Get a list of all sprites in the project
        assetFoldersStateLogicSO[0] =   (pathHead + path);
        StateLogicSO_GUIDs = AssetDatabase.FindAssets($"t:{nameof(StateLogicSO)}", assetFoldersStateLogicSO) ;

        StateLogicSO_Paths = new string[StateLogicSO_GUIDs.Length];
        StateLogicSO_Asset = new StateLogicSO[StateLogicSO_GUIDs.Length];


        for (int i = 0; i < StateLogicSO_GUIDs.Length; i++)
        {
            StateLogicSO_Paths[i] = AssetDatabase.GUIDToAssetPath(StateLogicSO_GUIDs[i]);
            StateLogicSO_Asset[i] = (StateLogicSO)AssetDatabase.LoadAssetAtPath(StateLogicSO_Paths[i], typeof(StateLogicSO));
        }



        Func<VisualElement> makeItemStateLogic = () =>
        {
            var box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.justifyContent =  Justify.SpaceBetween;
            box.style.flexGrow = 1f;
            box.style.flexShrink = 0f;
            box.style.flexBasis = 0f;
            box.Add(new Label());
            box.Add(new Button(() => {
                EditorUtility.FocusProjectWindow();
                if (m_RightPane.selectedItem == null){
                    aleart.text = "You need to selecte the item StateLogic first ! ";
                    return;
                }
				EditorGUIUtility.PingObject(m_RightPane.selectedItem as UnityEngine.Object);
                aleart.text = "OK !";
            }) {text = "Locate"});
            return box;
        };

        Action<VisualElement, int> bindItemStateLogic = (e, i) => (e.ElementAt(0) as Label).text = StateLogicSO_Asset[i].name;

        m_RightPane.itemsSource = StateLogicSO_Asset;
        m_RightPane.makeItem = makeItemStateLogic;
        m_RightPane.bindItem = bindItemStateLogic;

    }
}