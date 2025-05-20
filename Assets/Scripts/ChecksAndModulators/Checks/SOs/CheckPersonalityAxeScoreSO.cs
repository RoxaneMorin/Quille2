using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Check_PersonalityAxe_", menuName = "Checks&dModulators/Check/Personality Axe", order = 0)]
    public class CheckPersonalityAxeScoreSO : CheckArithmeticSO
    {
        // ScriptableObject template for the instantiation of PersonalityAxe arithmetic checks.
        // This type of check runs an arithmetic comparison on the fetched value, and returns true or false depending on the result.
        // The specific PersonalityAxe value to fetch and check is set in instance assets.


        // VARIABLES/PARAM
        [SerializeField]
        private Quille.PersonalityAxeSO relevantPersonalityAxe;
        public Quille.PersonalityAxeSO RelevantPersonalityAxe { get { return relevantPersonalityAxe; } set { relevantPersonalityAxe = value; } }


        // METHODS
        // Fetch the value of the revelant personality axe.
        protected override void FetchParam(System.Object sourceObj)
        {
            param = Fetchers.FetchPersonalityAxeScore(sourceObj, relevantPersonalityAxe);
        }

        // Check defined in parent class.


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Personality Axe : {0} vs {1}", relevantPersonalityAxe.AxeNameLeft, relevantPersonalityAxe.AxeNameRight);
        }
    }
}
