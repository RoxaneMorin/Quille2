using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ChecksAndMods;

[CustomPropertyDrawer(typeof(PopulateCheckSubtypesAttribute))]
public class PopulateCheckSubtypesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // If the property contains a true value, display the expected property field.
        if (property.managedReferenceValue != null)
        {
            // Calculate positions and values.
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            //float boxYShift = singleLineHeight * 1.5f + 6f;
            //float propertyYShift = singleLineHeight * 2f;

            //Rect buttonDeleteRect = new Rect(position.x, position.y, singleLineHeight, singleLineHeight);
            //if (GUI.Button(buttonDeleteRect, "X"))
            //{
            //    property.managedReferenceValue = null;
            //    property.serializedObject.ApplyModifiedProperties();
            //}

            //if (property.isExpanded)
            //{
            //    Rect boxPositionRect = new Rect(position.x + boxYShift, position.y + singleLineHeight, position.width - boxYShift, position.height - singleLineHeight - 2f);
            //    GUI.Box(boxPositionRect, GUIContent.none, EditorStyles.helpBox); 
            //}

            //Rect updatedPositionRect = new Rect(position.x + propertyYShift, position.y, position.width - propertyYShift - 4f, position.height);
            //EditorGUI.PropertyField(updatedPositionRect, property);

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
            float labelWidth = EditorGUIUtility.labelWidth;
            float buttonWidth = (position.width - labelWidth) / 2f;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect buttonBoolRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, buttonWidth, position.height);
            Rect buttonArithRect = new Rect(position.x + EditorGUIUtility.labelWidth + buttonWidth, position.y, buttonWidth, position.height);

            // Draw elements.
            EditorGUI.LabelField(labelRect, label.text + " : Create this Check as...", EditorStyles.boldLabel);

            if (GUI.Button(buttonBoolRect, "Boolean Check"))
            {
                property.managedReferenceValue = new CheckBoolean();
                property.serializedObject.ApplyModifiedProperties();
                property.isExpanded = true;
            }
            if (GUI.Button(buttonArithRect, "Arithmetic Check"))
            {
                property.managedReferenceValue = new CheckArithmetic();
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
