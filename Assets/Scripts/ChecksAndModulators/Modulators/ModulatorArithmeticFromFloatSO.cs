using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public abstract class ModulatorArithmeticFromFloatSO : ScriptableObject
    {
        // VARIABLES/PARAM
        protected float param;


        // METHODS
        protected abstract void FetchParam(System.Object sourceObj);

        // returns target ?1 (param ?2 mod)
        public float Modulate(System.Object sourceObj, float target, float modifier, int mainOpIdx, int modOpIdx)
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

            float moddedParam = Operators.operationsArithmetic[modOpIdx](param, modifier);
            float result = Operators.operationsArithmetic[mainOpIdx](target, moddedParam);
            return result;
        }
    }
}
