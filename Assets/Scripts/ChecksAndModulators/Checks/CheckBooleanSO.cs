using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public abstract class CheckBooleanSO : ScriptableObject
    {
        // Parent class for specificed CheckBooleanSOs to inherit from.
        // This type of check runs an boolean comparison on the fetched value, and returns true or false depending on the result.
        // The specific value to fetch and check is elaborated upon by child classes and their instances.


        // VARIABLES/PARAM
        protected bool? param;


        // METHODS
        protected abstract void FetchParam(System.Object sourceObj);

        // returns param ? compareTo
        public bool Check(System.Object sourceObj, bool compareTo, int opIdx)
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

                return Operators.comparisonsBoolean[opIdx]((bool)param, compareTo);
            }
            catch
            {
                Debug.LogError("Modulate operation failed. The check will return false.");
                return false;
            }
        }
    }
}