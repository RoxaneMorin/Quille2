using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ChecksAndMods;

[CustomPropertyDrawer(typeof(Modulator_FromFloat))]
public class ModulatorArithmeticFromFloatDrawer : ModulatorDrawer
{
    SerializedProperty mainOpIdx;


    protected override void CollectSubproperties(SerializedProperty property)
    {
        base.CollectSubproperties(property);

        mainOpIdx = property.FindPropertyRelative("mainOpIdx");
    }

    protected override string BuildEquationString(GUIContent label)
    {
        string fetchedValue = target.objectReferenceValue ? target.objectReferenceValue.name : "[Fetched Value]";
        int mainOpIntIdx = mainOpIdx.enumValueIndex;
        int modOpIntIdx = modOpIdx.enumValueIndex;

        // Handle special cases as needed.
        if (mainOpIntIdx == 0)
            return "\"Result = Target\"";
        else if (modOpIntIdx == 0)
            return string.Format("\"Result = Target Value {0} '{1}'\"", Symbols.operationSymbolsArithmetic[mainOpIntIdx], fetchedValue);

        return string.Format("\"Result = Target Value {0} ('{1}' {2} {3})\"",
            Symbols.operationSymbolsArithmetic[mainOpIntIdx],
            fetchedValue,
            Symbols.operationSymbolsArithmetic[modOpIntIdx],
            modifier.floatValue);
    }

    protected override void DrawCheckProperties(Rect position, string equationString)
    {
        // Draw the default property fields
        Rect newPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(newPosition, target);
        newPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(newPosition, mainOpIdx);
        newPosition.y += EditorGUIUtility.singleLineHeight;
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
