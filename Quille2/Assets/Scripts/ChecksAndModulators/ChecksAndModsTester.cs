using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChecksAndMods
{
    public class ChecksAndModsTester : MonoBehaviour
    {
        // Tester class for the various types of checks and modulators.


        // VARIABLES/PARAM
        [Header("Modulators")]
        [SerializeReference][PopulateModulatorSubtypes] private Modulator[] testMods;

        [Header("Checks")]
        [SerializeReference][PopulateCheckSubtypes] private Check[] testChecks;


        // METHODS

        // BUILT IN
        void Start()
        { }

        void Update()
        { }
    }
}

