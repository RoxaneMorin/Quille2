using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    // Wrapper / instantiable class for use with modulator SOs.
    [System.Serializable]
    public class ModulatorArithmeticFromBool
    {
        // VARIABLES/PARAM
        // sourceObj, target given by the handler.
        [SerializeField]
        ModulatorArithmeticFromBoolSO modulator;

        [SerializeField]
        public ChecksBoolean checkOpIdx;
        [SerializeField]
        public OperationsArithmetic modOpIdx;

        [SerializeField]
        public bool expectedParam;
        [SerializeField]
        public float modifier;

        // METHODS
        // Execute.
        public float Execute(UnityEngine.Object sourceObj, float target)
        {
            return modulator.Modulate(sourceObj, target, expectedParam, modifier, ((int)checkOpIdx), (int)modOpIdx);
        }
    }
}

