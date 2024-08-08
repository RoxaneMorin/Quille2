using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.PersonalityAxeSO))]
public class PersonalityAxeSOEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty axeName;
    private SerializedProperty axeNameLeft;
    private SerializedProperty axeNameRight;
    private SerializedProperty axeIconLeft;
    private SerializedProperty axeIconRight;
    private SerializedProperty axeSpan;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        axeName = serializedObject.FindProperty("axeName");
        axeNameLeft = serializedObject.FindProperty("axeNameLeft");
        axeNameRight = serializedObject.FindProperty("axeNameRight");
        axeIconLeft = serializedObject.FindProperty("axeIconLeft");
        axeIconRight = serializedObject.FindProperty("axeIconRight");
        axeSpan = serializedObject.FindProperty("axeSpan");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw script field.
        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.PropertyField(scriptProperty);

        // Draw generic properties.
        EditorGUILayout.PropertyField(axeName);
        GUI.enabled = false;
        EditorGUILayout.PropertyField(axeSpan);
        GUI.enabled = true;

        // Draw side labels.
        GUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Left", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Right", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        // Draw the paired properties.
        EditorUtilities.drawAmbidextrousProperty(axeNameLeft, axeNameRight);
        EditorUtilities.drawAmbidextrousProperty(axeIconLeft, axeIconRight);

        // Add the various checks when I get there.

        serializedObject.ApplyModifiedProperties();
    }
}
