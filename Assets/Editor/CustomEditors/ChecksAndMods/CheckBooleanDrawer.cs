using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ChecksAndMods.CheckBoolean))]
public class CheckBooleanDrawer : PropertyDrawer
{
    private static readonly string[] checkSymbolsBoolean = { "", "==", "!=" };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Begin the property
        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout header
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

        // If the foldout is open, draw everything else.
        if (property.isExpanded)
        {
            // Draw the default property field
            EditorGUI.PropertyField(position, property, GUIContent.none, true);

            position.y += 4.5f * EditorGUIUtility.singleLineHeight;

            // Get the target object
            var targetObject = property.serializedObject.targetObject as Object;
            if (targetObject != null)
            {
                var check = property.serializedObject.FindProperty(property.propertyPath);
                if (check != null)
                {
                    // Build the string.
                    int opIdx = check.FindPropertyRelative("opIdx").enumValueIndex;

                    string labelText = string.Format("Is Fetched Value {0} {1} ?",
                        checkSymbolsBoolean[opIdx],
                        check.FindPropertyRelative("compareTo").boolValue);

                    // Handle special cases as needed.
                    if (opIdx == 0) // Are we keeping the numerical value as is?
                        labelText = "Is Fetched Value True?";

                    // Display the label proper.
                    EditorGUI.LabelField(position, labelText, EditorStyles.miniButton);
                }
            }
        }

        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    // Adjust property height.
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutHeight = EditorGUI.GetPropertyHeight(property) + 1.5f * EditorGUIUtility.singleLineHeight;
        return property.isExpanded ? foldoutHeight : EditorGUIUtility.singleLineHeight;
    }
}