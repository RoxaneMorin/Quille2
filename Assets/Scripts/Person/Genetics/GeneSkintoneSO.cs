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
    class GeneSkintoneSO : GeneSO
    {
        // VARIABLES/PARAMS - OVERRIDES
        [SerializeField] private SkinColourFamily colourFamily;


        // PROPERTIES
        public SkinColourFamily ColourFamily { get { return colourFamily; } }
    }
}