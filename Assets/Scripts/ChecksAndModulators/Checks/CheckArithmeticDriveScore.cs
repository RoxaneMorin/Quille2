using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public class CheckArithmeticDriveScore : CheckArithmetic
    {
        // VARIABLES/PARAM
        [SerializeField] [Tooltip("The target Drive to consult.")] public Quille.DriveSO targetElement;



        // METHODS
        // Fetch the value of the revelant drive.
        protected override float? FetchParam(System.Object sourceObj)
        {
            return Fetchers.FetchDriveScore(sourceObj, targetElement);
        }

        // Check defined in parent class.


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Is Drive : {0} {1} {2}?", targetElement ? targetElement.ItemName : "[source value]", Symbols.comparisonSymbolsArithmetic[(int)opIdx], compareTo);
        }
    }
}

