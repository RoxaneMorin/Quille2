using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public class Check_DriveScore : Check_Arithmetic
    {
        // The specific type of check used for Drive scores.


        // VARIABLES/PARAM
        [SerializeField] [Tooltip("The target Drive to consult.")] public Quille.DriveSO target;
        public Quille.DriveSO RelevantDrive { get { return target; } set { target = value; } }



        // METHODS
        protected override float? FetchParam(System.Object sourceObj)
        {
            return Fetchers.FetchDriveScore(sourceObj, target);
        }


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Is Drive : {0} {1} {2}?", target ? target.ItemName : "[source value]", Symbols.comparisonSymbolsArithmetic[(int)opIdx], compareTo);
        }
    }
}

