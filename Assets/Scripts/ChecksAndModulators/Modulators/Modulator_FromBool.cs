using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    // Wrapper / instantiable class for use with modulator SOs.
    [System.Serializable]
    public abstract class Modulator_FromBool : Modulator
    {
        // Parent class for specific modulators to inherit from.
        // This type of modulator runs a boolean comparison on the fetched value. ("expectedParam" is only used in the Equal/Not Equal comparisons.)
        // If true, it runs an arithmetic operation on the target value using the given parameters, and returns the result. Else, the target is returned unaltered.
        // The specific value to fetch and modulate is handled by child classes and their instances.


        // VARIABLES/PARAM
        [SerializeField] public ComparisonsBoolean checkOpIdx;
        [SerializeField] [Tooltip("Comparative value used in the Equal/Not Equal operations.")] public bool compareTo;
        // Target item defined in child classes.


        // METHODS
        protected abstract string GetTargetName();
        protected abstract bool? FetchParam(System.Object sourceObj);

        public override float Execute(System.Object sourceObj, float target)
        {
            // Will need to be cleaned up/refined to ensure safety and efficiency.
            try
            {
                bool? param = FetchParam(sourceObj);

                // Make sure that the param is not null.
                // Compare the fetched param to the expectedParam. Execute the following operation if this one returns true.
                if (param != null && Operators.comparisonsBoolean[((int)checkOpIdx)]((bool)param, compareTo))
                {
                    float result = Operators.operationsArithmetic[((int)modOpIdx)](target, modifier);
                    return result;
                }

                // Do not modulate if the param is null, or the check returns false.
                return target;
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
            if (modOpIdx == 0)
            {
                return "Result = Target Value";
            }
            else if (checkOpIdx == 0)
            {
                return string.Format("Result = {0} ? (Target Value {1} {2}) : Target Value", GetTargetName(), ChecksAndMods.Symbols.operationSymbolsArithmetic[(int)modOpIdx], modifier);
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

