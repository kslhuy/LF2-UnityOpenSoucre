using UnityEditor;

    internal class ScriptTemplates
    {

        public const string TemplatesRoot = "Packages/com.wayn-games.editor.extension/Editor";

        [MenuItem("Assets/Create/WAYN/Editor/ExtendedScriptableObjectDrawer")]
        public static void CreateAbilityEffectType()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                $"{TemplatesRoot}/ExtendedScriptableObjectDrawerTemplate.txt",
                "NewEffect.cs");
        }
       
    }

