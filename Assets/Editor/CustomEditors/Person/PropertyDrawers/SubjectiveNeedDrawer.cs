using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Quille.SubjectiveNeed))]
public class SubjectiveNeedDrawer : PropertyDrawer
{
    SerializedProperty needSO;
    SerializedProperty subneedLeft;
    SerializedProperty subneedRight;

    SerializedProperty needName;
    SerializedProperty subneedNameLeft;
    SerializedProperty subneedNameRight;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Find properties.
        needSO = property.FindPropertyRelative("needSO");
        subneedLeft = property.FindPropertyRelative("subneedLeft");
        subneedRight = property.FindPropertyRelative("subneedRight");

        // TODO: Find a cleaner way to find these?
        needName = property.FindPropertyRelative("needNameForUI");
        subneedNameLeft = property.FindPropertyRelative("needNameLeftForUI");
        subneedNameRight = property.FindPropertyRelative("needNameRightForUI");
        
        // Create header label.
        label.text += string.Format(" ({0}: {1} & {2})", needName.stringValue, subneedNameLeft.stringValue, subneedNameRight.stringValue);
        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout header
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

        // If the foldout is open, draw everything else.
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            // Draw the property field for needSo.
            Rect newPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(newPosition, needSO, true);

            // Draw the subneeds' blocks.
            newPosition = DrawVariablesBlock(newPosition, subneedLeft, "Left", subneedNameLeft.stringValue);
            DrawVariablesBlock(newPosition, subneedRight, "Right", subneedNameRight.stringValue);

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    private Rect DrawVariablesBlock(Rect position, SerializedProperty property, string direction, string subneedName)
    {
        // Draw title.
        Rect newPosition = new Rect(position.x, position.y + 1.25f * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(newPosition, string.Format("{0} Need ({1}) Variables", direction, subneedName), EditorStyles.boldLabel);
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

        newPosition.y += EditorGUIUtility.singleLineHeight;
        using (new EditorGUI.DisabledScope(true))
            EditorGUI.PropertyField(newPosition, needState);

        EditorGUI.indentLevel--;

        return newPosition;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutHeight = EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.singleLineHeight * 15.75f;
        return property.isExpanded ? foldoutHeight : EditorGUIUtility.singleLineHeight;
    }
}