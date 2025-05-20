using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CheckDrawer : PropertyDrawer
{
    protected SerializedProperty target;
    protected SerializedProperty opIdx;
    protected SerializedProperty compareTo;

    protected virtual void CollectSubproperties(SerializedProperty property)
    {
        target = property.FindPropertyRelative("target");
        opIdx = property.FindPropertyRelative("opIdx");
        compareTo = property.FindPropertyRelative("compareTo");
    }


    protected abstract string BuildEquationString(GUIContent label);

    protected virtual void DrawCheckProperties(Rect position, string equationString)
    {
        // Draw the default property fields
        Rect newPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(newPosition, target);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, opIdx);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, compareTo);
        newPosition.y += EditorGUIUtility.singleLineHeight * 1.15f;

        // Display a preview of the "equation".
        Rect equationRect = new Rect(position.x, newPosition.y, position.width, EditorGUIUtility.singleLineHeight);
        GUIStyle centeredProgressBarBack = new GUIStyle("ProgressBarBack");
        centeredProgressBarBack.alignment = TextAnchor.MiddleCenter;
        EditorGUI.LabelField(equationRect, equationString, centeredProgressBarBack);
    }


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
