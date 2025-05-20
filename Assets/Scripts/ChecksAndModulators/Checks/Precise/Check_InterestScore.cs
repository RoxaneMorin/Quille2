using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public class Check_InterestScore : Check_Arithmetic
    {
        // The specific type of check used for Interest scores.


        // VARIABLES/PARAM
        [SerializeField][Tooltip("The target Interest to consult.")] public Quille.InterestSO target;
        public Quille.InterestSO RelevantInterest { get { return target; } set { target = value; } }



        // METHODS
        protected override float? FetchParam(System.Object sourceObj)
        {
            return Fetchers.FetchInterestScore(sourceObj, target);
        }


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Is Interest : {0} {1} {2}?", target ? target.ItemName : "[source value]", Symbols.comparisonSymbolsArithmetic[(int)opIdx], compareTo);
        }
    }
}