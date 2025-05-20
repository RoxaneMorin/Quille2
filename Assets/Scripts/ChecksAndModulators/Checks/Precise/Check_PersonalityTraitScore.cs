using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [System.Serializable]
    public class Check_PersonalityTraitScore : Check_Arithmetic
    {
        // The specific type of check used for PersonalityTrait scores.


        // VARIABLES/PARAM
        [SerializeField][Tooltip("The target PersonalityTrait to consult.")] public Quille.PersonalityTraitSO target;
        public Quille.PersonalityTraitSO RelevantPersonalityTrait { get { return target; } set { target = value; } }



        // METHODS
        protected override float? FetchParam(System.Object sourceObj)
        {
            return Fetchers.FetchPersonalityTraitScore(sourceObj, target);
        }


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Is PersonalityTrait : {0} {1} {2}?", target ? target.ItemName : "[source value]", Symbols.comparisonSymbolsArithmetic[(int)opIdx], compareTo);
        }
    }
}
