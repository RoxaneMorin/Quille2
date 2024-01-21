using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulatorTester : MonoBehaviour
{
    public Quille.PersonalityController personalityController;
    
    public float originalTargetForFloat = 1;
    public float finalResultForFloat;
    public ChecksAndMods.ModulatorArithmeticFromFloat[] floatModulators;


    public float originalTargetForBool = 1;
    public float finalResultForBool;
    public ChecksAndMods.ModulatorArithmeticFromBool[] boolModulators;

    public bool checkTester;
    public ChecksAndMods.CheckArithmetic[] arithmeticChecks;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestFloatModulators();
            TestBoolModulators();

            TestArithmeticChecks();
        }
    }

    void TestFloatModulators()
    {
        finalResultForFloat = originalTargetForFloat;

        foreach (ChecksAndMods.ModulatorArithmeticFromFloat modulator in floatModulators)
            finalResultForFloat = modulator.Execute(personalityController, finalResultForFloat);
    }

    void TestBoolModulators()
    {
        finalResultForBool = originalTargetForBool;

        foreach (ChecksAndMods.ModulatorArithmeticFromBool modulator in boolModulators)
            finalResultForBool = modulator.Execute(personalityController, finalResultForBool);
    }

    void TestArithmeticChecks()
    {
        foreach (ChecksAndMods.CheckArithmetic check in arithmeticChecks)
            checkTester = check.Execute(personalityController);
    }
}
