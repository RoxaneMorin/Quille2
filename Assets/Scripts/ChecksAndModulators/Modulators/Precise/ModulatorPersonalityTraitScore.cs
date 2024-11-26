using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Modulator_PersonalityTrait_", menuName = "Checks&dModulators/Modulators/Personality Trait", order = 10)]
    public class ModulatorPersonalityTraitScore : ModulatorArithmeticFromFloatSO
    {
        // ScriptableObject template for the instantiation of PersonalityTrait arithmetic modulators.
        // This type of modulator runs an arithmetic operation on the target value using the fetched value and given parameters, and returns the result.
        // The specific PersonalityTrait value to fetch and check is set in instance assets.


        // VARIABLES/PARAM
        [SerializeField]
        private Quille.PersonalityTraitSO relevantPersonalityTrait;
        public Quille.PersonalityTraitSO RelevantPersonalityTrait { get { return relevantPersonalityTrait; } set { relevantPersonalityTrait = value; } }


        // METHODS
        // Fetch the value of the revelant personality trait.
        protected override void FetchParam(System.Object sourceObj)
        {
            param = Fetchers.FetchPersonalityTraitScore(sourceObj, relevantPersonalityTrait);
        }

        // Modulate defined in parent class.
    }
}