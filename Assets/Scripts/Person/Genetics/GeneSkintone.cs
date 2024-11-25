using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of a skin colour gene, used for the creation of specific skin colours as assets.
    // They are to be used in the character genetic system.


    // SKINTONE DEFAULTS
    [CreateAssetMenu(fileName = "DefaultSkintone", menuName = "Quille/Genetics/Skintone", order = 0)]
    //[System.Serializable]
    class GeneSkintone : Gene
    {
        // LOCAL DEFINES - OVERRIDES
        new enum ColourFamily { none, Main, Warm, Cool, RedPurple, BlueGreen };

        // VARIABLES/PARAMS - OVERRIDES
        new ColourFamily colourFamily;
        new Color colour = Color.white;
        // ! Potentially use colour dominance to weigth for or against supernatural colours? !
        new int colourDominance = 1;

        // As skintones do not use dominance values, freeze the variable.
        //[Tooltip("Warning : Unused"), Range(0,0)]
        //new int colourDominance = 0;
    }
}