using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Modulator_PersonalityTrait_", menuName = "Checks&dModulators/Modulators/Personality Trait", order = 10)]
    public class ModulatorPersonalityTraitScore : ModulatorArithmeticFromFloatSO
    {
        // VARIABLES/PARAM
        [SerializeField]
        private Quille.PersonalityTraitSO relevantPersonalityTrait;


        // METHODS
        // Fetch the value of the revelant personality trait.
        protected override void FetchParam(System.Object sourceObj)
        {
            param = Fetchers.FetchPersonalityTraitScore(sourceObj, relevantPersonalityTrait);
        }

        // Modulate defined in parent class.
    }
}