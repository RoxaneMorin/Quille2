using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public class Check_PersonalityAxeScore : Check_Arithmetic
    {
        // The specific type of check used for PersonalityAxe scores.


        // VARIABLES/PARAM
        [SerializeField] [Tooltip("The target PersonalityAxe to consult.")] public Quille.PersonalityAxeSO target;
        public Quille.PersonalityAxeSO RelevantPersonalityAxe { get { return target; } set { target = value; } }



        // METHODS
        protected override string GetTargetName()
        {
            return target ? target.ItemAndAxeNames : "[source value]";
        }

        protected override float? FetchParam(System.Object sourceObj)
        {
            return Fetchers.FetchPersonalityAxeScore(sourceObj, target);
        }
    }
}
