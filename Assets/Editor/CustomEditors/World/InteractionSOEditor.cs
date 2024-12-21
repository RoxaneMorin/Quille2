using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using World;

[CustomEditor(typeof(InteractionSO))]
public class InteractionSOEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty interactionName;
    private SerializedProperty defaultBaseScore;
    private SerializedProperty advertisedNeeds;
    private SerializedProperty effectedNeeds;
    private SerializedProperty viabilityChecks;
    private SerializedProperty scoringModulators;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        interactionName = serializedObject.FindProperty("interactionName");
        defaultBaseScore = serializedObject.FindProperty("defaultBaseScore");
        advertisedNeeds = serializedObject.FindProperty("advertisedNeeds");
        effectedNeeds = serializedObject.FindProperty("effectedNeeds");
        viabilityChecks = serializedObject.FindProperty("viabilityChecks");
        scoringModulators = serializedObject.FindProperty("scoringModulators");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw script field.
        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.PropertyField(scriptProperty);
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

        // Draw properties
        EditorGUILayout.PropertyField(interactionName);
        EditorGUILayout.PropertyField(defaultBaseScore);
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight/2f);

        EditorGUILayout.PropertyField(effectedNeeds);
        if (effectedNeeds.arraySize == 0)
        {
            EditorGUILayout.HelpBox("An interaction should effect at least one need.", MessageType.Error);
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);
        }
        EditorGUILayout.PropertyField(advertisedNeeds);
        if (advertisedNeeds.arraySize == 0)
        {
            EditorGUILayout.HelpBox("An interaction should advertise to at least one need.", MessageType.Error);
        }
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

        EditorGUILayout.PropertyField(viabilityChecks);
        if (viabilityChecks.arraySize == 0)
        {
            EditorGUILayout.HelpBox("This interaction will always be viable.", MessageType.Info);
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);
        }
        EditorGUILayout.PropertyField(scoringModulators);
        if (scoringModulators.arraySize == 0)
        {
            EditorGUILayout.HelpBox("This interaction will be scored the same for everyone.", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
