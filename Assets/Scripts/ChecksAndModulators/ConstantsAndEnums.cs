using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    // Constants, enums and delegates used by the system of checks and modulators.
    // Order is important! Enums and Funcs reflect each others' indices.


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
            ((a, b) => Mathf.Approximately(a, b)),
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

    public static class Symbols
    {
        public static readonly string[] operationSymbolsArithmetic = { "", "+", "-", "*", "/", "%", "^" };

        public static string[] comparisonSymbolsArithmetic = { "==", "!=", "~=", ">", ">=", "<", "<=" };

        public static readonly string[] comparisonSymbolsBoolean = { "", "==", "!=" };
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



    // TODO: simplify comparison booleans to just IsTrue/IsFalse?




    public enum SubtypeNames
    {
        DriveScore,

    }

    public static class Subtypes
    {
        public static readonly Type[] subtypesCheck = { typeof(CheckArithmeticDriveScore) };
    }

    // DOTO: probably need both enum & list of types for it.
}