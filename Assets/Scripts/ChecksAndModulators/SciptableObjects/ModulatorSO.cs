using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public abstract class ModulatorSO : ScriptableObject
    {
        // VARIABLES/PARAMS 
        // -> Consulted parameter (SO?)
        // -> Dictionary/Array of functions?

        // EXPECTED INPUTS
        // -> Consulted GO
        // -> Consulted parameter
        // -> Target value/variable
        // -> Additional modifier value/variable
        // -> Operator(s)


        // METHODS
        // -> Fetch the consulted parameter.
        // -> Modulate!

        public abstract float Modulate(UnityEngine.Object sourceObj, float target, float modifier, int mainOpIdx, int modOpIdx);
    }
}