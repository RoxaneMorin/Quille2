using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Quille.SubjectiveNeed))]
public class SubjectiveNeedDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout header
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

        // If the foldout is open, draw everything else.
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            // Draw the property field for needSo.
            Rect newPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("needSO"), true);

            // Draw the sided variables.
            newPosition = DrawVariablesBlock(newPosition, property, "Left");
            DrawVariablesBlock(newPosition, property, "Right");

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    private Rect DrawVariablesBlock(Rect position, SerializedProperty property, string direction)
    {
        // Draw title.
        Rect newPosition = new Rect(position.x, position.y + 1.2f * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(newPosition, direction + " Need Variables", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        // Collect properties.
        SerializedProperty localAiPriorityWeighting = property.FindPropertyRelative("localAiPriorityWeighting" + direction);
        SerializedProperty levelFull = property.FindPropertyRelative("levelFull" + direction);
        SerializedProperty levelCurrent = property.FindPropertyRelative("levelCurrent" + direction);
        SerializedProperty baseChangeRate = property.FindPropertyRelative("baseChangeRate" + direction);
        SerializedProperty currentChangeRate = property.FindPropertyRelative("currentChangeRate" + direction);
        SerializedProperty thresholdWarning = property.FindPropertyRelative("thresholdWarning" + direction);
        SerializedProperty thresholdCritical = property.FindPropertyRelative("thresholdCritical" + direction);
        SerializedProperty isWarning = property.FindPropertyRelative("isWarning" + direction);
        SerializedProperty isCritical = property.FindPropertyRelative("isCritical" + direction);
        SerializedProperty isFailure = property.FindPropertyRelative("isFailure" + direction);

        // Draw properties.
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, localAiPriorityWeighting);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, levelFull);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, levelCurrent);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, baseChangeRate);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, currentChangeRate);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, thresholdWarning);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, thresholdCritical);

        GUI.enabled = false;
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, isWarning);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, isCritical);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, isFailure);
        GUI.enabled = true;

        EditorGUI.indentLevel--;

        return newPosition;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutHeight = EditorGUI.GetPropertyHeight(property) - EditorGUIUtility.singleLineHeight;
        return property.isExpanded ? foldoutHeight : EditorGUIUtility.singleLineHeight;
    }
}