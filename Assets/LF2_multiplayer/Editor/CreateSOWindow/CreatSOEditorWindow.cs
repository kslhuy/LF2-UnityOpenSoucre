using System.Collections;
using System.Collections.Generic;
using LF2.Client;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.IO;
using LF2;
using System.Linq;

public class CreatSOEditorWindow : EditorWindow
{
    // private CO_ScaleToDistance_X CO_ScaleToDistance_X;


    enum TypeConsideration
    {
        DistanceX_InRange,
        DistanceX,
        DistanceY_InRange,
        DistanceY,
        DistanceZ_InRange,
        DistanceZ,
        ScaleToDistance_X,
        ScaleToDistance_Y,
        ScaleToDistance_Z,
        Facing,
        HaveHP,
        HaveMP,
        IsEnemyState,
        IsSeftState,
        RandomeScore,
        SpecEnemyType,
        SupCOScore,

    }

    public VisualTreeAsset Vs_Uxml;

    private EnumField EnumCO;
    private Button createBtn;

    private ListView m_RightPane;
    private ListView m_LeftPane;
    private ListView m_MidPane;

    private VisualElement m_Panel3;

    private VisualElement m_View1;
    private VisualElement m_View2;

    private ScrollView m_VScrobar1;
    private ScrollView m_VScrobar2;
    private ScrollView m_VScrobar3;

    private Label _log;
    // private MultiColumnListView listView;
    private string[] assetFoldersStateAI_SO = new string[] { "Assets/LF2_multiplayer/GamePlay/Game/UtiltyAI/AI_SOs" };

    string[] StateAISO_GUIDs;
    string[] StateAISO_Paths;
    StateActionAI_SO[] StateAISO_Asset;

    string[] SA_GUIDs;
    string[] SA_Paths;
    StateAction[] SA_Asset;


    string[] CO_GUIDs;
    string[] CO_Paths;
    Consideration[] CO_Asset;

    [MenuItem("Tools/CreatSOWindow")]
    private static void ShowWindow()
    {
        var window = GetWindow<CreatSOEditorWindow>();
        window.titleContent = new GUIContent("CreatSOWindow");
        window.minSize = new Vector2(800, 600);

        window.maxSize = new Vector2(1405, 686);

        window.Show();
    }
    public void CreateGUI()
    {

        // Create a new VisualElement to be the root of our inspector UI
        var root = rootVisualElement;

        // Load and clone a visual tree from UXML
        Vs_Uxml.CloneTree(root);

        GetAssetStateActionAIs();

        m_LeftPane = root.Query<ListView>("left");
        m_RightPane = root.Query<ListView>("right");
        m_MidPane = root.Query<ListView>("mid");


        m_Panel3 = root.Query<VisualElement>("panel3");
        m_VScrobar1 = m_Panel3.Q<ScrollView>("midScobar");
        m_VScrobar2 = m_Panel3.Q<ScrollView>("view2");
        m_VScrobar3 = m_Panel3.Q<ScrollView>("view3");


        m_LeftPane.selectionType = SelectionType.Single;
        m_RightPane.selectionType = SelectionType.Single;

        _log = root.Query<Label>("contentLog");


        Func<VisualElement> makeItem = () =>
        {
            var box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.justifyContent = Justify.SpaceBetween;
            box.style.flexGrow = 1f;
            box.style.flexShrink = 0f;
            box.style.flexBasis = 0f;
            box.Add(new Label());
            box.Add(new Button(() =>
            {
                EditorUtility.FocusProjectWindow();
                if (m_LeftPane.selectedItem == null)
                {
                    _log.text = " You need to selecte the item StateActionAI first !";
                    return;
                }
                EditorGUIUtility.PingObject(m_LeftPane.selectedItem as UnityEngine.Object);
                _log.text = " Locate OK !";
            })
            { text = "Locate" });
            return box;
        };

        Action<VisualElement, int> bindItem = (e, i) => (e.ElementAt(0) as Label).text = StateAISO_Asset[i].name;


        // Initialize the list view with all sprites' names
        m_LeftPane.itemsSource = StateAISO_Asset;
        m_LeftPane.makeItem = makeItem;
        m_LeftPane.bindItem = bindItem;
        // m_LeftPane.bindItem = (item, index) => { (item as Label).text = objects[index].name; };

        m_LeftPane.onItemsChosen += LeftPanelSelectionChange;
        m_RightPane.onSelectionChange += RightPanelSelectionChange;
        m_MidPane.onSelectionChange += MidPanelSelectionChange;


        EnumCO = root.Query<EnumField>("COtype");
        EnumCO.Init(TypeConsideration.DistanceX);

        createBtn = root.Query<Button>("Create");
        createBtn.clicked += CreateAssetWithSavePrompt;

    }

    private void GetAssetStateActionAIs()
    {
        // Get a list of all sprites in the project
        // assetFoldersStateAI_SO[0] =   (pathHead + path);
        StateAISO_GUIDs = AssetDatabase.FindAssets($"t:{nameof(StateActionAI_SO)}", assetFoldersStateAI_SO);

        StateAISO_Paths = new string[StateAISO_GUIDs.Length];
        StateAISO_Asset = new StateActionAI_SO[StateAISO_GUIDs.Length];


        for (int i = 0; i < StateAISO_GUIDs.Length; i++)
        {
            StateAISO_Paths[i] = AssetDatabase.GUIDToAssetPath(StateAISO_GUIDs[i]);
            StateAISO_Asset[i] = (StateActionAI_SO)AssetDatabase.LoadAssetAtPath(StateAISO_Paths[i], typeof(StateActionAI_SO));
        }
    }

    private void LeftPanelSelectionChange(IEnumerable<object> selectedItems)
    {
        m_RightPane.Clear();
        m_VScrobar1.Clear();

        var stateActionAI = selectedItems.First() as StateActionAI_SO;

        var statActionAInspector = new InspectorElement(stateActionAI);
        m_VScrobar1.Add(statActionAInspector);
        // List view
        // Debug.Log(AssetDatabase.GetAssetPath(stateActionAI));
        FindAssetStateActions(AssetDatabase.GetAssetPath(stateActionAI));
        Func<VisualElement> makeItem = () =>
        {
            var box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.justifyContent = Justify.SpaceBetween;
            box.style.flexGrow = 1f;
            box.style.flexShrink = 0f;
            box.style.flexBasis = 0f;
            box.Add(new Label());
            box.Add(new Button(() =>
            {
                EditorUtility.FocusProjectWindow();
                if (m_RightPane.selectedItem == null)
                {
                    _log.text = " You need to selecte the item first !";
                    return;
                }
                EditorGUIUtility.PingObject(m_RightPane.selectedItem as UnityEngine.Object);
                _log.text = " Locate OK !";
            })
            { text = "Locate" });
            return box;
        };
        Action<VisualElement, int> bindItem = (e, i) => (e.ElementAt(0) as Label).text = SA_Asset[i].name;

        m_RightPane.itemsSource = SA_Asset;
        m_RightPane.makeItem = makeItem;
        m_RightPane.bindItem = bindItem;

    }

    private void RightPanelSelectionChange(IEnumerable<object> selectedItems)
    {
        m_VScrobar2.Clear();

        var stateActions = selectedItems.First() as StateAction;
        var serializedObject = new SerializedObject(stateActions);

        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true); // Expand the first child.
        do
        {
            // Create the UIElements PropertyField.
            var uieDefaultProperty = new PropertyField(property);
            m_VScrobar2.Add(uieDefaultProperty);
        }
        while (property.NextVisible(false));

        m_VScrobar2.Bind(serializedObject);
        // var stateActionsInspector = new InspectorElement(stateActions);
        // m_VScrobar2.Add(stateActionsInspector);

        FindAssetCO_of_StateAction(AssetDatabase.GetAssetPath(stateActions));
        Func<VisualElement> makeItem = () =>
        {
            var box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.justifyContent = Justify.SpaceBetween;
            box.style.flexGrow = 1f;
            box.style.flexShrink = 0f;
            box.style.flexBasis = 0f;
            box.Add(new Label());
            box.Add(new Button(() =>
            {
                EditorUtility.FocusProjectWindow();
                if (m_MidPane.selectedItem == null)
                {
                    _log.text = " You need to selecte the item first !";
                    return;
                }
                EditorGUIUtility.PingObject(m_MidPane.selectedItem as UnityEngine.Object);
                _log.text = " Locate OK !";
            })
            { text = "Locate" });
            return box;
        };
        Action<VisualElement, int> bindItem = (e, i) => (e.ElementAt(0) as Label).text = CO_Asset[i].name;

        m_MidPane.itemsSource = CO_Asset;
        m_MidPane.makeItem = makeItem;
        m_MidPane.bindItem = bindItem;
    }
    private void MidPanelSelectionChange(IEnumerable<object> selectedItems)
    {
        m_VScrobar3.Clear();

        var considerations = selectedItems.First() as Consideration;
        var serializedObject = new SerializedObject(considerations);
        // var considerationsInspector = new InspectorElement(serializedObject);
        // m_VScrobar3.Add(considerationsInspector);

        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true); // Expand the first child.
        do
        {

            // Create the UIElements PropertyField.
            var uieDefaultProperty = new PropertyField(property);

            m_VScrobar3.Add(uieDefaultProperty);
        }
        while (property.NextVisible(false));

        m_VScrobar3.Bind(serializedObject);

    }

    



    private void FindAssetStateActions(string path)
    {
        path = path.Replace(Path.GetFileName(path), "");
        // Debug.Log(path);
        var pathAsset = new string[] {
            path
        };
        SA_GUIDs = AssetDatabase.FindAssets($"t:{nameof(StateAction)}", pathAsset);

        SA_Paths = new string[SA_GUIDs.Length];
        SA_Asset = new StateAction[SA_GUIDs.Length];


        for (int i = 0; i < SA_GUIDs.Length; i++)
        {
            SA_Paths[i] = AssetDatabase.GUIDToAssetPath(SA_GUIDs[i]);
            SA_Asset[i] = (StateAction)AssetDatabase.LoadAssetAtPath(SA_Paths[i], typeof(StateAction));
        }
    }
    private void FindAssetCO_of_StateAction(string path)
    {
        // Get a list of all sprites in the project
        // assetFoldersStateAI_SO[0] =   (pathHead + path);
        path = path.Replace(Path.GetFileName(path), "");

        var pathAsset = new string[] {
            path
        };
        CO_GUIDs = AssetDatabase.FindAssets($"t:{nameof(Consideration)}", pathAsset);

        CO_Paths = new string[CO_GUIDs.Length];
        CO_Asset = new Consideration[CO_GUIDs.Length];


        for (int i = 0; i < CO_GUIDs.Length; i++)
        {
            CO_Paths[i] = AssetDatabase.GUIDToAssetPath(CO_GUIDs[i]);
            CO_Asset[i] = (Consideration)AssetDatabase.LoadAssetAtPath(CO_Paths[i], typeof(Consideration));
        }
    }



    // 	// Creates a new ScriptableObject via the default Save File panel
    void CreateAssetWithSavePrompt()
    {
        var typeAsset = (TypeConsideration)EnumCO.value;
        ScriptableObject asset;
        switch (typeAsset)
        {
            case TypeConsideration.DistanceX_InRange:
                asset = ScriptableObject.CreateInstance(typeof(CO_DistanceX_InRange));
                break;
            case TypeConsideration.DistanceX:
                asset = ScriptableObject.CreateInstance(typeof(CO_DistanceX));
                break;
            // case TypeConsideration.DistanceY_InRange :
            //     asset = ScriptableObject.CreateInstance(typeof(CO_DistanceY_InRange));
            //     break;
            // case TypeConsideration.DistanceY :
            //     asset = ScriptableObject.CreateInstance(typeof(CO_DistanceY));
            //     break;
            case TypeConsideration.DistanceZ_InRange:
                asset = ScriptableObject.CreateInstance(typeof(CO_DistanceZ_InRange));
                break;
            case TypeConsideration.DistanceZ:
                asset = ScriptableObject.CreateInstance(typeof(CO_DistanceZ));
                break;
            case TypeConsideration.ScaleToDistance_X:
                asset = ScriptableObject.CreateInstance(typeof(CO_ScaleToDistance_X));
                break;
            case TypeConsideration.ScaleToDistance_Y:
                asset = ScriptableObject.CreateInstance(typeof(CO_ScaleToDistance_Y));
                break;
            case TypeConsideration.ScaleToDistance_Z:
                asset = ScriptableObject.CreateInstance(typeof(CO_ScaleToDistance_Z));
                break;
            case TypeConsideration.Facing:
                asset = ScriptableObject.CreateInstance(typeof(CO_Facing));
                break;
            case TypeConsideration.HaveHP:
                asset = ScriptableObject.CreateInstance(typeof(CO_HaveHP));
                break;
            case TypeConsideration.HaveMP:
                asset = ScriptableObject.CreateInstance(typeof(CO_HaveMana));
                break;
            case TypeConsideration.IsEnemyState:
                asset = ScriptableObject.CreateInstance(typeof(CO_IsEnemyState));
                break;
            case TypeConsideration.IsSeftState:
                asset = ScriptableObject.CreateInstance(typeof(CO_IsSeftState));
                break;
            case TypeConsideration.RandomeScore:
                asset = ScriptableObject.CreateInstance(typeof(CO_RandomeScore));
                break;
            case TypeConsideration.SpecEnemyType:
                asset = ScriptableObject.CreateInstance(typeof(CO_SpecEnemyType));
                break;
            case TypeConsideration.SupCOScore:
                asset = ScriptableObject.CreateInstance(typeof(CO_SupCOScore));
                break;
            default:
                asset = null;
                return;
        }


        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", "CO_" + typeAsset.ToString() + ".asset", "asset", "Enter a file name for the ScriptableObject.", "Assets/LF2_multiplayer/GamePlay/Game/UtiltyAI/AI_SOs");
            if (path == "") return;
        }
        else if (Path.GetExtension(path) == "")
        {
            // The selection is a folder, so use it as the save path
            path = path + "/";
        }
        else
        {
            // The selection is a file, so use the parent folder as the save path
            path = path.Replace(Path.GetFileName(path), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/CO_" + typeAsset.ToString() + ".asset");

        int i = 1;
        while (File.Exists(assetPathAndName))
        {
            string newAssetPathAndName = path + "/CO_" + typeAsset.ToString() + "_" + i + ".asset";
            if (!File.Exists(newAssetPathAndName))
            {
                assetPathAndName = newAssetPathAndName;
                break;
            }
            i++;
        }

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        EditorGUIUtility.PingObject(asset);
        Selection.activeObject = asset;

    }

}

// // Developed by Tom Kail at Inkle
// // Released under the MIT Licence as held at https://opensource.org/licenses/MIT

// // Must be placed within a folder named "Editor"
// using System;
// using System.Reflection;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using Object = UnityEngine.Object;

// /// <summary>
// /// Extends how ScriptableObject object references are displayed in the inspector
// /// Shows you all values under the object reference
// /// Also provides a button to create a new ScriptableObject if property is null.
// /// </summary>
// [CustomPropertyDrawer(typeof(ScriptableObject), true)]
// public class ExtendedScriptableObjectDrawer : PropertyDrawer {

// 	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
// 		float totalHeight = EditorGUIUtility.singleLineHeight;
// 		if(property.objectReferenceValue == null || !AreAnySubPropertiesVisible(property)){
// 			return totalHeight;
// 		}
// 		if(property.isExpanded) {
// 			var data = property.objectReferenceValue as ScriptableObject;
// 			if( data == null ) return EditorGUIUtility.singleLineHeight;
// 			SerializedObject serializedObject = new SerializedObject(data);
// 			SerializedProperty prop = serializedObject.GetIterator();
// 			if (prop.NextVisible(true)) {
// 				do {
// 					if(prop.name == "m_Script") continue;
// 					var subProp = serializedObject.FindProperty(prop.name);
// 					float height = EditorGUI.GetPropertyHeight(subProp, null, true) + EditorGUIUtility.standardVerticalSpacing;
// 					totalHeight += height;
// 				}
// 				while (prop.NextVisible(false));
// 			}
// 			// Add a tiny bit of height if open for the background
// 			totalHeight += EditorGUIUtility.standardVerticalSpacing;
// 			serializedObject.Dispose();
// 		}
// 		return totalHeight;
// 	}

// 	const int buttonWidth = 66;

// 	static readonly List<string> ignoreClassFullNames = new List<string>{ "TMPro.TMP_FontAsset" };

// 	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
// 		EditorGUI.BeginProperty (position, label, property);
// 		var type = GetFieldType();

// 		if(type == null || ignoreClassFullNames.Contains(type.FullName)) {
// 			EditorGUI.PropertyField(position, property, label);	
// 			EditorGUI.EndProperty ();
// 			return;
// 		}

// 		ScriptableObject propertySO = null;
// 		if(!property.hasMultipleDifferentValues && property.serializedObject.targetObject != null && property.serializedObject.targetObject is ScriptableObject) {
// 			propertySO = (ScriptableObject)property.serializedObject.targetObject;
// 		}

// 		var propertyRect = Rect.zero;
// 		var guiContent = new GUIContent(property.displayName);
// 		var foldoutRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
// 		if(property.objectReferenceValue != null && AreAnySubPropertiesVisible(property)) {
// 			property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, guiContent, true);
// 		} else {
// 			// So yeah having a foldout look like a label is a weird hack 
// 			// but both code paths seem to need to be a foldout or 
// 			// the object field control goes weird when the codepath changes.
// 			// I guess because foldout is an interactable control of its own and throws off the controlID?
// 			foldoutRect.x += 12;
// 			EditorGUI.Foldout(foldoutRect, property.isExpanded, guiContent, true, EditorStyles.label);
// 		}
// 		var indentedPosition = EditorGUI.IndentedRect(position);
// 		var indentOffset = indentedPosition.x - position.x;
// 		propertyRect = new Rect(position.x + (EditorGUIUtility.labelWidth - indentOffset), position.y, position.width - (EditorGUIUtility.labelWidth - indentOffset), EditorGUIUtility.singleLineHeight);

// 		if(propertySO != null || property.objectReferenceValue == null) {
// 			propertyRect.width -= buttonWidth;
// 		}

// 		EditorGUI.ObjectField(propertyRect, property, type, GUIContent.none);
// 		if (GUI.changed) property.serializedObject.ApplyModifiedProperties();

// 		var buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, EditorGUIUtility.singleLineHeight);

// 		if(property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null) {
// 			var data = (ScriptableObject)property.objectReferenceValue;

// 			if(property.isExpanded) {
// 				// Draw a background that shows us clearly which fields are part of the ScriptableObject
// 				GUI.Box(new Rect(0, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing - 1, Screen.width, position.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing), "");

// 				EditorGUI.indentLevel++;
// 				SerializedObject serializedObject = new SerializedObject(data);

// 				// Iterate over all the values and draw them
// 				SerializedProperty prop = serializedObject.GetIterator();
// 				float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
// 				if (prop.NextVisible(true)) {
// 					do {
// 						// Don't bother drawing the class file
// 						if(prop.name == "m_Script") continue;
// 						float height = EditorGUI.GetPropertyHeight(prop, new GUIContent(prop.displayName), true);
// 						EditorGUI.PropertyField(new Rect(position.x, y, position.width-buttonWidth, height), prop, true);
// 						y += height + EditorGUIUtility.standardVerticalSpacing;
// 					}
// 					while (prop.NextVisible(false));
// 				}
// 				if (GUI.changed)
// 					serializedObject.ApplyModifiedProperties();
// 				serializedObject.Dispose();
// 				EditorGUI.indentLevel--;
// 			}
// 		} else {
// 			if(GUI.Button(buttonRect, "Create")) {
// 				string selectedAssetPath = "Assets";
// 				if(property.serializedObject.targetObject is MonoBehaviour) {
// 					MonoScript ms = MonoScript.FromMonoBehaviour((MonoBehaviour)property.serializedObject.targetObject);
// 					selectedAssetPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath( ms ));
// 				}

// 				property.objectReferenceValue = CreateAssetWithSavePrompt(type, selectedAssetPath);
// 			}
// 		}
// 		property.serializedObject.ApplyModifiedProperties();
// 		EditorGUI.EndProperty ();
// 	}

// 	public static T _GUILayout<T> (string label, T objectReferenceValue, ref bool isExpanded) where T : ScriptableObject {
// 		return _GUILayout<T>(new GUIContent(label), objectReferenceValue, ref isExpanded);
// 	}

// 	public static T _GUILayout<T> (GUIContent label, T objectReferenceValue, ref bool isExpanded) where T : ScriptableObject {
// 		Rect position = EditorGUILayout.BeginVertical();

// 		var propertyRect = Rect.zero;
// 		var guiContent = label;
// 		var foldoutRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
// 		if(objectReferenceValue != null) {
// 			isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, guiContent, true);

// 			var indentedPosition = EditorGUI.IndentedRect(position);
// 			var indentOffset = indentedPosition.x - position.x;
// 			propertyRect = new Rect(position.x + EditorGUIUtility.labelWidth - indentOffset, position.y, position.width - EditorGUIUtility.labelWidth - indentOffset, EditorGUIUtility.singleLineHeight);
// 		} else {
// 			// So yeah having a foldout look like a label is a weird hack 
// 			// but both code paths seem to need to be a foldout or 
// 			// the object field control goes weird when the codepath changes.
// 			// I guess because foldout is an interactable control of its own and throws off the controlID?
// 			foldoutRect.x += 12;
// 			EditorGUI.Foldout(foldoutRect, isExpanded, guiContent, true, EditorStyles.label);

// 			var indentedPosition = EditorGUI.IndentedRect(position);
// 			var indentOffset = indentedPosition.x - position.x;
// 			propertyRect = new Rect(position.x + EditorGUIUtility.labelWidth - indentOffset, position.y, position.width - EditorGUIUtility.labelWidth - indentOffset-60, EditorGUIUtility.singleLineHeight);
// 		}

// 		EditorGUILayout.BeginHorizontal();
// 		objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(" "), objectReferenceValue, typeof(T), false) as T;

// 		if(objectReferenceValue != null) {

// 			EditorGUILayout.EndHorizontal();
// 			if(isExpanded) {
// 				DrawScriptableObjectChildFields(objectReferenceValue);
// 			}
// 		} else {
// 			if(GUILayout.Button("Create", GUILayout.Width(buttonWidth))) {
// 				string selectedAssetPath = "Assets";
// 				var newAsset = CreateAssetWithSavePrompt(typeof(T), selectedAssetPath);
// 				if(newAsset != null) {
// 					objectReferenceValue = (T)newAsset;
// 				}
// 			}
// 			EditorGUILayout.EndHorizontal();
// 		}
// 		EditorGUILayout.EndVertical();
// 		return objectReferenceValue;
// 	}

// 	static void DrawScriptableObjectChildFields<T> (T objectReferenceValue) where T : ScriptableObject {
// 		// Draw a background that shows us clearly which fields are part of the ScriptableObject
// 		EditorGUI.indentLevel++;
// 		EditorGUILayout.BeginVertical(GUI.skin.box);

// 		var serializedObject = new SerializedObject(objectReferenceValue);
// 		// Iterate over all the values and draw them
// 		SerializedProperty prop = serializedObject.GetIterator();
// 		if (prop.NextVisible(true)) {
// 			do {
// 				// Don't bother drawing the class file
// 				if(prop.name == "m_Script") continue;
// 				EditorGUILayout.PropertyField(prop, true);
// 			}
// 			while (prop.NextVisible(false));
// 		}
// 		if (GUI.changed)
// 			serializedObject.ApplyModifiedProperties();
// 		serializedObject.Dispose();
// 		EditorGUILayout.EndVertical();
// 		EditorGUI.indentLevel--;
// 	}

// 	public static T DrawScriptableObjectField<T> (GUIContent label, T objectReferenceValue, ref bool isExpanded) where T : ScriptableObject {
// 		Rect position = EditorGUILayout.BeginVertical();

// 		var propertyRect = Rect.zero;
// 		var guiContent = label;
// 		var foldoutRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
// 		if(objectReferenceValue != null) {
// 			isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, guiContent, true);

// 			var indentedPosition = EditorGUI.IndentedRect(position);
// 			var indentOffset = indentedPosition.x - position.x;
// 			propertyRect = new Rect(position.x + EditorGUIUtility.labelWidth - indentOffset, position.y, position.width - EditorGUIUtility.labelWidth - indentOffset, EditorGUIUtility.singleLineHeight);
// 		} else {
// 			// So yeah having a foldout look like a label is a weird hack 
// 			// but both code paths seem to need to be a foldout or 
// 			// the object field control goes weird when the codepath changes.
// 			// I guess because foldout is an interactable control of its own and throws off the controlID?
// 			foldoutRect.x += 12;
// 			EditorGUI.Foldout(foldoutRect, isExpanded, guiContent, true, EditorStyles.label);

// 			var indentedPosition = EditorGUI.IndentedRect(position);
// 			var indentOffset = indentedPosition.x - position.x;
// 			propertyRect = new Rect(position.x + EditorGUIUtility.labelWidth - indentOffset, position.y, position.width - EditorGUIUtility.labelWidth - indentOffset-60, EditorGUIUtility.singleLineHeight);
// 		}

// 		EditorGUILayout.BeginHorizontal();
// 		objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(" "), objectReferenceValue, typeof(T), false) as T;

// 		if(objectReferenceValue != null) {
// 			EditorGUILayout.EndHorizontal();
// 			if(isExpanded) {

// 			}
// 		} else {
// 			if(GUILayout.Button("Create", GUILayout.Width(buttonWidth))) {
// 				string selectedAssetPath = "Assets";
// 				var newAsset = CreateAssetWithSavePrompt(typeof(T), selectedAssetPath);
// 				if(newAsset != null) {
// 					objectReferenceValue = (T)newAsset;
// 				}
// 			}
// 			EditorGUILayout.EndHorizontal();
// 		}
// 		EditorGUILayout.EndVertical();
// 		return objectReferenceValue;
// 	}

// 	// Creates a new ScriptableObject via the default Save File panel
// 	static ScriptableObject CreateAssetWithSavePrompt (Type type, string path) {
// 		path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", type.Name+".asset", "asset", "Enter a file name for the ScriptableObject.", path);
// 		if (path == "") return null;
// 		ScriptableObject asset = ScriptableObject.CreateInstance(type);
// 		AssetDatabase.CreateAsset (asset, path);
// 		AssetDatabase.SaveAssets ();
// 		AssetDatabase.Refresh();
// 		AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
// 		EditorGUIUtility.PingObject(asset);
// 		return asset;
// 	}

// 	Type GetFieldType () {
// 		Type type = fieldInfo.FieldType;
// 		if(type.IsArray) type = type.GetElementType();
// 		else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) type = type.GetGenericArguments()[0];
// 		return type;
// 	}

// 	static bool AreAnySubPropertiesVisible(SerializedProperty property) {
// 		var data = (ScriptableObject)property.objectReferenceValue;
// 		SerializedObject serializedObject = new SerializedObject(data);
// 		SerializedProperty prop = serializedObject.GetIterator();
// 		while (prop.NextVisible(true)) {
// 			if (prop.name == "m_Script") continue;
// 			return true; //if theres any visible property other than m_script
// 		}
// 		serializedObject.Dispose();
// 		return false;
// 	}
// }
