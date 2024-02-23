using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "ModModulatorPersonalityTraitScoreulatorPersonalityTrait", menuName = "Checks&dModulators/Modulators/Personality Trait", order = 10)]
    public class ModulatorPersonalityTraitScore : ModulatorArithmeticFromFloatSO
    {
        // VARIABLES/PARAM
        [SerializeField]
        private Quille.PersonalityTraitSO relevantPersonalityTrait;


        // METHODS
        // Fetch the value of the revelant personality axe.
        protected override void FetchParam(UnityEngine.Object sourceObj)
        {
            param = Fetchers.FetchPersonalityTraitScore(sourceObj, relevantPersonalityTrait);
        }

        // Modulate defined in parent class.
    }
}