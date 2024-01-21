using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepRangeAttribute : PropertyAttribute
{
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
