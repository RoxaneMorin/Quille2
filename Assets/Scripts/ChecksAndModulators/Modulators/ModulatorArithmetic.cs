using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public abstract class ModulatorArithmetic
    {
        // Abstract parent class to ModulatorArithmeticFromBool and ModulatorArithmeticFromFloat.


        // VARIABLES/PARAMS
        [SerializeField] public OperationsArithmetic modOpIdx;
        [SerializeField] public float modifier;



        // METHODS
        public abstract float Execute(System.Object sourceObj, float target);
    }
}
