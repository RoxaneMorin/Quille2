using UnityEngine;

namespace ChecksAndMods
{
    public static class Fetchers
    {
        // Static methods which acquire specific data from a given object or character.
        // The desired value is usually represented by its template or key scriptableObject.

        // TODO: split into partial classes eventually.

        // PERSON

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


        // Fetch a specific drive score.
        public static float? FetchDriveScore(System.Object sourceObj, Quille.DriveSO relevantDrive)
        {
            Quille.Person_Character sourcePersonCharacter;

            if (sourceObj is Quille.Person)
            {
                Quille.Person sourceQuille = (Quille.Person)sourceObj;
                sourcePersonCharacter = sourceQuille.MyPersonCharacter;
                return FetchDriveScoreFromPersonCharacter(sourcePersonCharacter, relevantDrive);
            }
            else if (sourceObj is Quille.Person_Character)
            {
                return FetchDriveScoreFromPersonCharacter((Quille.Person_Character)sourceObj, relevantDrive);
            }
            else
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in a Drive modulator.\nThis modulator or check will return null.", sourceObj.ToString()));
                return null;
            }
        }
        public static float? FetchDriveScoreFromPersonCharacter(Quille.Person_Character sourcePersonalityController, Quille.DriveSO relevantDrive)
        {
            return sourcePersonalityController.GetDriveScore(relevantDrive);
        }


        // Fetch a specific interest score.
        public static float? FetchInterestScore(System.Object sourceObj, Quille.InterestSO relevantInterest)
        {
            Quille.Person_Character sourcePersonCharacter;

            if (sourceObj is Quille.Person)
            {
                Quille.Person sourceQuille = (Quille.Person)sourceObj;
                sourcePersonCharacter = sourceQuille.MyPersonCharacter;
                return FetchInterestScoreFromPersonCharacter(sourcePersonCharacter, relevantInterest);
            }
            else if (sourceObj is Quille.Person_Character)
            {
                return FetchInterestScoreFromPersonCharacter((Quille.Person_Character)sourceObj, relevantInterest);
            }
            else
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in an Interest modulator.\nThis modulator or check will return null.", sourceObj.ToString()));
                return null;
            }
        }
        public static float? FetchInterestScoreFromPersonCharacter(Quille.Person_Character sourcePersonalityController, Quille.InterestSO relevantInterest)
        {
            return sourcePersonalityController.GetInterestScore(relevantInterest);
        }


        // TODO: fetch need levels and the like.

        public static float? FetchBasicNeedLevel(System.Object sourceObj, Quille.BasicNeedSO relevantNeed, bool asPercentage = false)
        {
            Quille.Person_NeedController sourceNeedController;

            if (sourceObj is Quille.Person)
            {
                Quille.Person sourceQuille = (Quille.Person)sourceObj;
                sourceNeedController = sourceQuille.MyNeedController;
                return FetchBasicNeedLevelFromPersonNeedController(sourceNeedController, relevantNeed, asPercentage);
            }
            else if (sourceObj is Quille.Person_NeedController)
            {
                return FetchBasicNeedLevelFromPersonNeedController((Quille.Person_NeedController)sourceObj, relevantNeed, asPercentage);
            }
            else
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in a BasicNeed modulator.\nThis modulator or check will return null.", sourceObj.ToString()));
                return null;
            }
        }
        public static float? FetchBasicNeedLevelFromPersonNeedController(Quille.Person_NeedController sourceNeedController, Quille.BasicNeedSO relevantNeed, bool asPercentage = false)
        {
            Quille.BasicNeed theBasicNeed = sourceNeedController.GetBasicNeed(relevantNeed);

            if (theBasicNeed != null)
            {
                return (asPercentage ? theBasicNeed.LevelCurrentAsPercentage : theBasicNeed.LevelCurrent);
            }
            else
            {
                return null;
            }
        }

        public static float? FetchSubjectiveNeedLevel(System.Object sourceObj, Quille.SubjectiveNeedSO relevantNeed, Quille.BasicNeedSO relevantSubneed, bool asPercentage = false)
        {
            Quille.Person_NeedController sourceNeedController;

            if (sourceObj is Quille.Person)
            {
                Quille.Person sourceQuille = (Quille.Person)sourceObj;
                sourceNeedController = sourceQuille.MyNeedController;
                return FetchSubjectiveNeedLevelFromPersonNeedController(sourceNeedController, relevantNeed, relevantSubneed, asPercentage);
            }
            else if (sourceObj is Quille.Person_NeedController)
            {
                return FetchSubjectiveNeedLevelFromPersonNeedController((Quille.Person_NeedController)sourceObj, relevantNeed, relevantSubneed, asPercentage);
            }
            else
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in a BasicNeed modulator.\nThis modulator or check will return null.", sourceObj.ToString()));
                return null;
            }
        }
        public static float? FetchSubjectiveNeedLevelFromPersonNeedController(Quille.Person_NeedController sourceNeedController, Quille.SubjectiveNeedSO relevantNeed, Quille.BasicNeedSO relevantSubneed, bool asPercentage = false)
        {
            Quille.SubjectiveNeed theSubjectiveNeed = sourceNeedController.GetSubjectiveNeed(relevantNeed);

            if (theSubjectiveNeed != null)
            {
                return (asPercentage ? theSubjectiveNeed.LevelCurrentAsPercentageFor(relevantSubneed) : theSubjectiveNeed.LevelCurrentFor(relevantSubneed));
            }
            else
            {
                return null;
            }
        }
    }
}

