using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.SubjectiveNeedSO))]
public class SubjectiveNeedSOEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty needName;
    private SerializedProperty needNameLeft;
    private SerializedProperty needNameRight;
    private SerializedProperty needIconLeft;
    private SerializedProperty needIconRight;
    private SerializedProperty aiPriorityWeighting;
    private SerializedProperty levelFullLeft;
    private SerializedProperty levelFullRight;
    private SerializedProperty defaultChangeRateLeft;
    private SerializedProperty defaultChangeRateRight;
    private SerializedProperty thresholdElatedLeft;
    private SerializedProperty thresholdWarningLeft;
    private SerializedProperty thresholdCriticalLeft;
    private SerializedProperty thresholdElatedRight;
    private SerializedProperty thresholdWarningRight;
    private SerializedProperty thresholdCriticalRight;

    private SerializedProperty baseAIWeightingModulatedByLeft;
    private SerializedProperty baseChangeRateModulatedByLeft;
    private SerializedProperty thresholdsModulatedByLeft;
    private SerializedProperty baseAIWeightingModulatedByRight;
    private SerializedProperty baseChangeRateModulatedByRight;
    private SerializedProperty thresholdsModulatedByRight;

    private void OnEnable()
    {
        scriptProperty = serializedObject.FindProperty("m_Script");

        needName = serializedObject.FindProperty("needName");
        aiPriorityWeighting = serializedObject.FindProperty("aiPriorityWeighting");

        needNameLeft = serializedObject.FindProperty("needNameLeft");
        needNameRight = serializedObject.FindProperty("needNameRight");
        needIconLeft = serializedObject.FindProperty("needIconLeft");
        needIconRight = serializedObject.FindProperty("needIconRight");
        levelFullLeft = serializedObject.FindProperty("levelFullLeft");
        levelFullRight = serializedObject.FindProperty("levelFullRight");
        defaultChangeRateLeft = serializedObject.FindProperty("defaultChangeRateLeft");
        defaultChangeRateRight = serializedObject.FindProperty("defaultChangeRateRight");
        thresholdElatedLeft = serializedObject.FindProperty("thresholdElatedLeft");
        thresholdWarningLeft = serializedObject.FindProperty("thresholdWarningLeft");
        thresholdCriticalLeft = serializedObject.FindProperty("thresholdCriticalLeft");
        thresholdElatedRight = serializedObject.FindProperty("thresholdElatedRight");
        thresholdWarningRight = serializedObject.FindProperty("thresholdWarningRight");
        thresholdCriticalRight = serializedObject.FindProperty("thresholdCriticalRight");

        baseAIWeightingModulatedByLeft = serializedObject.FindProperty("baseAIWeightingModulatedByLeft");
        baseChangeRateModulatedByLeft = serializedObject.FindProperty("baseChangeRateModulatedByLeft");
        thresholdsModulatedByLeft = serializedObject.FindProperty("thresholdsModulatedByLeft");
        baseAIWeightingModulatedByRight = serializedObject.FindProperty("baseAIWeightingModulatedByRight");
        baseChangeRateModulatedByRight = serializedObject.FindProperty("baseChangeRateModulatedByRight");
        thresholdsModulatedByRight = serializedObject.FindProperty("thresholdsModulatedByRight");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw script field.
        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.PropertyField(scriptProperty);

        // Draw generic properties.
        EditorGUILayout.PropertyField(needName);
        EditorGUILayout.PropertyField(aiPriorityWeighting);

        // Draw side labels.
        GUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Left", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Right", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        // Draw the paired properties.
        EditorUtilities.drawAmbidextrousProperty(needNameLeft, needNameRight);
        EditorUtilities.drawAmbidextrousProperty(needIconLeft, needIconRight);
        EditorUtilities.drawAmbidextrousProperty(levelFullLeft, levelFullRight);
        EditorUtilities.drawAmbidextrousProperty(defaultChangeRateLeft, defaultChangeRateRight);
        EditorUtilities.drawAmbidextrousProperty(thresholdElatedLeft, thresholdElatedRight);
        EditorUtilities.drawAmbidextrousProperty(thresholdWarningLeft, thresholdWarningRight);
        EditorUtilities.drawAmbidextrousProperty(thresholdCriticalLeft, thresholdCriticalRight);

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
        EditorUtilities.drawAmbidextrousProperty(baseAIWeightingModulatedByLeft, baseAIWeightingModulatedByRight);
        EditorUtilities.drawAmbidextrousProperty(baseChangeRateModulatedByLeft, baseChangeRateModulatedByRight);
        EditorUtilities.drawAmbidextrousProperty(thresholdsModulatedByLeft, thresholdsModulatedByRight);

        serializedObject.ApplyModifiedProperties();
    }
}
