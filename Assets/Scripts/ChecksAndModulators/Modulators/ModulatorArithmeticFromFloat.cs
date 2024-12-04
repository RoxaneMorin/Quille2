using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public class ModulatorArithmeticFromFloat
    {
        // Wrapper / instantiable class for use by other scripts and assets at runtime, differentiated by its associated instance of ModulatorArithmeticFromFloatSO.
        // This type of modulator runs an arithmetic operation on the target value using the fetched value and given parameters, and returns the result.
        // The specific value to fetch and modulate is handled by the ModulatorArithmeticFromFloatSO object.


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


        // OVERRIDES
        public override string ToString()
        {
            // Handle special cases as needed.
            if (mainOpIdx == 0)
            {
                return "Result = Targer Value";
            }
            else if (modOpIdx == 0)
            {
                return string.Format("Result = Target Value {0} Fetched Value", ChecksAndMods.Symbols.operationSymbolsArithmetic[(int)mainOpIdx], ChecksAndMods.Symbols.operationSymbolsArithmetic[(int)modOpIdx]);
            }
            else
            {
                return string.Format("Result = Target Value {0} ({1} {2} {3})",
                        ChecksAndMods.Symbols.operationSymbolsArithmetic[(int)mainOpIdx],
                        modulator.ToString(),
                        ChecksAndMods.Symbols.operationSymbolsArithmetic[(int)modOpIdx],
                        modifier);
            }
        }
    }
}

