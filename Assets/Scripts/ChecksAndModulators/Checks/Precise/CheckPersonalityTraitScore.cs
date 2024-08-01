using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Check_PersonalityTrait_", menuName = "Checks&dModulators/Check/Personality Trait", order = 10)]
    public class CheckPersonalityTraitScore : CheckArithmeticSO
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

        // Check defined in parent class.
    }
}
