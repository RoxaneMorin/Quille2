using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ChecksAndMods;
using System;

[CustomPropertyDrawer(typeof(PopulateCheckSubtypesAttribute))]
public class PopulateCheckSubtypesDrawer : PropertyDrawer
{
    SubtypeNames targetCheckType;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // If the property contains a true value, display the expected property field.
        if (property.managedReferenceValue != null)
        {
            // Calculate positions and values.
            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                Rect boxPositionRect = new Rect(position.x - singleLineHeight, position.y + singleLineHeight, position.width + singleLineHeight, position.height - singleLineHeight);
                GUI.Box(boxPositionRect, GUIContent.none, EditorStyles.helpBox);
            }

            Rect newPosition = new Rect(position.x, position.y, position.width - singleLineHeight, position.height);
            EditorGUI.PropertyField(newPosition, property);

            Rect buttonDeleteRect = new Rect(position.x + newPosition.width, position.y, singleLineHeight, singleLineHeight);
            if (GUI.Button(buttonDeleteRect, "X"))
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
        else
        {
            // Calculate positions and values.
            float buttonWidth = (position.width - EditorGUIUtility.labelWidth) - EditorGUIUtility.singleLineHeight;
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            Rect enumPopupRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, buttonWidth, position.height);
            Rect buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth + buttonWidth, position.y, EditorGUIUtility.singleLineHeight, position.height);

            // Draw elements.
            EditorGUI.LabelField(labelRect, "Null. Create:", EditorStyles.boldLabel);
            targetCheckType = (SubtypeNames)EditorGUI.EnumPopup(enumPopupRect, targetCheckType);
            if (GUI.Button(buttonRect, "✓"))
            {
                property.managedReferenceValue = Activator.CreateInstance(Subtypes.subtypesCheck[(int)targetCheckType]);
                property.serializedObject.ApplyModifiedProperties();
                property.isExpanded = true;
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutHeight = EditorGUI.GetPropertyHeight(property, true);
        return property.isExpanded ? foldoutHeight : EditorGUIUtility.singleLineHeight;
    }
}
