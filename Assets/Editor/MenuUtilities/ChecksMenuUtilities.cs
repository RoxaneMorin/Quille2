using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ChecksMenuUtilities : MonoBehaviour
{
    // Editor utilities for the creation and management of CheckSO instances.


    // METHODS

    // Menu methods.
    [MenuItem("Quille/Person/ChecksAndMods/Create missing PersonalityAxe checks.")]
    static void CreatePersonalityAxeChecks()
    {
        // Load relevant resources.
        ChecksAndMods.CheckPersonalityAxeScoreSO[] relevantChecksSOs = Resources.LoadAll<ChecksAndMods.CheckPersonalityAxeScoreSO>(Constants_PathResources.SO_PATH_CHECKS_PERSONALITYAXES);

        List<Quille.PersonalityAxeSO> allPersonalityAxeSOs = Resources.LoadAll<Quille.PersonalityAxeSO>(Constants_PathResources.SO_PATH_PERSONALITYAXES).ToList();
        List<Quille.PersonalityAxeSO> coveredPersonalityAxeSOs = new List<Quille.PersonalityAxeSO>();

        // Populate the list of PersonalityAxeSOs for which checks already exist.
        foreach (ChecksAndMods.CheckPersonalityAxeScoreSO check in relevantChecksSOs)
        {
            coveredPersonalityAxeSOs.Add(check.RelevantPersonalityAxe);
        }

        // Find the difference between the two lists.
        List<Quille.PersonalityAxeSO> newPersonalityAxeSOs = allPersonalityAxeSOs.Except(coveredPersonalityAxeSOs).ToList();

        //Create new checkSOs for the remaining personalityAxes.
        foreach (Quille.PersonalityAxeSO personalityAxe in newPersonalityAxeSOs)
        {
            ChecksAndMods.CheckPersonalityAxeScoreSO newCheckSO = ScriptableObject.CreateInstance<ChecksAndMods.CheckPersonalityAxeScoreSO>();
            newCheckSO.RelevantPersonalityAxe = personalityAxe;

            string savePath = AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/Resources/{0}Check_PersonalityAxe_{1}.asset", Constants_PathResources.SO_PATH_CHECKS_PERSONALITYAXES, personalityAxe.ItemName.StripComplexChars()));

            AssetDatabase.CreateAsset(newCheckSO, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("A new instance of CheckPersonalityAxeScore was created for the '{0}' PersonalityAxe.", personalityAxe.ItemName));
        }
    }


    [MenuItem("Quille/Person/ChecksAndMods/Create missing PersonalityTrait checks.")]
    static void CreatePersonalityTraitChecks()
    {
        // Load relevant resources.
        ChecksAndMods.CheckPersonalityTraitScoreSO[] relevantChecksSOs = Resources.LoadAll<ChecksAndMods.CheckPersonalityTraitScoreSO>(Constants_PathResources.SO_PATH_CHECKS_PERSONALITYTRAITS);

        List<Quille.PersonalityTraitSO> allPersonalityTraitSOs = Resources.LoadAll<Quille.PersonalityTraitSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITS).ToList();
        List<Quille.PersonalityTraitSO> coveredPersonalityTraitSOs = new List<Quille.PersonalityTraitSO>();

        // Populate the list of PersonalityTraitSOs for which checks already exist.
        foreach (ChecksAndMods.CheckPersonalityTraitScoreSO check in relevantChecksSOs)
        {
            coveredPersonalityTraitSOs.Add(check.RelevantPersonalityTrait);
        }

        // Find the difference between the two lists.
        List<Quille.PersonalityTraitSO> newPersonalityTraitSOs = allPersonalityTraitSOs.Except(coveredPersonalityTraitSOs).ToList();

        //Create new checkSOs for the remaining personalityTraits.
        foreach (Quille.PersonalityTraitSO personalityTrait in newPersonalityTraitSOs)
        {
            ChecksAndMods.CheckPersonalityTraitScoreSO newCheckSO = ScriptableObject.CreateInstance<ChecksAndMods.CheckPersonalityTraitScoreSO>();
            newCheckSO.RelevantPersonalityTrait = personalityTrait;

            string savePath = AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/Resources/{0}Check_PersonalityTrait_{1}.asset", Constants_PathResources.SO_PATH_CHECKS_PERSONALITYTRAITS, personalityTrait.ItemName.StripComplexChars()));

            AssetDatabase.CreateAsset(newCheckSO, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("A new instance of CheckPersonalityTraitScore was created for the '{0}' PersonalityTrait.", personalityTrait.ItemName));
        }
    }


    [MenuItem("Quille/Person/ChecksAndMods/Create missing Drive checks.")]
    static void CreateDriveChecks()
    {
        // Load relevant resources.
        ChecksAndMods.CheckDriveScoreSO[] relevantChecksSOs = Resources.LoadAll<ChecksAndMods.CheckDriveScoreSO>(Constants_PathResources.SO_PATH_CHECKS_DRIVES);

        List<Quille.DriveSO> allDriveSOs = Resources.LoadAll<Quille.DriveSO>(Constants_PathResources.SO_PATH_DRIVES).ToList();
        List<Quille.DriveSO> coveredDriveSOs = new List<Quille.DriveSO>();

        // Populate the list of DriveSOs for which checks already exist.
        foreach (ChecksAndMods.CheckDriveScoreSO check in relevantChecksSOs)
        {
            coveredDriveSOs.Add(check.RelevantDrive);
        }

        // Find the difference between the two lists.
        List<Quille.DriveSO> newDriveSOs = allDriveSOs.Except(coveredDriveSOs).ToList();

        //Create new checkSOs for the remaining Drives.
        foreach (Quille.DriveSO Drive in newDriveSOs)
        {
            ChecksAndMods.CheckDriveScoreSO newCheckSO = ScriptableObject.CreateInstance<ChecksAndMods.CheckDriveScoreSO>();
            newCheckSO.RelevantDrive = Drive;

            string savePath = AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/Resources/{0}Check_Drive_{1}.asset", Constants_PathResources.SO_PATH_CHECKS_DRIVES, Drive.ItemName.StripComplexChars()));

            AssetDatabase.CreateAsset(newCheckSO, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("A new instance of CheckDriveScore was created for the '{0}' Drive.", Drive.ItemName));
        }
    }


    [MenuItem("Quille/Person/ChecksAndMods/Create missing Interest checks.")]
    static void CreateInterestChecks()
    {
        // Load relevant resources.
        ChecksAndMods.CheckInterestScoreSO[] relevantChecksSOs = Resources.LoadAll<ChecksAndMods.CheckInterestScoreSO>(Constants_PathResources.SO_PATH_CHECKS_INTERESTS);

        List<Quille.InterestSO> allInterestSOs = Resources.LoadAll<Quille.InterestSO>(Constants_PathResources.SO_PATH_INTERESTS).ToList();
        List<Quille.InterestSO> coveredInterestSOs = new List<Quille.InterestSO>();

        // Populate the list of InterestSOs for which checks already exist.
        foreach (ChecksAndMods.CheckInterestScoreSO check in relevantChecksSOs)
        {
            coveredInterestSOs.Add(check.RelevantInterest);
        }

        // Find the difference between the two lists.
        List<Quille.InterestSO> newInterestSOs = allInterestSOs.Except(coveredInterestSOs).ToList();

        //Create new checkSOs for the remaining Interests.
        foreach (Quille.InterestSO interest in newInterestSOs)
        {
            ChecksAndMods.CheckInterestScoreSO newCheckSO = ScriptableObject.CreateInstance<ChecksAndMods.CheckInterestScoreSO>();
            newCheckSO.RelevantInterest = interest;

            string savePath = AssetDatabase.GenerateUniqueAssetPath(string.Format("Assets/Resources/{0}Check_Interest_{1}.asset", Constants_PathResources.SO_PATH_CHECKS_INTERESTS, interest.ItemName.StripComplexChars()));

            AssetDatabase.CreateAsset(newCheckSO, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("A new instance of CheckInterestScore was created for the '{0}' Interest.", interest.ItemName));
        }
    }
}
