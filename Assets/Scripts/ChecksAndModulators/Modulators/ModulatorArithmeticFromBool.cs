using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    // Wrapper / instantiable class for use with modulator SOs.
    [System.Serializable]
    public class ModulatorArithmeticFromBool
    {
        // Wrapper / instantiable class for use by other scripts and assets at runtime, differentiated by its associated instance of ModulatorArithmeticFromBoolSO.
        // This type of modulator runs a boolean comparison on the fetched value. ("expectedParam" is only used in the Equal/Not Equal comparisons.)
        // If true, it runs an arithmetic operation on the target value using the given parameters, and returns the result. Else, the target is returned unaltered.
        // The specific value to fetch and modulate is handled by the ModulatorArithmeticFromBoolSO object.


        // VARIABLES/PARAM
        // sourceObj, target given by the handler.
        [SerializeField]
        ModulatorArithmeticFromBoolSO modulator;

        [SerializeField]
        public ChecksBoolean checkOpIdx;
        [SerializeField]
        public OperationsArithmetic modOpIdx;

        //[SerializeField]
        [Tooltip("Comparative value used in the Equal/Not Equal operations.")] public bool expectedParam;
        [SerializeField]
        public float modifier;

        // METHODS
        // Execute.
        public float Execute(System.Object sourceObj, float target)
        {
            return modulator.Modulate(sourceObj, target, expectedParam, modifier, ((int)checkOpIdx), (int)modOpIdx);
        }
    }
}

