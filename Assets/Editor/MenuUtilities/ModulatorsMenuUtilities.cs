using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ModulatorsMenuUtilities : MonoBehaviour
{
    // Editor utilities for the creation and management of ModulatorSO instances.


    // METHODS

    // Menu methods.
    [MenuItem("Quille/Person/ChecksAndMods/Create missing PersonalityAxe modulators.")]
    static void CreatePersonalityAxeModulators()
    {
        // Load relevant resources.
        ChecksAndMods.ModulatorPersonalityAxeScore[] relevantModulatorSOs = Resources.LoadAll<ChecksAndMods.ModulatorPersonalityAxeScore>(PathConstants.SO_PATH_MODULATORS_PERSONALITYAXES);

        List<Quille.PersonalityAxeSO> allPersonalityAxeSOs = Resources.LoadAll<Quille.PersonalityAxeSO>(PathConstants.SO_PATH_PERSONALITYAXES).ToList();
        List<Quille.PersonalityAxeSO> coveredPersonalityAxeSOs = new List<Quille.PersonalityAxeSO>();

        // Populate the list of PersonalityAxeSOs for which modulators already exist.
        foreach (ChecksAndMods.ModulatorPersonalityAxeScore modulator in relevantModulatorSOs)
        {
            coveredPersonalityAxeSOs.Add(modulator.RelevantPersonalityAxe);
        }

        // Find the difference between the two lists.
        List<Quille.PersonalityAxeSO> newPersonalityAxeSOs = allPersonalityAxeSOs.Except(coveredPersonalityAxeSOs).ToList();

        //Create new modSOs for the remaining personalityAxes.
        foreach (Quille.PersonalityAxeSO personalityAxe in newPersonalityAxeSOs)
        {
            ChecksAndMods.ModulatorPersonalityAxeScore newModulatorSO = ScriptableObject.CreateInstance<ChecksAndMods.ModulatorPersonalityAxeScore>();
            newModulatorSO.RelevantPersonalityAxe = personalityAxe;

            string savePath = AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/Resources/{0}Modulator_PersonalityAxe_{1}.asset", PathConstants.SO_PATH_MODULATORS_PERSONALITYAXES, personalityAxe.AxeName.StripComplexChars()));

            AssetDatabase.CreateAsset(newModulatorSO, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("A new instance of ModulatorPersonalityAxeScore was created for the '{0}' PersonalityAxe.", personalityAxe.AxeName));
        }
    }


    [MenuItem("Quille/Person/ChecksAndMods/Create missing PersonalityTrait modulators.")]
    static void CreatePersonalityTraitModulators()
    {
        // Load relevant resources.
        ChecksAndMods.ModulatorPersonalityTraitScore[] relevantModulatorSOs = Resources.LoadAll<ChecksAndMods.ModulatorPersonalityTraitScore>(PathConstants.SO_PATH_MODULATORS_PERSONALITYTRAITS);

        List<Quille.PersonalityTraitSO> allPersonalityTraitSOs = Resources.LoadAll<Quille.PersonalityTraitSO>(PathConstants.SO_PATH_PERSONALITYTRAITS).ToList();
        List<Quille.PersonalityTraitSO> coveredPersonalityTraitSOs = new List<Quille.PersonalityTraitSO>();

        // Populate the list of PersonalityTraitSOs for which modulators already exist.
        foreach (ChecksAndMods.ModulatorPersonalityTraitScore modulator in relevantModulatorSOs)
        {
            coveredPersonalityTraitSOs.Add(modulator.RelevantPersonalityTrait);
        }

        // Find the difference between the two lists.
        List<Quille.PersonalityTraitSO> newPersonalityTraitSOs = allPersonalityTraitSOs.Except(coveredPersonalityTraitSOs).ToList();

        //Create new modSOs for the remaining personalityTraits.
        foreach (Quille.PersonalityTraitSO personalityTrait in newPersonalityTraitSOs)
        {
            ChecksAndMods.ModulatorPersonalityTraitScore newModulatorSO = ScriptableObject.CreateInstance<ChecksAndMods.ModulatorPersonalityTraitScore>();
            newModulatorSO.RelevantPersonalityTrait = personalityTrait;

            string savePath = AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/Resources/{0}Modulator_PersonalityTrait_{1}.asset", PathConstants.SO_PATH_MODULATORS_PERSONALITYTRAITS, personalityTrait.TraitName.StripComplexChars()));

            AssetDatabase.CreateAsset(newModulatorSO, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("A new instance of ModulatorPersonalityTraitScore was created for the '{0}' PersonalityTrait.", personalityTrait.TraitName));
        }
    }


    [MenuItem("Quille/Person/ChecksAndMods/Create missing Interest modulators.")]
    static void CreateInterestModulators()
    {
        // Load relevant resources.
        ChecksAndMods.ModulatorInterestScore[] relevantModulatorSOs = Resources.LoadAll<ChecksAndMods.ModulatorInterestScore>(PathConstants.SO_PATH_MODULATORS_INTERESTS);

        List<Quille.InterestSO> allInterestSOs = Resources.LoadAll<Quille.InterestSO>(PathConstants.SO_PATH_INTERESTS).ToList();
        List<Quille.InterestSO> coveredInterestSOs = new List<Quille.InterestSO>();

        // Populate the list of InterestSOs for which modulators already exist.
        foreach (ChecksAndMods.ModulatorInterestScore modulator in relevantModulatorSOs)
        {
            coveredInterestSOs.Add(modulator.RelevantInterest);
        }

        // Find the difference between the two lists.
        List<Quille.InterestSO> newInterestSOs = allInterestSOs.Except(coveredInterestSOs).ToList();

        //Create new modSOs for the remaining Interests.
        foreach (Quille.InterestSO Interest in newInterestSOs)
        {
            ChecksAndMods.ModulatorInterestScore newModulatorSO = ScriptableObject.CreateInstance<ChecksAndMods.ModulatorInterestScore>();
            newModulatorSO.RelevantInterest = Interest;

            string savePath = AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/Resources/{0}Modulator_Interest_{1}.asset", PathConstants.SO_PATH_MODULATORS_INTERESTS, Interest.InterestName.StripComplexChars()));

            AssetDatabase.CreateAsset(newModulatorSO, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("A new instance of ModulatorInterestScore was created for the '{0}' Interest.", Interest.InterestName));
        }
    }
}
