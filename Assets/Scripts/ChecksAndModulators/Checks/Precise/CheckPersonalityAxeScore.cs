using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Check_PersonalityAxe_", menuName = "Checks&dModulators/Check/Personality Axe", order = 0)]
    public class CheckPersonalityAxeScore : CheckArithmeticSO
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

        // Check defined in parent class.
    }
}
