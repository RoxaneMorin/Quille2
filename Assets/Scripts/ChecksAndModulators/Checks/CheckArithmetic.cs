using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public class CheckArithmetic
    {
        // Wrapper / instantiable class for use by other scripts and assets at runtime, differentiated by its associated instance of CheckArithmeticSO.
        // This type of check runs an arithmetic comparison on the fetched value, and returns true or false depending on the result.
        // The specific value to fetch and check is handled by the CheckArithmeticSO object.


        // VARIABLES/PARAM
        // sourceObj, target given by the handler.
        [SerializeField]
        CheckArithmeticSO check;

        [SerializeField]
        public ComparisonsArithmetic opIdx;
        [SerializeField]
        public float compareTo;


        // METHODS
        // Execute.
        public bool Execute(System.Object sourceObj)
        {
            return check.Check(sourceObj, compareTo, (int)opIdx);
        }


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Is {0} {1} {2}?", check.ToString(), Symbols.comparisonSymbolsArithmetic[(int)opIdx], compareTo);
        }
    }
}
