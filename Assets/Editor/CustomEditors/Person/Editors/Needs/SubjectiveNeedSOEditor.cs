using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quille.SubjectiveNeedSO)), CanEditMultipleObjects]
public class SubjectiveNeedSOEditor : Editor
{
    private SerializedProperty scriptProperty;

    private SerializedProperty needName;
    private SerializedProperty leftNeedSO;
    private SerializedProperty rightNeedSO;

    private SerializedObject leftSerializedObject;
    private SerializedObject rightSerializedObject;

    private SerializedProperty needNameLeft;
    private SerializedProperty needNameRight;
    private SerializedProperty needIconLeft;
    private SerializedProperty needIconRight;
    private SerializedProperty aiPriorityWeightingLeft;
    private SerializedProperty aiPriorityWeightingRight;
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
        leftNeedSO = serializedObject.FindProperty("leftNeedSO");
        rightNeedSO = serializedObject.FindProperty("rightNeedSO");

        UpdateSubneedSerializedObjects();
    }

    private void UpdateSubneedSerializedObjects()
    {
        if (leftNeedSO.objectReferenceValue != null)
        {
            leftSerializedObject = new SerializedObject(leftNeedSO.objectReferenceValue);

            needNameLeft = leftSerializedObject.FindProperty("needName");
            needIconLeft = leftSerializedObject.FindProperty("needIcon");
            aiPriorityWeightingLeft = leftSerializedObject.FindProperty("aiPriorityWeighting");
            levelFullLeft = leftSerializedObject.FindProperty("levelFull");
            defaultChangeRateLeft = leftSerializedObject.FindProperty("defaultChangeRate");
            thresholdElatedLeft = leftSerializedObject.FindProperty("thresholdElated");
            thresholdWarningLeft = leftSerializedObject.FindProperty("thresholdWarning");
            thresholdCriticalLeft = leftSerializedObject.FindProperty("thresholdCritical");
            baseAIWeightingModulatedByLeft = leftSerializedObject.FindProperty("baseAIWeightingModulatedBy");
            baseChangeRateModulatedByLeft = leftSerializedObject.FindProperty("baseChangeRateModulatedBy");
            thresholdsModulatedByLeft = leftSerializedObject.FindProperty("thresholdsModulatedBy");
        }
        else
        {
            leftSerializedObject = null;

            needNameLeft = null;
            needIconLeft = null;
            aiPriorityWeightingLeft = null;
            levelFullLeft = null;
            defaultChangeRateLeft = null;
            thresholdElatedLeft = null;
            thresholdWarningLeft = null;
            thresholdCriticalLeft = null;
            baseAIWeightingModulatedByLeft = null;
            baseChangeRateModulatedByLeft = null;
            thresholdsModulatedByLeft = null;
        }

        if (rightNeedSO.objectReferenceValue != null)
        {
            rightSerializedObject = new SerializedObject(rightNeedSO.objectReferenceValue);

            needNameRight = rightSerializedObject.FindProperty("needName");
            needIconRight = rightSerializedObject.FindProperty("needIcon");
            aiPriorityWeightingRight = rightSerializedObject.FindProperty("aiPriorityWeighting");
            levelFullRight = rightSerializedObject.FindProperty("levelFull");
            defaultChangeRateRight = rightSerializedObject.FindProperty("defaultChangeRate");
            thresholdElatedRight = rightSerializedObject.FindProperty("thresholdElated");
            thresholdWarningRight = rightSerializedObject.FindProperty("thresholdWarning");
            thresholdCriticalRight = rightSerializedObject.FindProperty("thresholdCritical");
            baseAIWeightingModulatedByRight = rightSerializedObject.FindProperty("baseAIWeightingModulatedBy");
            baseChangeRateModulatedByRight = rightSerializedObject.FindProperty("baseChangeRateModulatedBy");
            thresholdsModulatedByRight = rightSerializedObject.FindProperty("thresholdsModulatedBy");
        }
        else
        {
            leftSerializedObject = null;

            needNameRight = null;
            needIconRight = null;
            aiPriorityWeightingRight = null;
            levelFullRight = null;
            defaultChangeRateRight = null;
            thresholdElatedRight = null;
            thresholdWarningRight = null;
            thresholdCriticalRight = null;
            baseAIWeightingModulatedByRight = null;
            baseChangeRateModulatedByRight = null;
            thresholdsModulatedByRight = null;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw script field.
        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.PropertyField(scriptProperty);

        // Draw general name field.
        EditorGUILayout.PropertyField(needName);

        // Draw side labels.
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Left", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Right", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        // Draw the paired properties.
        EditorUtilities.drawAmbidextrousProperty(leftNeedSO, rightNeedSO, drawLabels: false);
        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        if (serializedObject.ApplyModifiedProperties())
        {
            UpdateSubneedSerializedObjects();
        }

        if (targets.Length == 1)
        {
            if (leftSerializedObject != null && rightSerializedObject != null)
            {
                leftSerializedObject.Update();
                rightSerializedObject.Update();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical("HelpBox");
                EditorUtilities.drawLabelAndProperty(needNameLeft);
                EditorUtilities.drawLabelAndProperty(needIconLeft);
                GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
                EditorUtilities.drawLabelAndProperty(aiPriorityWeightingLeft);
                EditorUtilities.drawLabelAndProperty(levelFullLeft);
                GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
                EditorUtilities.drawLabelAndProperty(defaultChangeRateLeft);
                GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
                EditorUtilities.drawLabelAndProperty(thresholdElatedLeft);
                EditorUtilities.drawLabelAndProperty(thresholdWarningLeft);
                EditorUtilities.drawLabelAndProperty(thresholdCriticalLeft);
                GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
                EditorUtilities.drawLabelAndProperty(baseAIWeightingModulatedByLeft);
                EditorUtilities.drawLabelAndProperty(baseChangeRateModulatedByLeft);
                EditorUtilities.drawLabelAndProperty(thresholdsModulatedByLeft);
                EditorGUILayout.EndVertical();

                GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

                EditorGUILayout.BeginVertical("HelpBox");
                EditorUtilities.drawLabelAndProperty(needNameRight);
                EditorUtilities.drawLabelAndProperty(needIconRight);
                GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
                EditorUtilities.drawLabelAndProperty(aiPriorityWeightingRight);
                EditorUtilities.drawLabelAndProperty(levelFullRight);
                GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
                EditorUtilities.drawLabelAndProperty(defaultChangeRateRight);
                GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
                EditorUtilities.drawLabelAndProperty(thresholdElatedRight);
                EditorUtilities.drawLabelAndProperty(thresholdWarningRight);
                EditorUtilities.drawLabelAndProperty(thresholdCriticalRight);
                GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
                EditorUtilities.drawLabelAndProperty(baseAIWeightingModulatedByRight);
                EditorUtilities.drawLabelAndProperty(baseChangeRateModulatedByRight);
                EditorUtilities.drawLabelAndProperty(thresholdsModulatedByRight);
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                leftSerializedObject.ApplyModifiedProperties();
                rightSerializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUILayout.HelpBox("Two subneeds should be selected.", MessageType.Error);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Subneeds are only displayed when a single SubjectiveNeedSO is selected.", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
