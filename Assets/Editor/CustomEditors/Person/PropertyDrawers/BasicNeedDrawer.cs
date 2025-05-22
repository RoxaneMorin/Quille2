using Quille;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Quille.BasicNeed))]
public class BasicNeedDrawer : PropertyDrawer
{
    SerializedProperty localAiPriorityWeighting;
    SerializedProperty levelCurrent;
    SerializedProperty baseChangeRate;
    SerializedProperty currentChangeRate;
    SerializedProperty thresholdElated;
    SerializedProperty thresholdWarning;
    SerializedProperty thresholdCritical;
    SerializedProperty needState;

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

            // Draw title.
            newPosition = new Rect(position.x, position.y + 2.25f * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(newPosition, string.Format("Need ({0}) Variables", needName), EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            // Collect properties.
            localAiPriorityWeighting = property.FindPropertyRelative("localAiPriorityWeighting");
            levelCurrent = property.FindPropertyRelative("levelCurrent");
            baseChangeRate = property.FindPropertyRelative("baseChangeRate");
            currentChangeRate = property.FindPropertyRelative("currentChangeRate");
            thresholdElated = property.FindPropertyRelative("thresholdElated");
            thresholdWarning = property.FindPropertyRelative("thresholdWarning");
            thresholdCritical = property.FindPropertyRelative("thresholdCritical");
            needState = property.FindPropertyRelative("needState");

            // Draw properties.
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, localAiPriorityWeighting);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            using (new EditorGUI.DisabledScope(true))
                EditorGUI.FloatField(newPosition, "Level Full", Constants_Quille.DEFAULT_NEED_LEVEL_FULL);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, levelCurrent);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, baseChangeRate);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, currentChangeRate);
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

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutHeight = EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.singleLineHeight * 0.25F;
        return property.isExpanded ? foldoutHeight : EditorGUIUtility.singleLineHeight;
    }
}
