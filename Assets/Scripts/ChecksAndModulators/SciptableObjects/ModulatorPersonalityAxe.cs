using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    [CreateAssetMenu(fileName = "ModulatorPersonalityAxe", menuName = "Checks&dModulators/Modulators/Personality Axe", order = 0)]
    public class ModulatorPersonalityAxe : ModulatorAlterFloatFromFloatSO
    {
        // VARIABLES/PARAM
        [SerializeField]
        private Quille.PersonalityAxeSO relevantPersonalityAxe;


        // METHODS
        // Fetch the value of the revelant personality axe.
        protected override void FetchParam(UnityEngine.Object sourceObj)
        {
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

            param = sourcePersonalityController.GetScore(relevantPersonalityAxe);
        }

        // Modulate defined in parent class.
    }
}
