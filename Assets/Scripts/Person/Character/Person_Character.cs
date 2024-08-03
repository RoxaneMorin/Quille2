using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Newtonsoft.Json;

using System.IO;

namespace Quille
{
    [System.Serializable]
    public partial class Person_Character
    {
        // VARIABLES/PARAMS

        // Personality scores.
        [SerializeField, SerializedDictionary("Personality Axe", "Score")]
        private SerializedDictionary<PersonalityAxeSO, float> myPersonalityAxes;

        [SerializeField, SerializedDictionary("Personality Trait", "Score")]
        private SerializedDictionary<PersonalityTraitSO, float> myPersonalityTraits;
        // Scores should be limited to 0.5 or 1.
        // A trait with a score of zero should be pruned away.


        // Interests and preferences.
        [SerializeField, SerializedDictionary("Interest", "Score")]
        private SerializedDictionary<InterestSO, float> myInterests;
        //Scores range between 1 (loved) and -1 (hated).
        // A score of zero means the character know of this interest but is indifferent to it.
        // Interests that do not appear in the dictionary are unknown.

        // Misc favourites
        // Colours, styles, etc.


        // PROPERTIES IN PARTIAL CLASS


        // CONSTRUCTORS
        public Person_Character(SerializedDictionary<PersonalityAxeSO, float> myPersonalityAxes = null, SerializedDictionary<PersonalityTraitSO, float> myPersonalityTraits = null, SerializedDictionary<InterestSO, float> myInterests = null)
        {
            this.myPersonalityAxes = myPersonalityAxes != null ? myPersonalityAxes: new SerializedDictionary<PersonalityAxeSO, float>();
            this.myPersonalityTraits = myPersonalityTraits != null ? myPersonalityTraits : new SerializedDictionary<PersonalityTraitSO, float>();
            this.myInterests = myInterests != null ? myInterests : new SerializedDictionary<InterestSO, float>();
        }



        // METHODS

    }
}
