using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    // Wrapper / instantiable class for use with modulator SOs.
    [System.Serializable]
    public class ModulatorArithmeticFromFloat
    {
        // VARIABLES/PARAM
        // sourceObj, target given by the handler.
        [SerializeField]
        ModulatorArithmeticFromFloatSO modulator;
        
        [SerializeField]
        public OperationsArithmetic mainOpIdx;
        [SerializeField]
        public OperationsArithmetic modOpIdx;

        [SerializeField]
        public float modifier;


        // METHODS
        // Execute.
        public float Execute(System.Object sourceObj, float target)
        {
            return modulator.Modulate(sourceObj, target, modifier, ((int)mainOpIdx), (int)modOpIdx);
        }
    }
}

