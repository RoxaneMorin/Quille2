using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public abstract class CheckBoolean : Check
    {
        // Parent class for specificed boolean checks to inherit from.
        // This type of check runs an boolean comparison on the fetched value, and returns true or false depending on the result.
        // The specific value to fetch and check is elaborated upon by child classes and their instances.


        // VARIABLES/PARAM
        [SerializeField] public ComparisonsBoolean opIdx;
        [SerializeField] [Tooltip("Comparative value used in the Equal/Not Equal operations.")] public bool compareTo;
        // Target item defined in child classes.



        // METHODS
        protected abstract bool? FetchParam(System.Object sourceObj);

        public override bool Execute(System.Object sourceObj)
        {
            // Will need to be cleaned up/refined to ensure safety and efficiency.
            try
            {
                bool? param = FetchParam(sourceObj);
                if (param == null)
                {
                    // Return false. See if we should return null instead?
                    return false;
                }

                return Operators.comparisonsBoolean[((int)opIdx)]((bool)param, compareTo);
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
        //    if (opIdx == ComparisonsBoolean.IsTrue)
        //    {
        //        return string.Format("Is {0} True?", check ? check.ToString() : "[source value]");
        //    }
        //    else
        //    {
        //        return string.Format("Is {0} {1} {2}?", check ? check.ToString() : "[source value]", Symbols.comparisonSymbolsBoolean[(int)opIdx], compareTo);
        //    }
        //}
    }
}