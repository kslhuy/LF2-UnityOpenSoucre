using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace LF2.Editor{

    public class SceneSelectorWindow : EditorWindow
    {

        
        Dictionary<string, string> m_Parents;
        Dictionary<string, string[]> m_Children;
        private ScrollView m_ListContainer;
        private ScrollView m_ListScenceContainer;


        ToolbarBreadcrumbs m_Breadcrumbs;


        static string RootPath => "Assets/Scenes";

        static string TitleClass => "SceneFolder-title";
        static string ItemClass => "Scene-item";
        static string LabelItem => "SceneName-label";


        string[] sceneGuids;
        string[] scenePaths;
        // %#o : is shortcut Ctrl + Shift + O ; 
        [MenuItem("Tools/Scene Selector %#o")]
        static void OpenWindow()
        {
            SceneSelectorWindow window = GetWindow<SceneSelectorWindow>();
            window.titleContent = new GUIContent("Scene Selector");

        }

        // [InitializeOnLoadMethod]
        // static void RegisterCallbacks()
        // {
        //     EditorApplication.playModeStateChanged += ReturnToPreviousScene;
        // }


        void CreateGUI()
        {

            var root = rootVisualElement;
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/CustomEditor/SceneSelectorTool/Editor/breadcrumbs-demo.uss"));

            var toolbar = new Toolbar();
            root.Add(toolbar);

            toolbar.Add(new ToolbarSpacer());

            m_Breadcrumbs = new ToolbarBreadcrumbs();
            toolbar.Add(m_Breadcrumbs);

            var box = new VisualElement();
            m_ListContainer = new ScrollView();
            // m_ListContainer.horizontalScrollerVisibility = ;
            box.Add(m_ListContainer);

            root.Add(box);

            var box2 = new VisualElement();
            m_ListScenceContainer = new ScrollView();
            m_ListScenceContainer.style.color = Color.cyan;
            box2.Add(m_ListScenceContainer);
            root.Add(box2);

            InitElements();

        }

        // static void ReturnToPreviousScene(PlayModeStateChange change)
        // {
        //     if (change == PlayModeStateChange.EnteredEditMode)
        //     {
        //         EditorSceneManager.OpenScene(SceneSelectorSettings.instance.PreviousScenePath, OpenSceneMode.Single);
        //     }
        // }


        VisualElement CreateSceneButton(string scenePath)
        {

            // var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
            var buttonGroup = new VisualElement();
            buttonGroup.style.flexDirection = FlexDirection.Row;
            buttonGroup.style.marginLeft = 3;

            var sceneAsset = AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset));
            var label = new Label($"{sceneAsset.name}");
            
            label.AddToClassList(LabelItem);
            buttonGroup.Add(label);

            var openButton = new Button(() => { EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single); });
            openButton.text = "Open";
            openButton.AddToClassList(ItemClass);
            buttonGroup.Add(openButton);

            var playButton = new Button(() =>
            {
                // SceneSelectorSettings.instance.PreviousScenePath = SceneManager.GetActiveScene().path;
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                EditorApplication.EnterPlaymode();
            });
            playButton.text = "Play";
            playButton.AddToClassList(ItemClass);
            buttonGroup.Add(playButton);
            
            var locateButton = new Button(() =>
            {
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(sceneAsset);
            });
            locateButton.text = "Locate";
            locateButton.AddToClassList(ItemClass);
            buttonGroup.Add(locateButton);

            return buttonGroup;
        }

        void InitElements()
        {
            m_Children = new Dictionary<string, string[]>();
            m_Parents = new Dictionary<string, string>();

            sceneGuids = AssetDatabase.FindAssets("t:Scene" ,new[] {RootPath});

            scenePaths = new string[sceneGuids.Length];

            for (int i = 0; i < sceneGuids.Length; i++){
                scenePaths[i] = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
            }


            // var path = scenePaths[1].Replace("/"+Path.GetFileName(scenePaths[1]), "");



            var folderPaths =  AssetDatabase.GetSubFolders(RootPath);
            AddChildren(RootPath, folderPaths);
            LoadContentByPath(RootPath+"/Main");
        }

        void AddChildren(string title, string[] children)
        {
            m_Children[title] = children;
            foreach (var child in children)
            {
                m_Parents[child] = title;
            }
        }

        void LoadContentByPath(string folderPath)
        {
            // Clear item in Container

            m_ListContainer.Clear();
            m_ListScenceContainer.Clear();

            // Make label for the container 
            var contentName =  Path.GetFileName(folderPath);                  
            var label = new Label(contentName);
            label.AddToClassList(TitleClass);
            m_ListContainer.Add(label);
            
            // Make button of all the subFolder of curent folder (folderPath)
            if ( m_Children.ContainsKey(folderPath)){
                foreach (var child in m_Children[folderPath])
                {
                    bool hasChildren = m_Children.ContainsKey(child);

                    Action clickEvent = null;
                    clickEvent = () => LoadContentByPath(child);
                    var button = new Button(clickEvent) {text = Path.GetFileName(child) };
                    // button.SetEnabled(hasChildren);
                    button.AddToClassList(ItemClass);
                    m_ListContainer.Add(button);
                }
            }
            // Make BreadCrums (tool bar) to track where is the current folder and 
            // also have posibility to return back to the parent folder
            BuildBreadCrumbs(folderPath);

            foreach (var scenePath in scenePaths ){
                var path = scenePath.Replace("/"+Path.GetFileName(scenePath), "");
                if (folderPath == path){
                    m_ListScenceContainer.Add(CreateSceneButton(scenePath));
                }
            }

            
        }
        void BuildBreadCrumbs(string contentPath)
        {
            m_Breadcrumbs.Clear();

            List<string> parentPaths = new List<string>();
            var contentName =  Path.GetFileName(contentPath);   
            // find all Parent Folder of the current contentPath               
            for (var c = contentPath; m_Parents.TryGetValue(c, out var parent); c = parent)
            {
                parentPaths.Add(parent);
            }
            // **Reverse order** and make all Parent Folder and push it to the end
            foreach (string parentPath in Enumerable.Reverse(parentPaths))
            {
                m_Breadcrumbs.PushItem(Path.GetFileName(parentPath), () => {
                    // Resgist Event if we wanna back to the parent folder   
                    LoadContentByPath(parentPath);
                    });
            }
            // make the current folder , and show in the last of all Folder  
            m_Breadcrumbs.PushItem(contentName);
        }

        /* Exemple to get SubFolder  */
        // static void SubFolderExample()
        // {
        //     //This method prints out the entire folder list of a project into the console
        //     var folders = AssetDatabase.GetSubFolders("Assets");
        //     foreach (var folder in folders)
        //     {
        //         Recursive(folder);
        //     }
        // }

        // static void Recursive(string folder)
        // {
        //     Debug.Log(folder);
        //     var folders = AssetDatabase.GetSubFolders(folder);
        //     foreach (var fld in folders)
        //     {
        //         Recursive(fld);
        //     }
        // }
    }
}