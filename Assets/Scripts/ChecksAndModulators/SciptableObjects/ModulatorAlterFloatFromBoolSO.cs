using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public abstract class ModulatorAlterFloatFromBoolSO : ModulatorSO
    {
        // VARIABLES/PARAM
        protected bool param;
        public bool expectedParam; // Have in the wrapper instead?

        // METHODS
        protected abstract void FetchParam(UnityEngine.Object sourceObj);

        // returns (param ?2 mod) if target ?1.
        public override float Modulate(UnityEngine.Object sourceObj, float target, float modifier, int mainOpIdx, int modOpIdx)
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

            // Compare the fetched param to the expectedParam. Execute the operation if they comform.
            if (Operators.checksBoolean[mainOpIdx](param, expectedParam))
            {
                float result = Operators.operationsArithmethic[modOpIdx](target, modifier);
                return result;
            }
            else // Else, return the original target value.
                return target;
        }
    }
}
