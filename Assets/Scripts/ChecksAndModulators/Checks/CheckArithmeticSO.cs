using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public abstract class CheckArithmeticSO : ScriptableObject
    {
        // VARIABLES/PARAM
        protected float param;


        // METHODS
        protected abstract void FetchParam(UnityEngine.Object sourceObj);

        // returns param ? compareTo
        public bool Check(UnityEngine.Object sourceObj, float compareTo, int opIdx)
        {
            // Will need to be cleaned up/refined to ensure safety and efficiency.
            try
            {
                FetchParam(sourceObj);
            }
            catch
            {
                Debug.LogError("Modulate operation failed. The check will return false.");
                return false;
            }

            return Operators.checksArithmetic[opIdx](param, compareTo);
        }
    }
}
