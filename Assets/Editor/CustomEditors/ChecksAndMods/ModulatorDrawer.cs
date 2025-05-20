using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ModulatorDrawer : PropertyDrawer
{
    protected SerializedProperty target;
    protected SerializedProperty modOpIdx;
    protected SerializedProperty modifier;

    protected virtual void CollectSubproperties(SerializedProperty property)
    {
        target = property.FindPropertyRelative("target");
        modOpIdx = property.FindPropertyRelative("modOpIdx");
        modifier = property.FindPropertyRelative("modifier");
    }


    protected abstract string BuildEquationString(GUIContent label);
    protected abstract void DrawCheckProperties(Rect position, string equationString);


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Collect the subproperties.
        CollectSubproperties(property);

        // Build the string.
        string equationString = BuildEquationString(label);
        label.text = string.Format("{0} : {1}", label.text, equationString); ;

        // Begin the property
        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout header
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

        // If the foldout is open, draw everything else.
        if (property.isExpanded)
        {
            DrawCheckProperties(position, equationString);
        }

        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    // Adjust property height.
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutHeight = EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.singleLineHeight;
        return property.isExpanded ? foldoutHeight : EditorGUIUtility.singleLineHeight;
    }
}
