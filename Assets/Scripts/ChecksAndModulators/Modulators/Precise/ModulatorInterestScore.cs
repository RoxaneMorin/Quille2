using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Modulator_Interest_", menuName = "Checks&dModulators/Modulators/Interest", order = 10)]
    public class ModulatorInterestScore : ModulatorArithmeticFromFloatSO
    {
        // ScriptableObject template for the instantiation of Interest arithmetic modulators.
        // This type of modulator runs an arithmetic operation on the target value using the fetched value and given parameters, and returns the result.
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

        // Modulate defined in parent class.


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Interest : {0}", relevantInterest.ItemName);
        }
    }
}
