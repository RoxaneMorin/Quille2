using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public static class ConstantsAndHelpers
    {
        // CONSTANTS

        // Resource folder paths.
        public const string PATH_PERSONALITYAXES = "ScriptableObjects/Personality/Axes/";
        public const string PATH_PERSONALITYTRAITS = "ScriptableObjects/Personality/Traits/";
        public const string PATH_INTERESTS = "ScriptableObjects/Interests/Fields/";
        public const string PATH_BASICNEEDS = "ScriptableObjects/Needs/Basic/";
        public const string PATH_SUBJECTIVENEEDS = "ScriptableObjects/Needs/Subjective/";



        // METHODS

        // Extension methods.
        public static float? ReadAsFloat(this JsonReader reader)
        {
            // Copy-pasted from the JsonHelperExtensions as it is internal and idk how to ping Unity.

            var str = reader.ReadAsString();

            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            else if (float.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var valueParsed))
            {
                return valueParsed;
            }
            else
            {
                return 0f;
            }
        }


        // Load scriptable objects from name string.
        public static PersonalityAxeSO loadPersonalityAxeSO(string nameString)
        {
            string path = ConstantsAndHelpers.PATH_PERSONALITYAXES + nameString;

            try
            {
                PersonalityAxeSO theSO = Resources.Load<PersonalityAxeSO>(path);
                return theSO;
            }
            catch
            {
                Debug.LogError(string.Format("Tried and failed to load a PersonalityAxeSO with the name {0}.", nameString));
                return null;
            }
        }
        public static PersonalityTraitSO loadPersonalityTraitSO(string nameString)
        {
            string path = ConstantsAndHelpers.PATH_PERSONALITYTRAITS + nameString;

            try
            {
                PersonalityTraitSO theSO = Resources.Load<PersonalityTraitSO>(path);
                return theSO;
            }
            catch
            {
                Debug.LogError(string.Format("Tried and failed to load a PersonalityTraitSO with the name {0}.", nameString));
                return null;
            }
        }

        public static InterestSO loadInterestSO(string nameString)
        {
            string path = ConstantsAndHelpers.PATH_INTERESTS + nameString;

            try
            {
                InterestSO theSO = Resources.Load<InterestSO>(path);
                return theSO;
            }
            catch
            {
                Debug.LogError(string.Format("Tried and failed to load an InterestSO with the name {0}.", nameString));
                return null;
            }
        }

        public static BasicNeedSO loadBasicNeedSO(string nameString)
        {
            string path = ConstantsAndHelpers.PATH_BASICNEEDS + nameString;

            try
            {
                BasicNeedSO theSO = Resources.Load<BasicNeedSO>(path);
                return theSO;
            }
            catch
            {
                Debug.LogError(string.Format("Tried and failed to load a BasicNeedSO with the name {0}.", nameString));
                return null;
            }
        }
        public static SubjectiveNeedSO loadSubjectiveNeedSO(string nameString)
        {
            string path = ConstantsAndHelpers.PATH_SUBJECTIVENEEDS + nameString;

            try
            {
                SubjectiveNeedSO theSO = Resources.Load<SubjectiveNeedSO>(path);
                return theSO;
            }
            catch
            {
                Debug.LogError(string.Format("Tried and failed to load a SubjectiveNeedSO with the name {0}.", nameString));
                return null;
            }
        }
    }
}
