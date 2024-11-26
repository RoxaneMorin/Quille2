using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Modulator_Drive_", menuName = "Checks&dModulators/Modulators/Drive", order = 15)]
    public class ModulatorDriveScore : ModulatorArithmeticFromFloatSO
    {
        // ScriptableObject template for the instantiation of Drive arithmetic modulators.
        // This type of modulator runs an arithmetic operation on the target value using the fetched value and given parameters, and returns the result.
        // The specific Drive value to fetch and check is set in instance assets.


        // VARIABLES/PARAM
        [SerializeField]
        private Quille.DriveSO relevantDrive;
        public Quille.DriveSO RelevantDrive { get { return relevantDrive; } set { relevantDrive = value; } }


        // METHODS
        // Fetch the value of the revelant personality trait.
        protected override void FetchParam(System.Object sourceObj)
        {
            param = Fetchers.FetchDriveScore(sourceObj, relevantDrive);
        }

        // Modulate defined in parent class.
    }
}