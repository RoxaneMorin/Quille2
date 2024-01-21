using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Group logic by FuzzyLogic here https://discussions.unity.com/t/how-to-make-a-readonly-property-in-inspector/75448/4

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class InspectorReadOnlyAttribute : PropertyAttribute {}

public class BeginInspectorReadOnlyGroupAttribute : PropertyAttribute {}

public class EndInspectorReadOnlyGroupAttribute : PropertyAttribute { }