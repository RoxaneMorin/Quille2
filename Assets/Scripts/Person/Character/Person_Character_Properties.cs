using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Newtonsoft.Json;

using System.IO;

namespace Quille
{
    // The C# object class containing a person's identity and personality. 
    // This data is editable, but needs not be updated or accessed at every tick.
    // Properties and helper methods only.
    // This part of the class is not JSON serialized.

    public partial class Person_Character
    {
        // PROPERTIES & THEIR HELPER METHODS

        // IDENTITY

        // NAMES
        [JsonIgnore] public string FirstName { get { return firstName; } set { firstName = value; } }
        [JsonIgnore] public string LastName { get { return lastName; } set { lastName = value; } }
        [JsonIgnore] public string NickName { get { return nickName; } set { nickName = value; } }
        [JsonIgnore] public List<string> SecondaryNames { get { return secondaryNames; } set { secondaryNames = value; } }
        // TODO: review how this should be handled. We likely won't be manipulating the full set at once.
        // Check the existance of a secondary name.
        // Add secondary name.
        // Remove secondary name.
        // TODO: same thing for the following.
        [JsonIgnore] public string FirstAndLastName { get { return string.Format("{0} {1}", firstName, lastName); } } // Name order?
        [JsonIgnore] public string FirstNickAndLastName { get { return (nickName != "" ? string.Format("{0} '{1}' {2}", firstName, nickName, lastName) : FirstAndLastName); } }
        [JsonIgnore] public string FullName { get {

                if (secondaryNames != null & secondaryNames.Count > 0)
                {
                    return (nickName != "" ? string.Format("{0} {1} {2}, '{3}'", firstName, string.Join(" ", secondaryNames), lastName, nickName) : string.Format("{0} {1} {2}", firstName, string.Join(" ", secondaryNames), lastName));
                }
                else 
                    return FirstNickAndLastName;
            } }


        // Age, gender, etc.


        // PERSONALITY

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
                SerializedDictionary<PersonalityAxeSO, float> newPersonalityAxeDict = new SerializedDictionary<PersonalityAxeSO, float>();

                foreach (PersonalityAxeSO key in personalityAxeDict.Keys)
                {
                    newPersonalityAxeDict[key] = capPersonalityAxeScore(personalityAxeDict[key]);
                }

                this.myPersonalityAxes = newPersonalityAxeDict;
            }
            else
            {
                this.myPersonalityAxes = personalityAxeDict;
            }
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
            else if (value <= Constants.PERSONALITY_HALF_SPAN / 2)
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
                SerializedDictionary<PersonalityTraitSO, float> newPersonalityTraitDict = new SerializedDictionary<PersonalityTraitSO, float>();

                foreach (PersonalityTraitSO key in personalityTraitDict.Keys)
                {
                    newPersonalityTraitDict[key] = capPersonalityTraitScore(personalityTraitDict[key]);
                }

                this.myPersonalityTraits = newPersonalityTraitDict;
            }
            else
            {
                this.myPersonalityTraits = personalityTraitDict;
            }
        }
        internal SerializedDictionary<PersonalityTraitSO, float> GetTraitScoreDict()
        {
            return myPersonalityTraits;
        }


        // DRIVES
        private float capDriveScore(float value)
        {
            // Drives can only have a value of 0.5 or 1.
            if (value <= 0) // Discard zero and negative values;
            {
                return 0;
            }
            else if (value <= Constants.DRIVE_SPAN / 2)
            {
                return Constants.DRIVE_SPAN / 2;
            }
            return Constants.DRIVE_SPAN;
        }

        // Single scores.
        public void SetDriveScore(DriveSO targetDrive, float value)
        {
            value = capDriveScore(value);
            if (value != 0) // Discard zero and negative values;
            {
                myDrives[targetDrive] = value;
            }
        }
        public float? GetDriveScore(DriveSO targetDrive)
        {
            if (myDrives.ContainsKey(targetDrive))
            {
                return myDrives[targetDrive];
            }
            else
            {
                return null;
            }
        }

        // The full dict.
        internal void SetDriveScoreDict(SerializedDictionary<DriveSO, float> driveDict, bool capScores = true)
        {
            if (capScores)
            {
                SerializedDictionary<DriveSO, float> newDriveDict = new SerializedDictionary<DriveSO, float>();

                foreach (DriveSO key in driveDict.Keys)
                {
                    newDriveDict[key] = capDriveScore(driveDict[key]);
                }

                this.myDrives = newDriveDict;
            }
            else
            {
                this.myDrives = driveDict;
            }
        }
        internal SerializedDictionary<DriveSO, float> GetDriveScoreDict()
        {
            return myDrives;
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
                SerializedDictionary<InterestSO, float> newInterestDict = new SerializedDictionary<InterestSO, float>();

                foreach (InterestSO key in interestDict.Keys)
                {
                    newInterestDict[key] = capInterestScore(interestDict[key]);
                }

                this.myInterests = newInterestDict;
            }
            else
            {
                this.myInterests = interestDict;
            }
        }
        internal SerializedDictionary<InterestSO, float> GetInterestScoreDict()
        {
            return myInterests;
        }
    }
}
