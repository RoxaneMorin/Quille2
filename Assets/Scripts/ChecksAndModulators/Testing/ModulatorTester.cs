using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulatorTester : MonoBehaviour
{
    public Quille.PersonalityController personalityController;
    
    public float originalTargetForFloat = 1;
    public float finalResultForFloat;
    public ChecksAndMods.ModulatorAlterFloatFromFloat[] floatModulators;


    public float originalTargetForBool = 1;
    public float finalResultForBool;
    public ChecksAndMods.ModulatorAlterFloatFromBool[] boolModulators;

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
        }
    }

    void TestFloatModulators()
    {
        finalResultForFloat = originalTargetForFloat;

        foreach (ChecksAndMods.ModulatorAlterFloatFromFloat modulator in floatModulators)
            finalResultForFloat = modulator.Execute(personalityController, finalResultForFloat);
    }

    void TestBoolModulators()
    {
        finalResultForBool = originalTargetForBool;

        foreach (ChecksAndMods.ModulatorAlterFloatFromBool modulator in boolModulators)
            finalResultForBool = modulator.Execute(personalityController, finalResultForBool);
    }
}
