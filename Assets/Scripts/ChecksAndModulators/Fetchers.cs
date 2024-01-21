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
            // TO DO: edit to be able to receive both a basePerson and a personalityController.
            
            Quille.PersonalityController sourcePersonalityController;

            // Will need to be cleaned up/refined to ensure safety and efficiency.
            try
            {
                sourcePersonalityController = (Quille.PersonalityController)sourceObj;
            }
            catch
            {
                Debug.LogError(string.Format("The input object '{0}' is of the wrong type and cannot be used in a PersonalityAxe modulator.", sourceObj.name));
                throw;
            }

            return sourcePersonalityController.GetScore(relevantPersonalityAxe);
        }
    }
}

