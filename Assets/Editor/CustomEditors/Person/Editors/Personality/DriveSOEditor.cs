using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quille.DriveSO))]
public class DriveSOEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty itemName;
    private SerializedProperty itemIcon;
    private SerializedProperty menuSortingIndex;

    private SerializedProperty driveSpan;
    private SerializedProperty driveMidpoint;

    private SerializedProperty inDomains;
    private SerializedProperty incompatiblePersonChecks;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        itemName = serializedObject.FindProperty("itemName");
        itemIcon = serializedObject.FindProperty("itemIcon");
        menuSortingIndex = serializedObject.FindProperty("menuSortingIndex");

        driveSpan = serializedObject.FindProperty("driveSpan");
        driveMidpoint = serializedObject.FindProperty("driveMidpoint");

        inDomains = serializedObject.FindProperty("inDomains");
        incompatiblePersonChecks = serializedObject.FindProperty("incompatiblePersonChecks");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw script field & read only span.
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(scriptProperty);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Trait Steps", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.TextField("0 (Absent)");
            EditorGUILayout.PropertyField(driveMidpoint, GUIContent.none);
            EditorGUILayout.PropertyField(driveSpan, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        GUI.enabled = true;
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw generic properties.
        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(itemIcon);
        EditorGUILayout.PropertyField(menuSortingIndex);
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw inDomains, and handle potential warning message.
        EditorGUILayout.PropertyField(inDomains);
        Quille.DriveSO driveSO = (Quille.DriveSO)target;
        if (driveSO.InDomains == null || driveSO.InDomains.Count == 0)
        {
            EditorGUILayout.HelpBox("A drive should belong to one or more domains.", MessageType.Error);
        }
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw incompatiblePersonChecks.
        EditorGUILayout.PropertyField(incompatiblePersonChecks);
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw clean up buttons.
        if (GUILayout.Button("Clean Up & Fix Dangling References"))
        {
            DrivesMenuUtilities.FixDriveReferences();
        }
        if (GUILayout.Button("Clean Up & Delete Dangling References"))
        {
            DrivesMenuUtilities.DeleteDriveReferences();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
