using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    // Various extension methods.


    // ARRAY
    public static void InvertedSort(this Array array)
    {
        Array.Sort(array);
        Array.Reverse(array);
    }

    public static float GetHighestValue(this float[] floatArray)
    {
        var highestVal = floatArray[0];

        for (int i = 0; i < floatArray.Length; i++)
        {
            if (floatArray[i] > highestVal)
                highestVal = floatArray[i];
        }

        return highestVal;
    }


    // TUPLES
    // (INT, INT)
    public static (int, int) Add(this (int, int) tupleA, (int, int) tupleB)
    {
        return (tupleA.Item1 + tupleB.Item1, tupleA.Item2 + tupleB.Item2);
    }
}
