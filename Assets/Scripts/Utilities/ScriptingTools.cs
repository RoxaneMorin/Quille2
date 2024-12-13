using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AYellowpaper.SerializedCollections;
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


    // STRING
    public static string StripComplexChars(this String theString)
    {
        return Regex.Replace(theString, "[^\\w\\._]", "");
    }


    // TUPLES
    // (INT, INT)
    public static (int, int) Add(this (int, int) tupleA, (int, int) tupleB)
    {
        return (tupleA.Item1 + tupleB.Item1, tupleA.Item2 + tupleB.Item2);
    }


    // DICTIONARIES & SERIALIZED DICTIONARIES
    public static SerializedDictionary<TKey, TValue> ToSerializedDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sourceEnumerableKeyValuePair)
    {
        if (sourceEnumerableKeyValuePair == null)
        {
            throw new ArgumentNullException(nameof(sourceEnumerableKeyValuePair));
        }

        var serializedDictionary = new SerializedDictionary<TKey, TValue>();
        foreach (var keyValuePair in sourceEnumerableKeyValuePair)
        {
            serializedDictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }
        return serializedDictionary;
    }

    public static SerializedDictionary<TKey, TValue> ToSerializedDictionary<TKey, TValue>(this IEnumerable<TValue> sourceEnumerableValue, Func<TValue, TKey> keySelector)
    {
        if (sourceEnumerableValue == null)
        {
            throw new ArgumentNullException(nameof(sourceEnumerableValue));
        }
        if (keySelector == null)
        {
            throw new ArgumentNullException(nameof(keySelector));
        }

        var serializedDictionary = new SerializedDictionary<TKey, TValue>();
        foreach (var value in sourceEnumerableValue)
        {
            serializedDictionary.Add(keySelector(value), value);
        }
        return serializedDictionary;
    }

    public static SerializedDictionary<TKey, TElement> ToSerializedDictionary<TKey, TValue, TElement>(this IEnumerable<TValue> sourceEnumerableValue, Func<TValue, TKey> keySelector, Func<TValue, TElement> elementSelector)
    {
        if (sourceEnumerableValue == null)
        {
            throw new ArgumentNullException(nameof(sourceEnumerableValue));
        }
        if (keySelector == null)
        {
            throw new ArgumentNullException(nameof(keySelector));
        }
        if (elementSelector == null)
        {
            throw new ArgumentNullException(nameof(elementSelector));
        }

        var serializedDictionary = new SerializedDictionary<TKey, TElement>();
        foreach (var item in sourceEnumerableValue)
        {
            serializedDictionary.Add(keySelector(item), elementSelector(item));
        }
        return serializedDictionary;
    }


    // OTHER LINQ STUFF
    public static void ForEach<T>(this IEnumerable<T> sourceEnumerable, Action<T> action)
    {
        if (sourceEnumerable == null)
        {
            throw new ArgumentNullException(nameof(sourceEnumerable));
        }
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        foreach (T item in sourceEnumerable)
        {
            action(item);
        }
    }
}


public static class RandomExtended
{
    // Random cannot receive extention methods, cue this helper class.


    // Unity style wrapper for an existing System.Random operation.
    public static int RangeInt(int minInclusive, int maxInclusive)
    {
        var random = new System.Random();
        return random.Next(minInclusive, maxInclusive + 1);
    }

    // New random number in range, not X;
    public static int RangeInt(int minInclusive, int maxInclusive, int avoid)
    {
        var random = new System.Random();

        int randomNumber = avoid;
        while (randomNumber == avoid)
        {
            randomNumber = random.Next(minInclusive, maxInclusive + 1);
        }
        return randomNumber;
    }

    // Similar to Random.range, but the values produced are stepped.
    public static float RangeStepped(float minInclusive, float maxInclusive, float step)
    {
        int stepsInRange = Mathf.FloorToInt((maxInclusive - minInclusive) / step) + 1;
        int randomStep = UnityEngine.Random.Range(0, stepsInRange);
        return minInclusive + (randomStep * step);
    }
    public static int RangeIntStepped(int minInclusive, int maxInclusive, int step)
    {
        int stepsInRange = Mathf.FloorToInt((maxInclusive - minInclusive) / step) + 1;
        int randomStep = RangeInt(0, stepsInRange);
        return minInclusive + (randomStep * step);
    }


    // Get X non-repeating Ints in this range. MaxValue is exclusive.
    public static List<int> NonRepeatingIntegersInRange(int minInclusive, int maxExclusive, int targetCount)
    {
        var random = new System.Random();

        HashSet<int> theInts = new HashSet<int>();
        while (theInts.Count < targetCount)
        {
            theInts.Add(random.Next(minInclusive, maxExclusive));
        }

        return theInts.ToList();
    }


    // Return either true or false.
    public static bool CoinFlip()
    {
        return UnityEngine.Random.value > 0.5f;
    }

    // Either of the two given values.
    public static float CoinFlipBetween(float valueA, float valueB)
    {
        return (UnityEngine.Random.value > 0.5f ? valueA : valueB);
    }


    // Generate a random alphanumeric string of length X;
    public static string RandomAlphanumericString(int targetLength)
    {
        var random = new System.Random();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        string randomString = "";
        for (int i = 0; i < targetLength; i++)
        {
            randomString += chars[random.Next(chars.Length)];
        }
        return randomString;    
    }
}
