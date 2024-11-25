using UnityEngine;

namespace ChecksAndMods
{
    public static class Fetchers
    {
        // Static methods which acquire specific data from a given object or character.
        // The desired value is usually represented by its template or key scriptableObject.


        // Fetch a specific personality axe score.
        public static float? FetchPersonalityAxeScore(System.Object sourceObj, Quille.PersonalityAxeSO relevantPersonalityAxe)
        {
            Quille.Person_Character sourcePersonCharacter;

            if (sourceObj is Quille.Person)
            {
                Quille.Person sourceQuille = (Quille.Person)sourceObj;
                sourcePersonCharacter = sourceQuille.MyPersonCharacter;
                return FetchAxeScoreFromPersonCharacter(sourcePersonCharacter, relevantPersonalityAxe);
            }
            else if (sourceObj is Quille.Person_Character)
            {
                return FetchAxeScoreFromPersonCharacter((Quille.Person_Character)sourceObj, relevantPersonalityAxe);
            }
            else
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in a PersonalityAxe modulator.\nThis modulator or check will return null.", sourceObj.ToString()));
                return null;
            }
        }
        public static float? FetchAxeScoreFromPersonCharacter(Quille.Person_Character sourcePersonCharacter, Quille.PersonalityAxeSO relevantPersonalityAxe)
        {
            return sourcePersonCharacter.GetAxeScore(relevantPersonalityAxe);
        }


        // Fetch a specific personality trait score.
        public static float? FetchPersonalityTraitScore(System.Object sourceObj, Quille.PersonalityTraitSO relevantPersonalityTrait)
        {
            Quille.Person_Character sourcePersonCharacter;

            if (sourceObj is Quille.Person)
            {
                Quille.Person sourceQuille = (Quille.Person)sourceObj;
                sourcePersonCharacter = sourceQuille.MyPersonCharacter;
                return FetchTraitScoreFromPersonCharacter(sourcePersonCharacter, relevantPersonalityTrait);
            }
            else if (sourceObj is Quille.Person_Character)
            {
                return FetchTraitScoreFromPersonCharacter((Quille.Person_Character)sourceObj, relevantPersonalityTrait);
            }
            else
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in a PersonalityAxe modulator.\nThis modulator or check will return null.", sourceObj.ToString()));
                return null;
            }
        }
        public static float? FetchTraitScoreFromPersonCharacter(Quille.Person_Character sourcePersonalityController, Quille.PersonalityTraitSO relevantPersonalityTrait)
        {
            return sourcePersonalityController.GetTraitScore(relevantPersonalityTrait);
        }


        // TODO: fetch a specific interest score.
    }
}

