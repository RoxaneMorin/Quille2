using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    static class Constants
    {
        public const float THRESHOLD_ROUGHLY_EQUAL = 0.5f;
    }

    static class Operators
    {
        public static Func<float, float, float>[] operationsArithmetic = {
            ((a, b) => a),
            ((a, b) => a + b),
            ((a, b) => a - b),
            ((a, b) => a * b),
            ((a, b) => a / b),
            ((a, b) => a % b),
            ((a, b) => Mathf.Pow(a, b))
        };

        public static Func<bool, bool, bool>[] operationsBoolean =
        {
            ((a, b) => a),
            ((a, b) => a = b),
            ((a, b) => a = !b),
            ((a, b) => a = !a)
        };

        public static Func<float, float, bool>[] checksArithmetic =
        {
            ((a, b) => a == b),
            ((a, b) => a != b),
            ((a, b) => MathF.Abs(a - b) <= Constants.THRESHOLD_ROUGHLY_EQUAL),
            ((a, b) => a > b),
            ((a, b) => a >= b),
            ((a, b) => a < b),
            ((a, b) => a <= b)
        };

        public static Func<bool, bool, bool>[] checksBoolean =
        {
            ((a, b) => a),
            ((a, b) => a == b),
            ((a, b) => a != b)
        };
    }

    // The mapping of Modulators' potential operations. Both names and indices will be used.
    public enum OperationsArithmetic
    {
        PreserveNumber,
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        Pow
    }
    public enum OperationsBoolean
    {
        PreserveBool,
        SetEqualTo,
        SetOppositeTo,
        Invert
    }

    // The mapping of Checks' potential operations. Both names and indices will be used.
    public enum ChecksArithmetic
    {
        Equal,
        NotEqual,
        RoughlyEqual,
        GreaterThan,
        GreaterOrEqualThan,
        SmallerThan,
        SmallerOrEqualThan
    }

    public enum ChecksBoolean
    {
        Value,
        Equal,
        NotEqual
    }
}