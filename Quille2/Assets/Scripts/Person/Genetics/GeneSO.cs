using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of a generic gene, from which others will derive.


    // BASE GENE TEMPLATE
    public abstract class GeneSO : MenuSortableScriptableObject
    {
        // VARIABLES/PARAMS
        [SerializeField] protected string colourName;
        [SerializeField, ColorUsage(false, false)] protected Color colour = Color.black;


        // PROPERTIES
        public string ColourName { get { return colourName; } }
        public Color Colour { get { return colour; } }
    }

    public abstract class GeneWithDominanceSO : GeneSO
    {
        // VARIABLES/PARAMS
        [SerializeField, Range(-1, 3)] protected int geneDominance; // '-1' is not inheritable.


        // PROPERTIES
        public int GeneDominance { get { return geneDominance; } }
        public bool DominanceInheritability { get { return geneDominance > -1; } }
    }
}