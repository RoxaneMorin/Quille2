using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public abstract class Modulator_FromFloat : Modulator
    {
        // Parent class for specific modulators to inherit from.
        // This type of modulator runs an arithmetic operation on the target value using the fetched value and given parameters, and returns the result.
        // The specific value to fetch and modulate is handled by child classes and their instances.


        // VARIABLES/PARAM
        [SerializeField] protected OperationsArithmetic mainOpIdx;
        // Target item defined in child classes.


        // METHODS
        protected abstract string GetTargetName();
        protected abstract float? FetchParam(System.Object sourceObj);

        public override float Execute(System.Object sourceObj, float target)
        {
            // Will need to be cleaned up/refined to ensure safety and efficiency.
            try
            {
                float? param = FetchParam(sourceObj);
                if (param == null)
                {
                    // Do not modulate.
                    return target;
                }

                float moddedParam = Operators.operationsArithmetic[((int)modOpIdx)]((float)param, modifier);
                float result = Operators.operationsArithmetic[(int)mainOpIdx](target, moddedParam);
                return result;
            }
            catch
            {
                Debug.LogError("Modulate operation failed. The original target value will be returned.");
                return target;
            }
        }


        // OVERRIDES
        public override string ToString()
        {
            // Handle special cases as needed.
            if (mainOpIdx == 0)
            {
                return "Result = Target Value";
            }
            else if (modOpIdx == 0)
            {
                return string.Format("Result = Target Value {0} Fetched Value", ChecksAndMods.Symbols.operationSymbolsArithmetic[(int)mainOpIdx], ChecksAndMods.Symbols.operationSymbolsArithmetic[(int)modOpIdx]);
            }
            else
            {
                return string.Format("Result = Target Value {0} ({1} {2} {3})",
                        ChecksAndMods.Symbols.operationSymbolsArithmetic[(int)mainOpIdx],
                        GetTargetName(),
                        ChecksAndMods.Symbols.operationSymbolsArithmetic[(int)modOpIdx],
                        modifier);
            }
        }
    }
}

