using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ChecksAndMods.ModulatorArithmeticFromBool))]
public class ModulatorArithmeticFromBoolDrawer : PropertyDrawer
{
    private static readonly string[] operationSymbolsArithmetic = { "", "+", "-", "*", "/", "%", "^" };
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

            position.y += 6.8f * EditorGUIUtility.singleLineHeight;

            // Get the target object
            var targetObject = property.serializedObject.targetObject as Object;
            if (targetObject != null)
            {
                var modulator = property.serializedObject.FindProperty(property.propertyPath);
                if (modulator != null)
                {
                    // Build the string.
                    int mainOpIdx = modulator.FindPropertyRelative("checkOpIdx").enumValueIndex;
                    int modOpIdx = modulator.FindPropertyRelative("modOpIdx").enumValueIndex;

                    string labelText = string.Format("Result = (Modulator {0} {1}) ? (Target {2} {3}) : Target",
                        checkSymbolsBoolean[mainOpIdx],
                        modulator.FindPropertyRelative("expectedParam").boolValue,
                        operationSymbolsArithmetic[modOpIdx],
                        modulator.FindPropertyRelative("modifier").floatValue);

                    // Handle special cases as needed.
                    if (modOpIdx == 0) // Are we keeping the numerical value as is?
                        labelText = "Result = Target";
                    else if (mainOpIdx == 0) // Are we checking the value of the modulator itself?
                        labelText = string.Format("Result = Modulator ? (Target {0} {1}) : Target", operationSymbolsArithmetic[modOpIdx], modulator.FindPropertyRelative("modifier").floatValue);

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
