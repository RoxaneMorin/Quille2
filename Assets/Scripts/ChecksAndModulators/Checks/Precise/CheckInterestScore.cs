using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Check_Interest_", menuName = "Checks&dModulators/Check/Interest", order = 10)]
    public class CheckInterestScore : CheckArithmeticSO
    {
        // ScriptableObject template for the instantiation of Interest arithmetic checks.
        // This type of check runs an arithmetic comparison on the fetched value, and returns true or false depending on the result.
        // The specific Interest value to fetch and check is set in instance assets.


        // VARIABLES/PARAM
        [SerializeField]
        private Quille.InterestSO relevantInterest;
        public Quille.InterestSO RelevantInterest { get { return relevantInterest; } set { relevantInterest = value; } }


        // METHODS
        // Fetch the value of the revelant personality axe.
        protected override void FetchParam(System.Object sourceObj)
        {
            param = Fetchers.FetchInterestScore(sourceObj, relevantInterest);
        }

        // Check defined in parent class.


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Interest : {0}", relevantInterest.ItemName);
        }
    }
}
