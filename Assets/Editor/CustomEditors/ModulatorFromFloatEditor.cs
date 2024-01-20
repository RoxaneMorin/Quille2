using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ChecksAndMods.ModulatorAlterFloatFromFloat))]
public class ModulatorAlterFloatFromFloatDrawer : PropertyDrawer
{
    private bool foldoutOpen = true;

    private static readonly string[] operationSymbols = { "", "+", "-", "*", "/", "%", "^" };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        foldoutOpen = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), foldoutOpen, label);

        if (foldoutOpen)
        {
            // Draw the default property field
            EditorGUI.PropertyField(position, property, label, true);
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

                    string labelText = string.Format("Result = Modulator {0} (Target {1} {2})",
                        operationSymbols[mainOpIdx],
                        operationSymbols[modOpIdx],
                        modulator.FindPropertyRelative("modifier").floatValue);

                    // Handle special cases as needed.
                    if (mainOpIdx == 0 & modOpIdx == 0)
                        labelText = "Result = Target";
                    else if (mainOpIdx == 0)
                        labelText = "Result = Modulator";
                    else if (modOpIdx == 0)
                        labelText = string.Format("Result = Modulator {0} Target {1}", operationSymbols[mainOpIdx], operationSymbols[modOpIdx]);

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
        return foldoutOpen ? foldoutHeight : EditorGUIUtility.singleLineHeight;
    }
}
