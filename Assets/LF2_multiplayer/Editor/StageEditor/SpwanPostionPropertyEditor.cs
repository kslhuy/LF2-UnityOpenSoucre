using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(SpwanPostion))]
public class SpwanPostionPropertyEditor: PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        
        var root = new VisualElement();


        var spwanPoint = new PropertyField(property.FindPropertyRelative("m_Transform"));
        root.Add(spwanPoint);

        var spwanInspector = new Box();
        root.Add(spwanInspector);

        spwanPoint.RegisterCallback<ChangeEvent<Object> , VisualElement>(
            SpwanPointChanged , spwanInspector);
        

        return root;
    }

    private void SpwanPointChanged(ChangeEvent<Object> evt, VisualElement spwanInspector)
    {
        spwanInspector.Clear();

        var t = evt.newValue;
        if (t == null) 
            return;   
        
        spwanInspector.Add(new InspectorElement(t));

    }
}
