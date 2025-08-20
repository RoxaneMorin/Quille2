using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Group logic by FuzzyLogic here https://discussions.unity.com/t/how-to-make-a-readonly-property-in-inspector/75448/4
// TODO: doesn't work properly on serializedDicts

[CustomPropertyDrawer(typeof(InspectorReadOnlyAttribute))]
public class InspectorReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

[CustomPropertyDrawer(typeof(BeginInspectorReadOnlyGroupAttribute))]
public class BeginInspectorReadOnlyGroupDrawer : DecoratorDrawer
{
    public override float GetHeight() { return 0; }

    public override void OnGUI(Rect position)
    {
        EditorGUI.BeginDisabledGroup(true);
    }

}

[CustomPropertyDrawer(typeof(EndInspectorReadOnlyGroupAttribute))]
public class EndInspectorReadOnlyGroupDrawer : DecoratorDrawer
{
    public override float GetHeight() { return 0; }

    public override void OnGUI(Rect position)
    {
        EditorGUI.EndDisabledGroup();
    }

}