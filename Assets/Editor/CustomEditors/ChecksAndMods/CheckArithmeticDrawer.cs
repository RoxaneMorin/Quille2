using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ChecksAndMods.Check_Arithmetic), true)]
public class CheckArithmeticDrawer : CheckDrawer
{
    protected override string BuildEquationString(GUIContent label)
    {
        string fetchedValue = target.objectReferenceValue ? target.objectReferenceValue.name : "[Fetched Value]";
        int opIntIdx = opIdx.enumValueIndex;

        return string.Format("\"Is '{0}' {1} {2} ?\"",
            fetchedValue,
            ChecksAndMods.Symbols.comparisonSymbolsArithmetic[opIntIdx],
            compareTo.floatValue);
    }
}
