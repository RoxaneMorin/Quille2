﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // BASE GENE TEMPLATE
    //[System.Serializable]
    public class Gene : ScriptableObject
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
        [SerializeField, ColorUsage(false, false)]
        internal Color colour = Color.black;
        [SerializeField, Range(-1, 3)] // '-1' is not inheritable.
        internal int colourDominance;
    }
}