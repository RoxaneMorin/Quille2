using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ChecksAndMods;

[CustomPropertyDrawer(typeof(Modulator_FromBool))]
public class ModulatorArithmeticFromBoolDrawer : ModulatorDrawer
{
    SerializedProperty checkOpIdx;
    SerializedProperty compareTo;


    protected override void CollectSubproperties(SerializedProperty property)
    {
        base.CollectSubproperties(property);

        checkOpIdx = property.FindPropertyRelative("checkOpIdx");
        compareTo = property.FindPropertyRelative("compareTo");
    }

    protected override string BuildEquationString(GUIContent label)
    {
        string fetchedValue = target.objectReferenceValue ? target.objectReferenceValue.name : "[Fetched Value]";
        int checkOpIntIdx = checkOpIdx.enumValueIndex;
        int modOpIntIdx = modOpIdx.enumValueIndex;

        // Handle special cases as needed.
        if (modOpIntIdx == 0) // Are we keeping the numerical value as is?
            return "\"Result = Target\"";
        else if (checkOpIntIdx == 0) // Are we checking the value of the modulator itself?
            return string.Format("\"Result = '{0}' ? (Target Value {1} {2}) : Target Value\"", fetchedValue, Symbols.operationSymbolsArithmetic[modOpIntIdx], modifier.floatValue);

        return string.Format("\"Result = ('{0}' {1} {2}) ? (Target Value {3} {4}) : Target Value\"",
            fetchedValue,
            Symbols.comparisonSymbolsBoolean[checkOpIntIdx],
            compareTo.boolValue,
            Symbols.operationSymbolsArithmetic[modOpIntIdx],
            modifier.floatValue);
    }

    protected override void DrawCheckProperties(Rect position, string equationString)
    {
        // Draw the default property fields
        Rect newPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(newPosition, target);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, checkOpIdx);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        if (checkOpIdx.intValue > 0)
        {
            EditorGUI.PropertyField(newPosition, compareTo);
            newPosition.y += EditorGUIUtility.singleLineHeight;

        }
        EditorGUI.PropertyField(newPosition, modOpIdx);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, modifier);
        newPosition.y += EditorGUIUtility.singleLineHeight * 1.15f;

        // Display a preview of the "equation".
        Rect equationRect = new Rect(position.x, newPosition.y, position.width, EditorGUIUtility.singleLineHeight);
        GUIStyle centeredProgressBarBack = new GUIStyle("ProgressBarBack");
        centeredProgressBarBack.alignment = TextAnchor.MiddleCenter;
        EditorGUI.LabelField(equationRect, equationString, centeredProgressBarBack);
    }
}
