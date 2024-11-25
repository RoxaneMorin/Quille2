using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public abstract class ModulatorArithmeticFromFloatSO : ScriptableObject
    {
        // Parent class for specificed ModulatorArithmeticFromFloatSO to inherit from.
        // This type of modulator runs an arithmetic operation on the target value using the fetched value and given parameters, and returns the result.
        // The specific value to fetch and modulate is elaborated upon by child classes and their instances.


        // VARIABLES/PARAM
        protected float? param;


        // METHODS
        protected abstract void FetchParam(System.Object sourceObj);

        // returns target ?1 (param ?2 mod)
        public float Modulate(System.Object sourceObj, float target, float modifier, int mainOpIdx, int modOpIdx)
        {
            // Will need to be cleaned up/refined to ensure safety and efficiency.
            try
            {
                FetchParam(sourceObj);
                if (param == null)
                {
                    // Do not modulate.
                    return target;
                }

                float moddedParam = Operators.operationsArithmetic[modOpIdx]((float)param, modifier);
                float result = Operators.operationsArithmetic[mainOpIdx](target, moddedParam);
                return result;
            }
            catch
            {
                Debug.LogError("Modulate operation failed. The original target value will be returned.");
                return target;
            }
        }
    }
}
