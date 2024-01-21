using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
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