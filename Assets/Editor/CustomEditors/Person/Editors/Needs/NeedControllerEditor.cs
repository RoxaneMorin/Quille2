using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.Person_NeedController)), CanEditMultipleObjects]
public class NeedControllerEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty myBasicNeeds;
    private SerializedProperty mySubjectiveNeeds;
    private SerializedProperty myBasicNeedsMapped;
    private SerializedProperty mySubjectiveNeedsMapped;

    private bool dictsDisabled = true;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        myBasicNeeds = serializedObject.FindProperty("myBasicNeeds");
        mySubjectiveNeeds = serializedObject.FindProperty("mySubjectiveNeeds");
        myBasicNeedsMapped = serializedObject.FindProperty("myBasicNeedsMapped");
        mySubjectiveNeedsMapped = serializedObject.FindProperty("mySubjectiveNeedsMapped");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw script field.
        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.PropertyField(scriptProperty);

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

        // Draw need array properties.
        EditorGUILayout.LabelField("Need Arrays", style: "ProfilerRightPane");
        EditorGUILayout.PropertyField(myBasicNeeds);
        EditorGUILayout.PropertyField(mySubjectiveNeeds);

        // Option to populate in Editor.
        if (GUILayout.Button("Populate and Init Needs"))
        {
            Quille.Person_NeedController needController = (Quille.Person_NeedController)target;
            needController.Init();
        }
        // Option to clear all in Editor.
        if (GUILayout.Button("Clear All Existing Needs"))
        {
            Quille.Person_NeedController needController = (Quille.Person_NeedController)target;
            needController.ClearArraysAndDicts();
        }

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

        // Display dictionaries, "locked" by default.
        EditorGUILayout.LabelField("Need Dictionaries", style: "ProfilerRightPane");
        using (new EditorGUI.DisabledScope(dictsDisabled)) 
        {
            EditorGUILayout.PropertyField(myBasicNeedsMapped, includeChildren: true);
            EditorGUILayout.PropertyField(mySubjectiveNeedsMapped, includeChildren: true);
        }
        if (GUILayout.Button("(Un)lock Dictionaries"))
        {
            dictsDisabled = !dictsDisabled;
        }

        serializedObject.ApplyModifiedProperties();
    }
}