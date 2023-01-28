// using UnityEditor;
// using UnityEditor.UIElements;
// using UnityEngine.UIElements;
// using UnityEngine;
// using System.Collections.Generic;
// using LF2.Client;
// using System;

// public class CharacterStateSOsEditorWindow : EditorWindow
// {
//     public VisualTreeAsset m_VSTreeXML;
//     private CharacterStateSOs m_CharacterStateSOs;

//     private UnityEditor.Editor _characterStateSOsEditor;


    
//     [MenuItem("Tools/CharacterStateSOsWindow")]
//     public static void ShowDefaultWindow()
//     {
//         var wnd = GetWindow<CharacterStateSOsEditor>();
//         wnd.titleContent = new GUIContent("CharacterStateSOsWindow");
//     }

//     private void OnEnable() {
//         rootVisualElement.Add(m_VSTreeXML.CloneTree());

//         minSize = new Vector2(480, 360);

//     }

//     private void Update()
//     {

//         CreateListView();
//     }

//     private void CreateListView()
//     {
     
//        var assets = FindAssets();
//         ListView listView = rootVisualElement.Q<ListView>(className: "table-list");

//         listView.bindItem = null;

//         listView.itemsSource = assets;
//         listView.itemHeight = 16;

//         listView.bindItem = (element, i) => ((Label)element).text = assets[i].name;
//         listView.selectionType = SelectionType.Single;


//         // listView.onSelectionChanged -= OnListSelectionChanged;
//         // listView.onSelectionChanged += OnListSelectionChanged;

//         if (_characterStateSOsEditor && _characterStateSOsEditor.target)
//             listView.selectedIndex = System.Array.IndexOf(assets, _characterStateSOsEditor.target);
//         // m_CharacterStateSOs = ScriptableObject.CreateInstance<CharacterStateSOs>();
//         // var serializedObject = new SerializedObject(m_CharacterStateSOs);
//         // if (serializedObject == null)
//         //     return;

//         // SerializedProperty property = serializedObject.GetIterator();
//         // property.NextVisible(true); // Expand the first child.

//         // do
//         // {

//         //     // Create the UIElements PropertyField.
//         //     var uieDefaultProperty = new PropertyField(property);

//         //     var row = NewRow( uieDefaultProperty);
//         //     m_ScrollView.Add(row);
//         // }
//         // while (property.NextVisible(false));

//         // // Bind the entire ScrollView. This will actually generate the VisualElement fields from
//         // // the SerializedProperty types.
//         // m_ScrollView.Bind(serializedObject);
//     }

    // private void OnListSelectionChanged(List<object> list)
    // {
    //         IMGUIContainer editor = rootVisualElement.Q<IMGUIContainer>(className: "table-editor");
	// 		editor.onGUIHandler = null;
	// 		if (list.Count == 0)
	// 			return;

	// 		var table = (CharacterStateSOs)list[0];
	// 		if (table == null)
	// 			return;

	// 		if (_characterStateSOsEditor == null)
	// 			_characterStateSOsEditor = UnityEditor.Editor.CreateEditor(table, typeof(TransitionTableEditor));
	// 		else if (_characterStateSOsEditor.target != table)
	// 			UnityEditor.Editor.CreateCachedEditor(table, typeof(TransitionTableEditor), ref _characterStateSOsEditor);

	// 		editor.onGUIHandler = () =>
	// 		{
	// 			if (!_characterStateSOsEditor.target)
	// 			{
	// 				editor.onGUIHandler = null;
	// 				return;
	// 			}

	// 			ListView listView = rootVisualElement.Q<ListView>(className: "table-list");
	// 			if ((Object)listView.selectedItem != _characterStateSOsEditor.target)
	// 			{
	// 				var i = listView.itemsSource.IndexOf(_characterStateSOsEditor.target);
	// 				listView.selectedIndex = i;
	// 				if (i < 0)
	// 				{
	// 					editor.onGUIHandler = null;
	// 					return;
	// 				}
	// 			}

	// 			_characterStateSOsEditor.OnInspectorGUI();
	// 		};    
    //     }

//     private VisualElement NewRow( VisualElement right)
//     {
//         var comparer = m_VSTreeXML.CloneTree();

//         if (right != null)
//             comparer.Q("attack").Add(right);

//         return comparer;
//     }

//     private CharacterStateSOs[] FindAssets()
//     {
//         var guids = AssetDatabase.FindAssets($"t:{nameof(CharacterStateSOs)}");
//         var assets = new CharacterStateSOs[guids.Length];
//         for (int i = 0; i < guids.Length; i++)
//             assets[i] = AssetDatabase.LoadAssetAtPath<CharacterStateSOs>(AssetDatabase.GUIDToAssetPath(guids[i]));

//         return assets;
//     }
// }
