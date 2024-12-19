using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public abstract class Check
    {
        // Abstract parent class to both boolean and arithmetic checks.

        // METHODS
        public abstract bool Execute(System.Object sourceObj);
    }
}

