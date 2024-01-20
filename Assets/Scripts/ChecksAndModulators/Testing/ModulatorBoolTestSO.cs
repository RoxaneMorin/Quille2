using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "ModulatorBoolTest", menuName = "Checks&dModulators/Modulators/BoolTest", order = 0)]
    public class ModulatorBoolTestSO : ModulatorAlterFloatFromBoolSO
    {
        public bool theParam;

        protected override void FetchParam(UnityEngine.Object sourceObj)
        {
            param = theParam;
        }
    }
}

