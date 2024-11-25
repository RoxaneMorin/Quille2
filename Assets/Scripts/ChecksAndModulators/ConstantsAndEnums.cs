using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    // Constants, enums and delegates used by the system of checks and modulators.
    // Order is important! Enums and Funcs reflect each others' indices.


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

        public static Func<float, float, bool>[] comparisonsArithmetic =
        {
            ((a, b) => a == b),
            ((a, b) => a != b),
            ((a, b) => MathF.Abs(a - b) <= Constants.THRESHOLD_ROUGHLY_EQUAL),
            ((a, b) => a > b),
            ((a, b) => a >= b),
            ((a, b) => a < b),
            ((a, b) => a <= b)
        };

        public static Func<bool, bool, bool>[] comparisonsBoolean =
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
    public enum ComparisonsArithmetic
    {
        Equal,
        NotEqual,
        RoughlyEqual,
        GreaterThan,
        GreaterOrEqualThan,
        SmallerThan,
        SmallerOrEqualThan
    }

    public enum ComparisonsBoolean
    {
        IsTrue,
        Equal,
        NotEqual
    }
}