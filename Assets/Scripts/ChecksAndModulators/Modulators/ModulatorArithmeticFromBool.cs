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
        public ComparisonsBoolean checkOpIdx;
        [SerializeField]
        public OperationsArithmetic modOpIdx;

        [SerializeField]
        [Tooltip("Comparative value used in the Equal/Not Equal operations.")] public bool compareTo;
        [SerializeField]
        public float modifier;

        // METHODS
        // Execute.
        public float Execute(System.Object sourceObj, float target)
        {
            return modulator.Modulate(sourceObj, target, compareTo, modifier, ((int)checkOpIdx), (int)modOpIdx);
        }


        // OVERRIDES
        public override string ToString()
        {
            // Handle special cases as needed.
            if (modOpIdx == 0)
            {
                return "Result = Targer Value";
            }
            else if (checkOpIdx == 0)
            {
                return string.Format("Result = {0} ? (Target Value {1} {2}) : Target Value", modulator.ToString(), ChecksAndMods.Symbols.operationSymbolsArithmetic[(int)modOpIdx], modifier);
            }
            else
            {
                return string.Format("Result = (Fetched Value {0} {1}) ? (Target Value {2} {3}) : Target Value",
                        ChecksAndMods.Symbols.comparisonSymbolsBoolean[(int)checkOpIdx],
                        (compareTo ? "true" : "false"),
                        ChecksAndMods.Symbols.operationSymbolsArithmetic[(int)modOpIdx],
                        modifier);
            }
        }
    }
}

