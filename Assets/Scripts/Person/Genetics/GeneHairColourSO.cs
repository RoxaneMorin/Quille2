using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of a hair colour gene, used for the creation of specific hair colours as assets.
    // They are to be used in the character genetic system.


    // HAIR COLOUR GENE
    [CreateAssetMenu(fileName = "GeneHairColour", menuName = "Quille/Genetics/HairColour", order = 1)]
    class GeneHairColourSO : GeneWithDominanceSO
    {
        // VARIABLES/PARAMS - OVERRIDES
        [SerializeField] private HairColourFamily colourFamily;


        // PROPERTIES
        public HairColourFamily ColourFamily { get { return colourFamily; } }
    }
}