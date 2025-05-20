using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Check_PersonalityTrait_", menuName = "Checks&dModulators/Check/Personality Trait", order = 10)]
    public class CheckPersonalityTraitScoreSO : CheckArithmeticSO
    {
        // ScriptableObject template for the instantiation of PersonalityTrait arithmetic checks.
        // This type of check runs an arithmetic comparison on the fetched value, and returns true or false depending on the result.
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

        // Check defined in parent class.


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Personality Trait : {0}", relevantPersonalityTrait.ItemName);
        }
    }
}
