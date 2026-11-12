using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepRangeAttribute : PropertyAttribute
{
    // Property attribute to set a variable as a stepped range in editor.
    // It works similarly to the built-in range attribute, but will snap the input value to the closest multiple of its "step" parameter.


    public float MinValue { get; }
    public float MaxValue { get; }
    public float Step { get; }

    public StepRangeAttribute(float minValue, float maxValue, float step)
    {
        MinValue = minValue;
        MaxValue = maxValue;
        Step = step;
    }
}
