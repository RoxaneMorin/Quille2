using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    // Wrapper / instantiable class for use with modulator SOs.
    [System.Serializable]
    public class ModulatorAlterFloatFromFloat
    {
        // VARIABLES/PARAM
        // sourceObj, target given by the handler.
        [SerializeField]
        ModulatorAlterFloatFromFloatSO modulator;
        
        [SerializeField]
        public OperationsArithmethic mainOpIdx;
        [SerializeField]
        public OperationsArithmethic modOpIdx;

        [SerializeField]
        public float modifier;


        // METHODS
        // Execute.
        public void Execute()
        {
            //Modulate(UnityEngine.Object sourceObj, float target, float modifier, int mainOpIdx, int modOpIdx)
        }
    }
}

