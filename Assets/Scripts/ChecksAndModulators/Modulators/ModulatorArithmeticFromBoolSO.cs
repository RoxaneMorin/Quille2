using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public abstract class ModulatorArithmeticFromBoolSO : ScriptableObject
    {
        // VARIABLES/PARAM
        protected bool param;


        // METHODS
        protected abstract void FetchParam(System.Object sourceObj);

        // returns (param ?2 mod) if target ?1.
        public float Modulate(System.Object sourceObj, float target, bool expectedParam, float modifier, int checkOpIdx, int modOpIdx)
        {
            // Will need to be cleaned up/refined to ensure safety and efficiency.
            try
            {
                FetchParam(sourceObj);
            }
            catch
            {
                Debug.LogError("Modulate operation failed. The original target value will be returned.");
                return target;
            }

            // Compare the fetched param to the expectedParam. Execute the following operation if this one returns true.
            if (Operators.checksBoolean[checkOpIdx](param, expectedParam))
            {
                float result = Operators.operationsArithmetic[modOpIdx](target, modifier);
                return result;
            }
            else // Else, return the original target value.
                return target;
        }
    }
}
