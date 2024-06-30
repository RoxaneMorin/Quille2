using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class PersonFactory : MonoBehaviour
    {
        // VARIABLES



        PersonalityController personalityController;

        (Quille.PersonalityAxeSO, float)[] axeSOValuePairs;





        // METHODS


        // PERSONALITY CONTROLLER
        PersonalityController InitPersonalityController()
        {
            personalityController = new PersonalityController();

            System.Array.ForEach(axeSOValuePairs, axeSOValuePair => personalityController.SetAxeScore(axeSOValuePair.Item1, axeSOValuePair.Item2));


            return personalityController;
        }
        
    }
}

