using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.DriveDomainSO))]
public class DriveDomainSOEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty domainName;
    private SerializedProperty domainIcon;
    private SerializedProperty domainColour;
    private SerializedProperty menuSortingIndex;

    private SerializedProperty itemsInThisDomain;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        domainName = serializedObject.FindProperty("domainName");
        domainIcon = serializedObject.FindProperty("domainIcon");
        domainColour = serializedObject.FindProperty("domainColour");
        menuSortingIndex = serializedObject.FindProperty("menuSortingIndex");

        itemsInThisDomain = serializedObject.FindProperty("itemsInThisDomain");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw script field.
        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.PropertyField(scriptProperty);
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw generic properties.
        EditorGUILayout.PropertyField(domainName);
        EditorGUILayout.PropertyField(domainIcon);
        EditorGUILayout.PropertyField(domainColour);
        EditorGUILayout.PropertyField(menuSortingIndex);

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw itemsInThisDomain.
        EditorGUILayout.PropertyField(itemsInThisDomain);
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

        // Apply modified properties; modified linked objects as needed.
        if (serializedObject.ApplyModifiedProperties())
        {
            Quille.DriveDomainSO targetDriveDomainSO = serializedObject.targetObject as Quille.DriveDomainSO;
            UpdateReferencedDrives(targetDriveDomainSO);
        }
    }

    private void UpdateReferencedDrives(Quille.DriveDomainSO domain)
    {
        foreach (Quille.DriveSO drive in domain.DrivesInThisDomain)
        {
            if (drive && !drive.InDomain(domain))
            {
                drive.AddDomain(domain);
                EditorUtility.SetDirty(drive);
            }
        }
    }
}
