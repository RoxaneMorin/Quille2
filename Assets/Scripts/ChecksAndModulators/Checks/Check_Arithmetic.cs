using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public abstract class Check_Arithmetic : Check
    {
        // Parent class for specificed arithmetic checks to inherit from.
        // This type of check runs an arithmetic comparison on the fetched value, and returns true or false depending on the result.
        // The specific value to fetch and check is elaborated upon by child classes and their instances.


        // VARIABLES/PARAM
        [SerializeField] public ComparisonsArithmetic opIdx;
        [SerializeField] public float compareTo;
        // Target item defined in child classes.



        // METHODS
        protected abstract float? FetchParam(System.Object sourceObj);

        public override bool Execute(System.Object sourceObj)
        {
            // Will need to be cleaned up/refined to ensure safety and efficiency.
            try
            {
                float? param = FetchParam(sourceObj);
                if (param == null)
                {
                    // Return false. See if we should return null instead?
                    return false;
                }

                return Operators.comparisonsArithmetic[((int)opIdx)]((float)param, compareTo);
            }
            catch
            {
                Debug.LogError("Modulate operation failed. The check will return false.");
                return false;
            }
        }


        // OVERRIDES
        //public override string ToString()
        //{
        //    return string.Format("Is {0} {1} {2}?", check ? check.ToString() : "[source value]", Symbols.comparisonSymbolsArithmetic[(int)opIdx], compareTo);
        //}
    }
}
