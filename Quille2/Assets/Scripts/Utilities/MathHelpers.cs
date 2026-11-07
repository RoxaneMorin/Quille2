using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

public static class MathHelpers
{
    // Various useful mathematical functions.
    // To be merged with the class of the same name in the Quille_Construction project.

    static public bool TrySolveQuadratic(float a, float b, float c, out Vector2 solutions)
    {
        solutions = Vector2.zero;

        if (Mathf.Approximately(a, 0.0f))
        {
            return false;
        }

        // x = (-b ± sqrt(b^2 - 4ac)) / 2a

        float discriminant = (b * b) - (4.0f * a * c);
        if (discriminant < 0.0f)
        {
            return false;
        }

        float inSqrt = Mathf.Sqrt(discriminant);
        float denominator = 2 * a;

        solutions = new Vector2((-b + inSqrt) / denominator, (-b - inSqrt) / denominator);
        return true;
    }
}
