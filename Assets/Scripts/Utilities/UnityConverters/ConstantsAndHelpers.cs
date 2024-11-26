using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Quille;

namespace Newtonsoft.Json.UnityConverters.Quille
{
    public static class ConstantsAndHelpers
    {
        // Constant and helper methods used by the custon JsonConverters of Quille classes.
        // Many are fragile as they rely on hardcoded path and resource names.


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
            string path = PathConstants.SO_PATH_PERSONALITYAXES + nameString;

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
            string path = PathConstants.SO_PATH_PERSONALITYTRAITS + nameString;

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

        public static DriveSO loadDriveSO(string nameString)
        {
            string path = PathConstants.SO_PATH_DRIVES + nameString;

            try
            {
                DriveSO theSO = Resources.Load<DriveSO>(path);
                return theSO;
            }
            catch
            {
                Debug.LogError(string.Format("Tried and failed to load a DriveSO with the name {0}.", nameString));
                return null;
            }
        }

        public static InterestSO loadInterestSO(string nameString)
        {
            string path = PathConstants.SO_PATH_INTERESTS + nameString;

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
            string path = PathConstants.SO_PATH_NEEDSBASIC + nameString;

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
            string path = PathConstants.SO_PATH_NEEDSSUBJECTIVE + nameString;

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
