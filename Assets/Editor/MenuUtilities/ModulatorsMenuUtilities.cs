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
        ChecksAndMods.ModulatorPersonalityAxeScore[] relevantModulatorSOs = Resources.LoadAll<ChecksAndMods.ModulatorPersonalityAxeScore>(Constants_PathResources.SO_PATH_MODULATORS_PERSONALITYAXES);

        List<Quille.PersonalityAxeSO> allPersonalityAxeSOs = Resources.LoadAll<Quille.PersonalityAxeSO>(Constants_PathResources.SO_PATH_PERSONALITYAXES).ToList();
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

            string savePath = AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/Resources/{0}Modulator_PersonalityAxe_{1}.asset", Constants_PathResources.SO_PATH_MODULATORS_PERSONALITYAXES, personalityAxe.ItemName.StripComplexChars()));

            AssetDatabase.CreateAsset(newModulatorSO, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("A new instance of ModulatorPersonalityAxeScore was created for the '{0}' PersonalityAxe.", personalityAxe.ItemName));
        }
    }


    [MenuItem("Quille/Person/ChecksAndMods/Create missing PersonalityTrait modulators.")]
    static void CreatePersonalityTraitModulators()
    {
        // Load relevant resources.
        ChecksAndMods.ModulatorPersonalityTraitScore[] relevantModulatorSOs = Resources.LoadAll<ChecksAndMods.ModulatorPersonalityTraitScore>(Constants_PathResources.SO_PATH_MODULATORS_PERSONALITYTRAITS);

        List<Quille.PersonalityTraitSO> allPersonalityTraitSOs = Resources.LoadAll<Quille.PersonalityTraitSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITS).ToList();
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

            string savePath = AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/Resources/{0}Modulator_PersonalityTrait_{1}.asset", Constants_PathResources.SO_PATH_MODULATORS_PERSONALITYTRAITS, personalityTrait.ItemName.StripComplexChars()));

            AssetDatabase.CreateAsset(newModulatorSO, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("A new instance of ModulatorPersonalityTraitScore was created for the '{0}' PersonalityTrait.", personalityTrait.ItemName));
        }
    }


    [MenuItem("Quille/Person/ChecksAndMods/Create missing Drive modulators.")]
    static void CreateDriveModulators()
    {
        // Load relevant resources.
        ChecksAndMods.ModulatorDriveScore[] relevantModulatorSOs = Resources.LoadAll<ChecksAndMods.ModulatorDriveScore>(Constants_PathResources.SO_PATH_MODULATORS_DRIVES);

        List<Quille.DriveSO> allDriveSOs = Resources.LoadAll<Quille.DriveSO>(Constants_PathResources.SO_PATH_DRIVES).ToList();
        List<Quille.DriveSO> coveredDriveSOs = new List<Quille.DriveSO>();

        // Populate the list of DriveSOs for which modulators already exist.
        foreach (ChecksAndMods.ModulatorDriveScore modulator in relevantModulatorSOs)
        {
            coveredDriveSOs.Add(modulator.RelevantDrive);
        }

        // Find the difference between the two lists.
        List<Quille.DriveSO> newDriveSOs = allDriveSOs.Except(coveredDriveSOs).ToList();

        //Create new modSOs for the remaining Drives.
        foreach (Quille.DriveSO Drive in newDriveSOs)
        {
            ChecksAndMods.ModulatorDriveScore newModulatorSO = ScriptableObject.CreateInstance<ChecksAndMods.ModulatorDriveScore>();
            newModulatorSO.RelevantDrive = Drive;

            string savePath = AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/Resources/{0}Modulator_Drive_{1}.asset", Constants_PathResources.SO_PATH_MODULATORS_DRIVES, Drive.ItemName.StripComplexChars()));

            AssetDatabase.CreateAsset(newModulatorSO, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("A new instance of ModulatorDriveScore was created for the '{0}' Drive.", Drive.ItemName));
        }
    }


    [MenuItem("Quille/Person/ChecksAndMods/Create missing Interest modulators.")]
    static void CreateInterestModulators()
    {
        // Load relevant resources.
        ChecksAndMods.ModulatorInterestScore[] relevantModulatorSOs = Resources.LoadAll<ChecksAndMods.ModulatorInterestScore>(Constants_PathResources.SO_PATH_MODULATORS_INTERESTS);

        List<Quille.InterestSO> allInterestSOs = Resources.LoadAll<Quille.InterestSO>(Constants_PathResources.SO_PATH_INTERESTS).ToList();
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

            string savePath = AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/Resources/{0}Modulator_Interest_{1}.asset", Constants_PathResources.SO_PATH_MODULATORS_INTERESTS, Interest.ItemName.StripComplexChars()));

            AssetDatabase.CreateAsset(newModulatorSO, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("A new instance of ModulatorInterestScore was created for the '{0}' Interest.", Interest.ItemName));
        }
    }
}
