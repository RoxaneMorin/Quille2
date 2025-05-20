using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ChecksAndMods.Check_Boolean), true)]
public class CheckBooleanDrawer : CheckDrawer
{
    protected override string BuildEquationString(GUIContent label)
    {
        string fetchedValue = target.objectReferenceValue ? target.objectReferenceValue.name : "[Fetched Value]";
        int opIntIdx = opIdx.enumValueIndex;

        string equationString = string.Format("\"Is '{0}' {1} {2} ?\"",
            fetchedValue,
            ChecksAndMods.Symbols.comparisonSymbolsBoolean[opIntIdx],
            compareTo.boolValue);

        // Handle special cases as needed.
        if (opIntIdx == 0) // Are we keeping the numerical value as is?
            equationString = string.Format("\"Is '{0}' True?\"", fetchedValue);

        return equationString;
    }
}

// TODO: revise if CompareTo is removed for these.