using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.PersonalityAxeSO)), CanEditMultipleObjects]
public class PersonalityAxeSOEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty itemName;
    private SerializedProperty menuSortingIndex;
    private SerializedProperty axeSpan;
    private SerializedProperty axeNameLeft;
    private SerializedProperty axeNameRight;
    private SerializedProperty axeIconLeft;
    private SerializedProperty axeIconRight;
    
    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        itemName = serializedObject.FindProperty("itemName");
        menuSortingIndex = serializedObject.FindProperty("menuSortingIndex");

        axeSpan = serializedObject.FindProperty("axeSpan");
        axeNameLeft = serializedObject.FindProperty("axeNameLeft");
        axeNameRight = serializedObject.FindProperty("axeNameRight");
        axeIconLeft = serializedObject.FindProperty("axeIconLeft");
        axeIconRight = serializedObject.FindProperty("axeIconRight");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw script field & read only axe span.
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(scriptProperty);
            EditorGUILayout.PropertyField(axeSpan);
        }
        GUI.enabled = true;
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw generic properties.
        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(menuSortingIndex);
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw side labels.
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Left", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Right", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        // Draw the paired properties.
        EditorUtilities.drawAmbidextrousProperty(axeNameLeft, axeNameRight);
        EditorUtilities.drawAmbidextrousProperty(axeIconLeft, axeIconRight);

        serializedObject.ApplyModifiedProperties();
    }
}
