using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class floatPair
{
    // C# object grouping two float values as would a tupple, likely for ease of serialization.


    // VARIABLES
    public float Item1;
    public float Item2;

    // CONSTRUCTOR
    public floatPair(float Item1, float Item2)
    {
        this.Item1 = Item1;
        this.Item2 = Item2;
    
    }

    // EXTENSION METHODS
    public static implicit operator floatPair((float, float) tuple) => new floatPair(tuple.Item1, tuple.Item2);
    //public static explicit operator floatPair((float, float) tuple) => new floatPair(tuple.Item1, tuple.Item2);
}
