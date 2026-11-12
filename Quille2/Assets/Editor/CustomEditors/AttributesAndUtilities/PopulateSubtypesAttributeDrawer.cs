using AmplifyShaderEditor;
using ChecksAndMods;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CustomPropertyDrawer(typeof(PopulateSubtypesAttribute))]
public class PopulateSubtypesAttributeDrawer : PropertyDrawer
{
    // VARIABLES
    private bool inCollection;

    private Type targetBaseType = null;
    private Type[] validSubtypes;
    private string[] subtypeNames;

    private int currentSelection = -1;



    // METHODS
    private void FetchSubtypes()
    {
        targetBaseType = fieldInfo.FieldType;

        if (targetBaseType.IsArray || targetBaseType.HasElementType)
        {
            targetBaseType = targetBaseType.GetElementType();
            inCollection = true;
        }
        else if (targetBaseType.IsGenericType && targetBaseType.GenericTypeArguments.Length > 0)
        {
            targetBaseType = targetBaseType.GenericTypeArguments[0];
            inCollection = true;
        }

        validSubtypes = EditorUtilities.FindValidSubtypes(targetBaseType);
        subtypeNames = EditorUtilities.GetTypeNames(validSubtypes);

        Debug.Log(inCollection);
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;

            if (property.managedReferenceValue != null)
            {
                position.width -= lineHeight;
                EditorGUI.PropertyField(position, property);

                position.x += position.width;
                position.size = new Vector2(lineHeight, lineHeight);
                if (GUI.Button(position, GUIContent.none, new GUIStyle("ToolbarSearchCancelButtonWithJump")))
                {
                    property.managedReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if (targetBaseType == null)
                {
                    FetchSubtypes();
                }
                
                //
                float dropdownWidth = position.width - EditorGUIUtility.labelWidth - lineHeight - 3.0f;

                position.width = EditorGUIUtility.labelWidth;
                EditorGUI.LabelField(position, label);

                position.x += position.width + 3.0f;
                position.width = dropdownWidth;
                GUI.Label(position, "Null", new GUIStyle("TextFieldDropDownText"));

                position.x += position.width;
                position.width = lineHeight;
                EditorGUI.BeginChangeCheck();
                currentSelection = EditorGUI.Popup(position, -1, subtypeNames, new GUIStyle("TextFieldDropDown"));
                if (EditorGUI.EndChangeCheck())
                {
                    property.managedReferenceValue = Activator.CreateInstance(validSubtypes[currentSelection]);
                    property.serializedObject.ApplyModifiedProperties();
                    property.isExpanded = true;
                }
            }
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutHeight = EditorGUI.GetPropertyHeight(property, true);
        return (property.isExpanded ? foldoutHeight : EditorGUIUtility.singleLineHeight);
    }
}
