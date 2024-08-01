using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    // Wrapper / instantiable class for use with modulator checks.
    [System.Serializable]
    public class CheckArithmetic
    {
        // VARIABLES/PARAM
        // sourceObj, target given by the handler.
        [SerializeField]
        CheckArithmeticSO check;

        [SerializeField]
        public ChecksArithmetic opIdx;
        [SerializeField]
        public float compareTo;


        // METHODS
        // Execute.
        public bool Execute(System.Object sourceObj)
        {
            return check.Check(sourceObj, compareTo, (int)opIdx);
        }
    }
}
