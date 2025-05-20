using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public abstract class CheckArithmeticSO : ScriptableObject
    {
        // Parent class for specificed CheckArithmeticSOs to inherit from.
        // This type of check runs an arithmetic comparison on the fetched value, and returns true or false depending on the result.
        // The specific value to fetch and check is elaborated upon by child classes and their instances.


        // VARIABLES/PARAM
        protected float? param;


        // METHODS
        protected abstract void FetchParam(System.Object sourceObj);

        // returns param ? compareTo
        public bool Check(System.Object sourceObj, float compareTo, int opIdx)
        {
            // Will need to be cleaned up/refined to ensure safety and efficiency.
            try
            {
                FetchParam(sourceObj);
                if (param == null)
                {
                    // Return false. See if we should return null instead?
                    return false;
                }

                return Operators.comparisonsArithmetic[opIdx]((float)param, compareTo);
            }
            catch
            {
                Debug.LogError("Modulate operation failed. The check will return false.");
                return false;
            }
        }
    }
}
