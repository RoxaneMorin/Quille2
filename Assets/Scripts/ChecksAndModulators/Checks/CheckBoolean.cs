using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public class CheckBoolean
    {
        // Wrapper / instantiable class for use by other scripts and assets at runtime, differentiated by its associated instance of CheckBooleanSO.
        // This type of check runs an boolean comparison on the fetched value, and returns true or false depending on the result.
        // The specific value to fetch and check is handled by the CheckBooleanSO object.


        // VARIABLES/PARAM
        // sourceObj, target given by the handler.
        [SerializeField]
        CheckBooleanSO check;

        [SerializeField]
        public ComparisonsBoolean opIdx;
        [SerializeField]
        [Tooltip("Comparative value used in the Equal/Not Equal operations.")]
        public bool compareTo;


        // METHODS
        // Execute.
        public bool Execute(System.Object sourceObj)
        {
            return check.Check(sourceObj, compareTo, (int)opIdx);
        }
    }
}