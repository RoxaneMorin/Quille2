using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Modulator_PersonalityAxe_", menuName = "Checks&dModulators/Modulators/Personality Axe", order = 0)]
    public class ModulatorPersonalityAxeScore : ModulatorArithmeticFromFloatSO
    {
        // VARIABLES/PARAM
        [SerializeField]
        private Quille.PersonalityAxeSO relevantPersonalityAxe;


        // METHODS
        // Fetch the value of the revelant personality axe.
        protected override void FetchParam(System.Object sourceObj)
        {
            param = Fetchers.FetchPersonalityAxeScore(sourceObj, relevantPersonalityAxe);
        }

        // Modulate defined in parent class.
    }
}
