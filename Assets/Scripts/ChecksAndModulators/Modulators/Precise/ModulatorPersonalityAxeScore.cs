using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Modulator_PersonalityAxe_", menuName = "Checks&dModulators/Modulators/Personality Axe", order = 0)]
    public class ModulatorPersonalityAxeScore : ModulatorArithmeticFromFloatSO
    {
        // ScriptableObject template for the instantiation of PersonalityAxe arithmetic modulators.
        // This type of modulator runs an arithmetic operation on the target value using the fetched value and given parameters, and returns the result.
        // The specific PersonalityAxe value to fetch and check is set in instance assets.


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
