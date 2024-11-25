using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of a generic gene, from which others will derive.


    // BASE GENE TEMPLATE
    //[System.Serializable]
    public abstract class Gene : ScriptableObject
    {
        // LOCAL DEFINES
        internal enum ColourFamily {none};

        // VARIABLES/PARAMS
        [SerializeField]
        internal int idNumber;
        [SerializeField]
        internal string colourName;
        [SerializeField]
        internal ColourFamily colourFamily;
        // TODO: remove the colourFamily here to avoid having to replace it later on?
        [SerializeField, ColorUsage(false, false)]
        internal Color colour = Color.black;
        [SerializeField, Range(-1, 3)] // '-1' is not inheritable.
        internal int colourDominance;
    }
}