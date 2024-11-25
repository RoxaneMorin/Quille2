using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public abstract class ModulatorArithmeticFromBoolSO : ScriptableObject
    {
        // Parent class for specificed ModulatorArithmeticFromBoolSO to inherit from.
        // This type of modulator runs a boolean comparison on the fetched value. ("expectedParam" is only used in the Equal/Not Equal comparisons.)
        // If true, it runs an arithmetic operation on the target value using the given parameters, and returns the result. Else, the target is returned unaltered.
        // The specific value to fetch and modulate is elaborated upon by child classes and their instances.


        // VARIABLES/PARAM
        protected bool? param;


        // METHODS
        protected abstract void FetchParam(System.Object sourceObj);

        // returns (param ?2 mod) if target ?1.
        public float Modulate(System.Object sourceObj, float target, bool expectedParam, float modifier, int checkOpIdx, int modOpIdx)
        {
            // Will need to be cleaned up/refined to ensure safety and efficiency.
            try
            {
                FetchParam(sourceObj);

                // Make sure that the param is not null.
                // Compare the fetched param to the expectedParam. Execute the following operation if this one returns true.
                if (param != null && Operators.checksBoolean[checkOpIdx]((bool)param, expectedParam))
                {
                    float result = Operators.operationsArithmetic[modOpIdx](target, modifier);
                    return result;
                }
                
                // Do not modulate if the param is null, or the check returns false.
                return target;
            }
            catch
            {
                Debug.LogError("Modulate operation failed. The original target value will be returned.");
                return target;
            }
        }
    }
}
