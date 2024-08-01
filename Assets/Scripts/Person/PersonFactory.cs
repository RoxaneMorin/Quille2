using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class PersonFactory : MonoBehaviour
    {
        // VARIABLES



        Person_Character personalityController;

        (Quille.PersonalityAxeSO, float)[] axeSOValuePairs;





        // METHODS


        // PERSONALITY CONTROLLER
        Person_Character InitPersonalityController()
        {
            personalityController = new Person_Character();

            System.Array.ForEach(axeSOValuePairs, axeSOValuePair => personalityController.SetAxeScore(axeSOValuePair.Item1, axeSOValuePair.Item2));


            return personalityController;
        }
        
    }
}

