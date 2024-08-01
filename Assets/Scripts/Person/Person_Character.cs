using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Newtonsoft.Json;

using System.IO;

namespace Quille
{
    [System.Serializable]
    public class Person_Character
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



        // PROPERTIES
        // Personality axe scores.
        public float GetAxeScore(PersonalityAxeSO targetPersonalityAxe)
        {
            if (myPersonalityAxes.ContainsKey(targetPersonalityAxe))
            {
                return myPersonalityAxes[targetPersonalityAxe];
            }
            else
            { 
                return 0;
            }
        }
        public void SetAxeScore(PersonalityAxeSO targetPersonalityAxe, float value)
        {
            if (value < -Constants.PERSONALITY_HALF_SPAN)
                myPersonalityAxes[targetPersonalityAxe] = -Constants.PERSONALITY_HALF_SPAN;
            else if (value > Constants.PERSONALITY_HALF_SPAN)
                myPersonalityAxes[targetPersonalityAxe] = Constants.PERSONALITY_HALF_SPAN;
            else
                myPersonalityAxes[targetPersonalityAxe] = value;
        }

        // Personality trait scores.
        public float GetTraitScore(PersonalityTraitSO targetPersonalityTrait)
        {
            if (myPersonalityTraits.ContainsKey(targetPersonalityTrait))
            {
                return myPersonalityTraits[targetPersonalityTrait];
            }
            else
            {
                return 0;
            }
        }
        public void SetTraitScore(PersonalityTraitSO targetPersonalityTrait, float value)
        {
            if (value < Constants.PERSONALITY_HALF_SPAN/2)
            {
                if (value <= 0)
                {
                    Debug.Log(string.Format("Scores for personality traits should not be null or negative. {0} will be rounded up to half intensity.", targetPersonalityTrait.name));
                    // TODO: simply ignore the value instead?
                }
                // TODO: display the warning message here?
                myPersonalityTraits[targetPersonalityTrait] = Constants.PERSONALITY_HALF_SPAN / 2;
            }
            else
            {
                myPersonalityTraits[targetPersonalityTrait] = Constants.PERSONALITY_HALF_SPAN;
            }
        }

        // Interest scores.
        public float GetInterestScore(InterestSO targetInterestSO)
        {
            if (myInterests.ContainsKey(targetInterestSO))
            {
                return myInterests[targetInterestSO];
            }
            else
            {
                return 0; 
                // TODO: how to differentiate from unknown and known-but-neutral interests?
            }
        }
        public void SetInterestScore(InterestSO targetInterestSO, float value)
        {
            if (value < -Constants.INTEREST_HALF_SPAN)
                myInterests[targetInterestSO] = -Constants.PERSONALITY_HALF_SPAN;
            else if (value > Constants.INTEREST_HALF_SPAN)
                myInterests[targetInterestSO] = Constants.PERSONALITY_HALF_SPAN;
            else
                myInterests[targetInterestSO] = value;
        }



        // CONSTRUCTORS
        public Person_Character()
        {
            myPersonalityAxes = new SerializedDictionary<PersonalityAxeSO, float>();
            myPersonalityTraits = new SerializedDictionary<PersonalityTraitSO, float>();
            myInterests = new SerializedDictionary<InterestSO, float>();
        }




        // METHODS
    }
}
