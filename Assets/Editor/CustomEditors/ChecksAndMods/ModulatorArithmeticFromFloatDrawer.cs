using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ChecksAndMods;

[CustomPropertyDrawer(typeof(ModulatorArithmeticFromFloat))]
public class ModulatorArithmeticFromFloatDrawer : PropertyDrawer
{
    SerializedProperty modulator;
    SerializedProperty mainOpIdx;
    SerializedProperty modOpIdx;
    SerializedProperty modifier;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Begin the property
        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout header
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);
        
        // If the foldout is open, draw everything else.
        if (property.isExpanded)
        {
            // Collect the subproperties.
            modulator = property.FindPropertyRelative("modulator");
            mainOpIdx = property.FindPropertyRelative("mainOpIdx");
            modOpIdx = property.FindPropertyRelative("modOpIdx");
            modifier = property.FindPropertyRelative("modifier");

            // Draw the default property fields
            Rect newPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(newPosition, modulator);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, mainOpIdx);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, modOpIdx);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, modifier);
            newPosition.y += EditorGUIUtility.singleLineHeight * 1.15f;

            // Get the target object
            var targetObject = property.serializedObject.targetObject as Object;
            if (targetObject != null)
            {
                //var modulator = property.serializedObject.FindProperty(property.propertyPath);
                if (modulator != null)
                {
                    // Build the string.
                    string fetchedValue = modulator.objectReferenceValue ? modulator.objectReferenceValue.ToString() : "[Fetched Value]";
                    int mainOpIntIdx = mainOpIdx.enumValueIndex;
                    int modOpIntIdx = modOpIdx.enumValueIndex;

                    string labelText = string.Format("Result = Target Value {0} ({1} {2} {3})",
                        Symbols.operationSymbolsArithmetic[mainOpIntIdx],
                        fetchedValue,
                        Symbols.operationSymbolsArithmetic[modOpIntIdx],
                        modifier.floatValue);

                    // Handle special cases as needed.
                    if (mainOpIntIdx == 0)
                        labelText = "Result = Target";
                    else if (modOpIntIdx == 0)
                        labelText = string.Format("Result = Target Value {0} {1}", Symbols.operationSymbolsArithmetic[mainOpIntIdx], fetchedValue);

                    // Display the label proper.
                    EditorGUI.LabelField(newPosition, labelText, EditorStyles.miniButton);
                }
            }
        }

        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    // Adjust property height.
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutHeight = EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.singleLineHeight;
        return property.isExpanded ? foldoutHeight : EditorGUIUtility.singleLineHeight;
    }
}
