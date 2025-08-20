using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Property attributes to set a variable or series of variables as read only in editor.
// It is somewhat glitchy; I often build the read-only nature of variables into my custom editors instead.


// Group logic by FuzzyLogic here https://discussions.unity.com/t/how-to-make-a-readonly-property-in-inspector/75448/4

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class InspectorReadOnlyAttribute : PropertyAttribute {}

public class BeginInspectorReadOnlyGroupAttribute : PropertyAttribute {}

public class EndInspectorReadOnlyGroupAttribute : PropertyAttribute { }