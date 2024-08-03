using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public static class Fetchers
    {
        // Fetch a specific personality axe score.
        public static float? FetchPersonalityAxeScore(System.Object sourceObj, Quille.PersonalityAxeSO relevantPersonalityAxe)
        {
            Quille.Person_Character sourcePersonalityController;

            if (sourceObj is Quille.Person)
            {
                Quille.Person sourceQuille = (Quille.Person)sourceObj;
                sourcePersonalityController = sourceQuille.MyPersonalityController;
                return FetchAxeScoreFromPersonalityController(sourcePersonalityController, relevantPersonalityAxe);
            }
            else if (sourceObj is Quille.Person_Character)
            {
                return FetchAxeScoreFromPersonalityController((Quille.Person_Character)sourceObj, relevantPersonalityAxe);
            }
            else
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in a PersonalityAxe modulator.\nThis modulator or check will return null.", sourceObj.ToString()));
                return null;
            }
        }
        public static float? FetchAxeScoreFromPersonalityController(Quille.Person_Character sourcePersonalityController, Quille.PersonalityAxeSO relevantPersonalityAxe)
        {
            return sourcePersonalityController.GetAxeScore(relevantPersonalityAxe);
        }


        // Fetch a specific personality trait score.
        public static float? FetchPersonalityTraitScore(System.Object sourceObj, Quille.PersonalityTraitSO relevantPersonalityTrait)
        {
            Quille.Person_Character sourcePersonalityController;

            if (sourceObj is Quille.Person)
            {
                Quille.Person sourceQuille = (Quille.Person)sourceObj;
                sourcePersonalityController = sourceQuille.MyPersonalityController;
                return FetchTraitScoreFromPersonalityController(sourcePersonalityController, relevantPersonalityTrait);
            }
            else if (sourceObj is Quille.Person_Character)
            {
                return FetchTraitScoreFromPersonalityController((Quille.Person_Character)sourceObj, relevantPersonalityTrait);
            }
            else
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in a PersonalityAxe modulator.\nThis modulator or check will return null.", sourceObj.ToString()));
                return null;
            }
        }
        public static float? FetchTraitScoreFromPersonalityController(Quille.Person_Character sourcePersonalityController, Quille.PersonalityTraitSO relevantPersonalityTrait)
        {
            return sourcePersonalityController.GetTraitScore(relevantPersonalityTrait);
        }


        // TODO: fetch a specific interest score.
    }
}

