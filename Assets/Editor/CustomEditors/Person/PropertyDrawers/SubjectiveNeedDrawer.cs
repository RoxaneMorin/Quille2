using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Quille.SubjectiveNeed))]
public class SubjectiveNeedDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the subneed's name.
        // TODO: Find a cleaner way to do this.
        string needName = property.FindPropertyRelative("needNameForUI").stringValue;
        string subneedNameLeft = property.FindPropertyRelative("needNameLeftForUI").stringValue;
        string subneedNameRight = property.FindPropertyRelative("needNameRightForUI").stringValue;

        label.text += string.Format(" ({0}: {1} & {2})", needName, subneedNameLeft, subneedNameRight);
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
            newPosition = DrawVariablesBlock(newPosition, property, "Left", subneedNameLeft);
            DrawVariablesBlock(newPosition, property, "Right", subneedNameRight);

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    private Rect DrawVariablesBlock(Rect position, SerializedProperty property, string direction, string subneedName)
    {
        // Draw title.
        Rect newPosition = new Rect(position.x, position.y + 1.2f * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(newPosition, string.Format("{0} Need ({1}) Variables", direction, subneedName), EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        // Collect properties.
        SerializedProperty localAiPriorityWeighting = property.FindPropertyRelative("localAiPriorityWeighting" + direction);
        SerializedProperty levelFull = property.FindPropertyRelative("levelFull" + direction);
        SerializedProperty levelCurrent = property.FindPropertyRelative("levelCurrent" + direction);
        SerializedProperty baseChangeRate = property.FindPropertyRelative("baseChangeRate" + direction);
        SerializedProperty currentChangeRate = property.FindPropertyRelative("currentChangeRate" + direction);
        SerializedProperty currentChangeRateScaled = property.FindPropertyRelative("currentChangeRate" + direction + "Scaled");
        SerializedProperty thresholdElated = property.FindPropertyRelative("thresholdElated" + direction);
        SerializedProperty thresholdWarning = property.FindPropertyRelative("thresholdWarning" + direction);
        SerializedProperty thresholdCritical = property.FindPropertyRelative("thresholdCritical" + direction);
        SerializedProperty needState = property.FindPropertyRelative("needState" + direction);

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

        newPosition.y += EditorGUIUtility.singleLineHeight;
        using (new EditorGUI.DisabledScope(true))
            EditorGUI.PropertyField(newPosition, needState);

        EditorGUI.indentLevel--;

        return newPosition;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutHeight = EditorGUI.GetPropertyHeight(property) - EditorGUIUtility.singleLineHeight*4;
        return property.isExpanded ? foldoutHeight : EditorGUIUtility.singleLineHeight;
    }
}