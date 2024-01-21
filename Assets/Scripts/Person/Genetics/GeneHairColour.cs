using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // HAIR COLOUR GENE
    [CreateAssetMenu(fileName = "GeneHairColour", menuName = "Quille/Genetics/HairColour", order = 1)]
    //[System.Serializable]
    class GeneHairColour : Gene
    {
        // LOCAL DEFINES
        new enum ColourFamily { none, Main, Desat, Red, Supernatural, Grey };

        // VARIABLES/PARAMS - OVERRIDES
        new ColourFamily colourFamily;
    }
}