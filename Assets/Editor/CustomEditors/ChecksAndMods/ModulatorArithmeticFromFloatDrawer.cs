using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ChecksAndMods;

[CustomPropertyDrawer(typeof(ModulatorArithmeticFromFloat))]
public class ModulatorArithmeticFromFloatDrawer : PropertyDrawer
{
    SerializedProperty modulator;
    SerializedProperty mainOpIdx;
    SerializedProperty modOpIdx;
    SerializedProperty modifier;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Collect the subproperties.
        modulator = property.FindPropertyRelative("modulator");
        mainOpIdx = property.FindPropertyRelative("mainOpIdx");
        modOpIdx = property.FindPropertyRelative("modOpIdx");
        modifier = property.FindPropertyRelative("modifier");
        string equationString;


        // Build the "equation" string.
        string fetchedValue = modulator.objectReferenceValue ? modulator.objectReferenceValue.ToString() : "[Fetched Value]";
        int mainOpIntIdx = mainOpIdx.enumValueIndex;
        int modOpIntIdx = modOpIdx.enumValueIndex;

        equationString = string.Format("\"Result = Target Value {0} ('{1}' {2} {3})\"",
            Symbols.operationSymbolsArithmetic[mainOpIntIdx],
            fetchedValue,
            Symbols.operationSymbolsArithmetic[modOpIntIdx],
            modifier.floatValue);

        // Handle special cases as needed.
        if (mainOpIntIdx == 0)
            equationString = "\"Result = Target\"";
        else if (modOpIntIdx == 0)
            equationString = string.Format("\"Result = Target Value {0} '{1}'\"", Symbols.operationSymbolsArithmetic[mainOpIntIdx], fetchedValue);

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
            EditorGUI.PropertyField(newPosition, modulator);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, mainOpIdx);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, modOpIdx);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, modifier);
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
