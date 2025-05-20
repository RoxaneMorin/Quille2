using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public class Modulator_PersonalityTraitScore : Modulator_FromFloat
    {
        // The specific type of modulator used for PersonalityTrait scores.


        // VARIABLES/PARAM
        [SerializeField][Tooltip("The target PersonalityTrait to consult.")] public Quille.PersonalityTraitSO target;
        public Quille.PersonalityTraitSO RelevantPersonalityTrait { get { return target; } set { target = value; } }



        // METHODS
        protected override string GetTargetName()
        {
            return target ? target.ItemName : "[source value]";
        }

        protected override float? FetchParam(System.Object sourceObj)
        {
            return Fetchers.FetchPersonalityTraitScore(sourceObj, target);
        }
    }
}
