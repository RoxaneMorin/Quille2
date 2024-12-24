using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quille.PersonalityTraitSO)), CanEditMultipleObjects]
public class PersonalityTraitSOEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty itemName;
    private SerializedProperty itemIcon;
    private SerializedProperty menuSortingIndex;

    private SerializedProperty traitSpan;
    private SerializedProperty trainMidpoint;

    private SerializedProperty inDomains;
    private SerializedProperty incompatiblePersonChecks;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        itemName = serializedObject.FindProperty("itemName");
        itemIcon = serializedObject.FindProperty("itemIcon");
        menuSortingIndex = serializedObject.FindProperty("menuSortingIndex");

        traitSpan = serializedObject.FindProperty("traitSpan");
        trainMidpoint = serializedObject.FindProperty("trainMidpoint");

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
            EditorGUILayout.PropertyField(trainMidpoint, GUIContent.none);
            EditorGUILayout.PropertyField(traitSpan, GUIContent.none);
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
        Quille.PersonalityTraitSO traitSO = (Quille.PersonalityTraitSO)target;
        if (traitSO.InDomains == null || traitSO.InDomains.Length == 0)
        {
            EditorGUILayout.HelpBox("A trait should belong to one or more domains.", MessageType.Error);
        }
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw incompatiblePersonChecks.
        EditorGUILayout.PropertyField(incompatiblePersonChecks);
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        // Draw clean up buttons.
        if (GUILayout.Button("Clean Up & Fix Dangling References"))
        {
            TraitsMenuUtilities.FixPersonalityTraitReferences();
        }
        if (GUILayout.Button("Clean Up & Delete Dangling References"))
        {
            TraitsMenuUtilities.DeletePersonalityTraitReferences();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
