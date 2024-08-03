using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Newtonsoft.Json;

using System.IO;

namespace Quille
{
    public partial class Person_Character
    {
        // PROPERTIES & THEIR HELPER METHODS


        // PERSONALITY AXES
        private float capPersonalityAxeScore(float value)
        {
            if (value < -Constants.PERSONALITY_HALF_SPAN)
                return -Constants.PERSONALITY_HALF_SPAN;
            else if (value > Constants.PERSONALITY_HALF_SPAN)
                return Constants.PERSONALITY_HALF_SPAN;
            
            return value;
        }
        // Single scores.
        public void SetAxeScore(PersonalityAxeSO targetPersonalityAxe, float value)
        {
            myPersonalityAxes[targetPersonalityAxe] = capPersonalityAxeScore(value);
        }

        public float? GetAxeScore(PersonalityAxeSO targetPersonalityAxe)
        {
            if (myPersonalityAxes.ContainsKey(targetPersonalityAxe))
            {
                return myPersonalityAxes[targetPersonalityAxe];
            }
            else
            {
                return null;
            }
        }
        // The full dict.
        internal void SetAxeScoreDict(SerializedDictionary<PersonalityAxeSO, float> personalityAxeDict, bool capScores = true)
        {
            if (capScores)
            {
                
                foreach (PersonalityAxeSO key in personalityAxeDict.Keys)
                {
                    personalityAxeDict[key] = capPersonalityAxeScore(personalityAxeDict[key]);
                }
            }

            this.myPersonalityAxes = personalityAxeDict;
        }
        internal SerializedDictionary<PersonalityAxeSO, float> GetAxeScoreDict()
        {
            return myPersonalityAxes;
        }


        // PERSONALITY TRAITS
        private float capPersonalityTraitScore(float value)
        {
            // Personality traits can only have a value of 0.5 or 1.
            if (value <= 0) // Discard zero and negative values;
            {
                return 0;
            }
            else if (value < Constants.PERSONALITY_HALF_SPAN / 2)
            {
                return Constants.PERSONALITY_HALF_SPAN / 2;
            }
            return Constants.PERSONALITY_HALF_SPAN;
        }
        // Single scores.
        public void SetTraitScore(PersonalityTraitSO targetPersonalityTrait, float value)
        {
            value = capPersonalityTraitScore(value);
            if (value != 0) // Discard zero and negative values;
            {
                myPersonalityTraits[targetPersonalityTrait] = value;
            }
        }
        public float? GetTraitScore(PersonalityTraitSO targetPersonalityTrait)
        {
            if (myPersonalityTraits.ContainsKey(targetPersonalityTrait))
            {
                return myPersonalityTraits[targetPersonalityTrait];
            }
            else
            {
                return null;
            }
        }
        // The full dict.
        internal void SetTraitScoreDict(SerializedDictionary<PersonalityTraitSO, float> personalityTraitDict, bool capScores = true)
        {
            if (capScores)
            {

                foreach (PersonalityTraitSO key in personalityTraitDict.Keys)
                {
                    personalityTraitDict[key] = capPersonalityTraitScore(personalityTraitDict[key]);
                }
            }

            this.myPersonalityTraits = personalityTraitDict;
        }
        internal SerializedDictionary<PersonalityTraitSO, float> GetTraitScoreDict()
        {
            return myPersonalityTraits;
        }


        // INTERESTS
        private float capInterestScore(float value)
        {
            if (value < -Constants.INTEREST_HALF_SPAN)
                value = -Constants.INTEREST_HALF_SPAN;
            else if (value > Constants.INTEREST_HALF_SPAN)
                value = Constants.INTEREST_HALF_SPAN;

            return value;
        }
        // Single scores.
        public void SetInterestScore(InterestSO targetInterestSO, float value)
        {
            myInterests[targetInterestSO] = capInterestScore(value);
        }
        public float? GetInterestScore(InterestSO targetInterestSO)
        {
            if (myInterests.ContainsKey(targetInterestSO))
            {
                return myInterests[targetInterestSO];
            }
            else
            {
                return null;
            }
        }
        // The full dict.
        internal void SetInterestScoreDict(SerializedDictionary<InterestSO, float> interestDict, bool capScores = true)
        {
            if (capScores)
            {

                foreach (InterestSO key in interestDict.Keys)
                {
                    interestDict[key] = capInterestScore(interestDict[key]);
                }
            }

            this.myInterests = interestDict;
        }
        internal SerializedDictionary<InterestSO, float> GetInterestScoreDict()
        {
            return myInterests;
        }
    }
}