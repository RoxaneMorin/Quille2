using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "Check_Drive_", menuName = "Checks&dModulators/Check/Drive", order = 15)]
    public class CheckDriveScore : CheckArithmeticSO
    {
        // ScriptableObject template for the instantiation of Drive arithmetic checks.
        // This type of check runs an arithmetic comparison on the fetched value, and returns true or false depending on the result.
        // The specific Drive value to fetch and check is set in instance assets.


        // VARIABLES/PARAM
        [SerializeField]
        private Quille.DriveSO relevantDrive;
        public Quille.DriveSO RelevantDrive { get { return relevantDrive; } set { relevantDrive = value; } }


        // METHODS
        // Fetch the value of the revelant drive.
        protected override void FetchParam(System.Object sourceObj)
        {
            param = Fetchers.FetchDriveScore(sourceObj, relevantDrive);
        }

        // Check defined in parent class.
    }
}
