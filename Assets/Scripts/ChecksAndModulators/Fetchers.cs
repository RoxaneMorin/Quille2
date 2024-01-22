using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public static class Fetchers
    {
        // Fetch a specific personality score.
        public static float FetchPersonalityAxeScore(UnityEngine.Object sourceObj, Quille.PersonalityAxeSO relevantPersonalityAxe)
        {
            Quille.PersonalityController sourcePersonalityController;

            if (sourceObj is Quille.BasePerson)
            {
                Quille.BasePerson sourceQuille = (Quille.BasePerson)sourceObj;
                sourcePersonalityController = sourceQuille.MyPersonalityController;
                return FetchFromPersonalityController(sourcePersonalityController, relevantPersonalityAxe);
            }
            else if (sourceObj is Quille.PersonalityController)
            {
                sourcePersonalityController = (Quille.PersonalityController)sourceObj;
                return FetchFromPersonalityController(sourcePersonalityController, relevantPersonalityAxe);
            }
            else 
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in a PersonalityAxe modulator.\nThis modulator or check will return a zero.", sourceObj.name));
                return 0;
                // Throw error.
            }
        }

        public static float FetchFromPersonalityController(Quille.PersonalityController sourcePersonalityController, Quille.PersonalityAxeSO relevantPersonalityAxe)
        {
            return sourcePersonalityController.GetScore(relevantPersonalityAxe);
        }
    }
}

