using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ChecksAndMods;

[CustomPropertyDrawer(typeof(ModulatorArithmeticFromBool))]
public class ModulatorArithmeticFromBoolDrawer : PropertyDrawer
{
    SerializedProperty modulator;
    SerializedProperty checkOpIdx;
    SerializedProperty modOpIdx;
    SerializedProperty compareTo;
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
            checkOpIdx = property.FindPropertyRelative("checkOpIdx");
            modOpIdx = property.FindPropertyRelative("modOpIdx");
            compareTo = property.FindPropertyRelative("compareTo");
            modifier = property.FindPropertyRelative("modifier");

            // Draw the default property fields
            Rect newPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(newPosition, modulator);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, checkOpIdx);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, modOpIdx);
            newPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(newPosition, compareTo);
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
                    int checkOpIntIdx = checkOpIdx.enumValueIndex;
                    int modOpIntIdx = modOpIdx.enumValueIndex;

                    string labelText = string.Format("Result = ({0} {1} {2}) ? (Target Value {3} {4}) : Target Value",
                        fetchedValue,
                        Symbols.comparisonSymbolsBoolean[checkOpIntIdx],
                        compareTo.boolValue,
                        Symbols.operationSymbolsArithmetic[modOpIntIdx],
                        modifier.floatValue);

                    // Handle special cases as needed.
                    if (modOpIntIdx == 0) // Are we keeping the numerical value as is?
                        labelText = "Result = Target";
                    else if (checkOpIntIdx == 0) // Are we checking the value of the modulator itself?
                        labelText = string.Format("Result = {0} ? (Target Value {1} {2}) : Target Value", fetchedValue, Symbols.operationSymbolsArithmetic[modOpIntIdx], modifier.floatValue);

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
