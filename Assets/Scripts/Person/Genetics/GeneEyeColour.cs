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
    class GeneEyeColour : GeneWithDominance
    {
        // VARIABLES/PARAMS
        [SerializeField] private EyeColourFamily colourFamily;


        // PROPERTIES
        public EyeColourFamily ColourFamily { get { return colourFamily; } }
    }
}