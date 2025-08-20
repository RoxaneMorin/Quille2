using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public class Modulator_InterestScore : Modulator_FromFloat
    {
        // The specific type of modulator used for Interest scores.


        // VARIABLES/PARAM
        [SerializeField][Tooltip("The target Interest to consult.")] public Quille.InterestSO target;
        public Quille.InterestSO RelevantInterest { get { return target; } set { target = value; } }



        // METHODS
        protected override string GetTargetName()
        {
            return target ? target.ItemName : "[source value]";
        }

        protected override float? FetchParam(System.Object sourceObj)
        {
            return Fetchers.FetchInterestScore(sourceObj, target);
        }
    }
}
