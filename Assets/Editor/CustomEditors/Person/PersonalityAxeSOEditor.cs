using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.PersonalityAxeSO))]
public class PersonalityAxeSOEditor : Editor
{
    private SerializedProperty axeName;
    private SerializedProperty axeNameLeft;
    private SerializedProperty axeNameRight;
    private SerializedProperty axeIconLeft;
    private SerializedProperty axeIconRight;
    private SerializedProperty axeSpan;

    private void OnEnable()
    {
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
        drawAmbidextrousProperty(axeNameLeft, axeNameRight);
        drawAmbidextrousProperty(axeIconLeft, axeIconRight);

        // Add the various checks when I get there.

        serializedObject.ApplyModifiedProperties();
    }

    private void drawAmbidextrousProperty(SerializedProperty left, SerializedProperty right)
    {
        // Draw labels
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(left.displayName);
        GUILayout.FlexibleSpace();
        GUILayout.Label(right.displayName);
        EditorGUILayout.EndHorizontal();

        // Draw properties.
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(left, GUIContent.none);
        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(right, GUIContent.none);
        EditorGUILayout.EndHorizontal();

        //GUILayout.Space(EditorGUIUtility.singleLineHeight/5);
    }
}
