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

        [SerializeField] private ModulatorArithmeticFromFloat testModArithmeticFromFloat;
        [SerializeField] private ModulatorArithmeticFromBool testModArithmeticFromBool;

        [Header("Checks")]

        [SerializeReference][PopulateCheckSubtypes] private CheckArithmetic testCheckArithmetic;
        [SerializeReference][PopulateCheckSubtypes] private CheckBoolean testCheckBoolean;

        [Space]

        [SerializeField] private CheckArithmeticDriveScore testDriveScore;


        // METHODS

        // BUILT IN
        void Start()
        { }

        void Update()
        { }
    }
}

