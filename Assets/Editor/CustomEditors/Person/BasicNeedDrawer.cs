using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Quille.BasicNeed))]
public class BasicNeedDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the need's name.
        // TODO: Find a cleaner way to do this.
        string needName = property.FindPropertyRelative("needNameForUI").stringValue;

        label.text += string.Format(" ({0})", needName);
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
            DrawVariablesBlock(newPosition, property, needName);

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    private Rect DrawVariablesBlock(Rect position, SerializedProperty property, string needName)
    {
        // Draw title.
        Rect newPosition = new Rect(position.x, position.y + 1.2f * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(newPosition, string.Format("Need ({0}) Variables", needName), EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        // Collect properties.
        SerializedProperty localAiPriorityWeighting = property.FindPropertyRelative("localAiPriorityWeighting");
        SerializedProperty levelFull = property.FindPropertyRelative("levelFull");
        SerializedProperty levelCurrent = property.FindPropertyRelative("levelCurrent");
        SerializedProperty baseChangeRate = property.FindPropertyRelative("baseChangeRate");
        SerializedProperty currentChangeRate = property.FindPropertyRelative("currentChangeRate");
        SerializedProperty currentChangeRateScaled = property.FindPropertyRelative("currentChangeRateScaled");
        SerializedProperty thresholdElated = property.FindPropertyRelative("thresholdElated");
        SerializedProperty thresholdWarning = property.FindPropertyRelative("thresholdWarning");
        SerializedProperty thresholdCritical = property.FindPropertyRelative("thresholdCritical");
        SerializedProperty needState = property.FindPropertyRelative("needState");

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
        EditorGUI.PropertyField(newPosition, currentChangeRateScaled);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, thresholdElated);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, thresholdWarning);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, thresholdCritical);

        GUI.enabled = false;
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, needState);
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
