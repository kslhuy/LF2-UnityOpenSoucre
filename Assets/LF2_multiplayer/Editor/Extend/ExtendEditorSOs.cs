using UnityEngine;
using UnityEditor;
namespace LF2.Editor{

    public class ExtendEditorSOs : PropertyAttribute{

    }
    [CustomPropertyDrawer(typeof(ExtendEditorSOs), true)]
    public class ExtendEditorSOsPropertyDrawer : ExtendedScriptableObjectDrawer
    {

    }
}







