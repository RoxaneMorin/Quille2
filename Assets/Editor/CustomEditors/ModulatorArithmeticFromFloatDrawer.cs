using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ChecksAndMods.ModulatorArithmeticFromFloat))]
public class ModulatorArithmeticFromFloatDrawer : PropertyDrawer
{
    private static readonly string[] operationSymbols = { "", "+", "-", "*", "/", "%", "^" };

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

            position.y += 5.6f * EditorGUIUtility.singleLineHeight;

            // Get the target object
            var targetObject = property.serializedObject.targetObject as Object;
            if (targetObject != null)
            {
                var modulator = property.serializedObject.FindProperty(property.propertyPath);
                if (modulator != null)
                {
                    // Build the string.
                    int mainOpIdx = modulator.FindPropertyRelative("mainOpIdx").enumValueIndex;
                    int modOpIdx = modulator.FindPropertyRelative("modOpIdx").enumValueIndex;

                    string labelText = string.Format("Result = Target {0} (Modulator {1} {2})",
                        operationSymbols[mainOpIdx],
                        operationSymbols[modOpIdx],
                        modulator.FindPropertyRelative("modifier").floatValue);

                    // Handle special cases as needed.
                    if (mainOpIdx == 0)
                        labelText = "Result = Target";
                    else if (modOpIdx == 0)
                        labelText = string.Format("Result = Target {0} Modulator", operationSymbols[mainOpIdx], operationSymbols[modOpIdx]);

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
