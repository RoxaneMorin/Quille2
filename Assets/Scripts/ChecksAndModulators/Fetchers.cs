using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public static class Fetchers
    {
        // Fetch a specific personality axe score.
        public static float FetchPersonalityAxeScore(UnityEngine.Object sourceObj, Quille.PersonalityAxeSO relevantPersonalityAxe)
        {
            Quille.PersonalityController sourcePersonalityController;

            if (sourceObj is Quille.BasePerson)
            {
                Quille.BasePerson sourceQuille = (Quille.BasePerson)sourceObj;
                sourcePersonalityController = sourceQuille.MyPersonalityController;
                return FetchAxeScoreFromPersonalityController(sourcePersonalityController, relevantPersonalityAxe);
            }
            else if (sourceObj is Quille.PersonalityController)
            {
                sourcePersonalityController = (Quille.PersonalityController)sourceObj;
                return FetchAxeScoreFromPersonalityController(sourcePersonalityController, relevantPersonalityAxe);
            }
            else 
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in a PersonalityAxe modulator.\nThis modulator or check will return a zero.", sourceObj.name));
                return 0;
                // Throw error.
            }
        }
        public static float FetchAxeScoreFromPersonalityController(Quille.PersonalityController sourcePersonalityController, Quille.PersonalityAxeSO relevantPersonalityAxe)
        {
            return sourcePersonalityController.GetAxeScore(relevantPersonalityAxe);
        }


        // Fetch a specific personality trait score.
        public static float FetchPersonalityTraitScore(UnityEngine.Object sourceObj, Quille.PersonalityTraitSO relevantPersonalityTrait)
        {
            Quille.PersonalityController sourcePersonalityController;

            if (sourceObj is Quille.BasePerson)
            {
                Quille.BasePerson sourceQuille = (Quille.BasePerson)sourceObj;
                sourcePersonalityController = sourceQuille.MyPersonalityController;
                return FetchTraitScoreFromPersonalityController(sourcePersonalityController, relevantPersonalityTrait);
            }
            else if (sourceObj is Quille.PersonalityController)
            {
                sourcePersonalityController = (Quille.PersonalityController)sourceObj;
                return FetchTraitScoreFromPersonalityController(sourcePersonalityController, relevantPersonalityTrait);
            }
            else
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in a PersonalityAxe modulator.\nThis modulator or check will return a zero.", sourceObj.name));
                return 0;
                // Throw error.
            }
        }
        public static float FetchTraitScoreFromPersonalityController(Quille.PersonalityController sourcePersonalityController, Quille.PersonalityTraitSO relevantPersonalityTrait)
        {
            return sourcePersonalityController.GetTraitScore(relevantPersonalityTrait);
        }
    }
}

