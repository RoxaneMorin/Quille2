using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public abstract class Modulator
    {
        // Abstract parent class to modulators from both boolean and arithmetic operations.


        // VARIABLES/PARAMS
        [SerializeField] protected OperationsArithmetic modOpIdx;
        [SerializeField] protected float modifier;



        // METHODS
        public abstract float Execute(System.Object sourceObj, float target);
    }
}
