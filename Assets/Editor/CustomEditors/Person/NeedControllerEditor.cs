using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.Person_NeedController))]
public class NeedControllerEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty myNeedData;
    private SerializedProperty myBasicNeedsMapped;
    private SerializedProperty mySubjectiveNeedsMapped;

    private bool dictsDisabled = true;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        myNeedData = serializedObject.FindProperty("myNeedData");
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

        // Draw properties.
        EditorGUILayout.LabelField("Need Arrays", style: "ProfilerRightPane");
        EditorGUILayout.PropertyField(myNeedData);

        // Option to populate in Editor.
        if (GUILayout.Button("Populate and Init Needs"))
        {
            Quille.Person_NeedController needController = (Quille.Person_NeedController)target;
            needController.Init();
        }
        // TODO: option to clear all in Editor.



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