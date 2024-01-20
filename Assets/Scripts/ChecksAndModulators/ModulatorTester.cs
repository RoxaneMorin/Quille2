using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulatorTester : MonoBehaviour
{
    public Quille.PersonalityController personalityController;
    
    public float originalTarget = 1;
    public float finalResult;
    public ChecksAndMods.ModulatorAlterFloatFromFloat[] modulators;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestModulators();
        }
    }

    void TestModulators()
    {
        finalResult = originalTarget;

        foreach (ChecksAndMods.ModulatorAlterFloatFromFloat modulator in modulators)
            finalResult = modulator.Execute(personalityController, finalResult);
    }
}
