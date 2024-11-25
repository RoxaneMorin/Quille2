using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of an eye colour gene, used for the creation of specific eye colours as assets.
    // They are to be used in the character genetic system.


    // EYE COLOUR GENE
    [CreateAssetMenu(fileName = "GeneEyeColour", menuName = "Quille/Genetics/EyeColour", order = 2)]
    //[System.Serializable]
    class GeneEyeColour : Gene
    {
        // LOCAL DEFINES - OVERRIDES
        new enum ColourFamily { none, BrownHazelGreen, TealBlueGrey, Supernatural };

        // VARIABLES/PARAMS - OVERRIDES
        new ColourFamily colourFamily;
    }
}