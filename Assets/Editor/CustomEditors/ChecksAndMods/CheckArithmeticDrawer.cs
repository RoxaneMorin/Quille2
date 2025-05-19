using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ChecksAndMods.CheckArithmetic), true)]
public class CheckArithmeticDrawer : PropertyDrawer
{
    SerializedProperty targetItem;
    SerializedProperty opIdx;
    SerializedProperty compareTo;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Collect the subproperties.
        targetItem = property.FindPropertyRelative("targetItem");
        opIdx = property.FindPropertyRelative("opIdx");
        compareTo = property.FindPropertyRelative("compareTo");
        string equationString;


        // Build the string.
        string fetchedValue = targetItem.objectReferenceValue ? targetItem.objectReferenceValue.name : "[Fetched Value]";
        int opIntIdx = opIdx.enumValueIndex;

        equationString = string.Format("\"Is '{0}' {1} {2} ?\"",
            fetchedValue,
            ChecksAndMods.Symbols.comparisonSymbolsArithmetic[opIntIdx],
            compareTo.floatValue);

        label.text = string.Format("{0} : {1}", label.text, equationString);


        // Begin the property
        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout header
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

        // If the foldout is open, draw everything else.
        if (property.isExpanded)
        {
            // Draw the default property fields
            Rect newPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(newPosition, targetItem);
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
