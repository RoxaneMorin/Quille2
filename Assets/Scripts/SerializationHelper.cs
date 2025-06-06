using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quille;
using World;

// TODO: should we use a namespace?

public static class SerializationHelper
{
    // Static class containing methods for writing and manipulation the actual JSON save files.

    // TODO: verify how a computer choses whether to use / or \.

    // METHODS

    // PATH CONSTRUCTORS
    private static string ReturSavePath()
    {
        // TODO: ability to change the default paths.
        return string.Format("{0}{1}", Constants_Serialization.DEFAULT_GAME_SAVE_LOCATION_PATH, Constants_Serialization.DEFAULT_GAME_SAVE_FOLDER_NAME);
    }

    private static string ReturnBackpackSavePath()
    {
        return string.Format("{0}\\{1}", ReturSavePath(), Constants_Serialization.DEFAULT_BACKPACK_SAVE_FOLDER_NAME);
    }
    private static string ReturnBackpackCharacterSavePath()
    {
        return string.Format("{0}\\{1}", ReturnBackpackSavePath(), Constants_Serialization.DEFAULT_CHARACTER_SAVE_FOLDER_NAME);
    }

    private static string ReturnCurrentWorldSavePath()
    {
        return string.Format("{0}\\{1}\\{2}", ReturSavePath(), Constants_Serialization.DEFAULT_WORLD_SAVE_FOLDER_NAME, WorldData.GetCurrentWorldData().SafeWorldName);
    }
    private static string ReturnCurrentCharacterSavePath()
    {
        return string.Format("{0}\\{1}", ReturnCurrentWorldSavePath(), Constants_Serialization.DEFAULT_CHARACTER_SAVE_FOLDER_NAME);
    }

    private static string ReturnBackUpFilePath(string sourceFilePath)
    {
        return sourceFilePath + Constants_Serialization.SUFFIX_BACKUP;
    }


    //



    // CHARACTERS
    public static string[] ReturnAllCharacterFilePathsAt(SaveType location = SaveType.CurrentWorld, bool includeBackUps = false)
    {
        return location == SaveType.CurrentWorld ? ReturnAllCurrentCharacterFilePaths(includeBackUps) : ReturnAllBackpackCharacterFilePaths(includeBackUps);
    }
    public static string[] ReturnAllCurrentCharacterFilePaths(bool includeBackUps = false)
    {
        string[] allCharacterFilesFound = includeBackUps ? Directory.GetFiles(ReturnCurrentCharacterSavePath()) : Directory.GetFiles(ReturnCurrentCharacterSavePath(),  "*" + Constants_Serialization.SUFFIX_JSON);
        return allCharacterFilesFound;
    }
    public static string[] ReturnAllBackpackCharacterFilePaths(bool includeBackUps = false)
    {
        string[] allCharacterFilesFound = includeBackUps ? Directory.GetFiles(ReturnBackpackCharacterSavePath()) : Directory.GetFiles(ReturnBackpackCharacterSavePath(), "*" + Constants_Serialization.SUFFIX_JSON);

        return allCharacterFilesFound;
    }

    public static void SaveJSONCharacterToFile(Person theCharacter, string fileName, string formatedJSON, SaveType saveTo = SaveType.CurrentWorld)
    {
        try
        {
            string folderPath = saveTo == SaveType.CurrentWorld ? ReturnCurrentCharacterSavePath() : ReturnBackpackCharacterSavePath();
            string filePath = String.Format("{0}\\{1}", folderPath, fileName);

            // Create the save directory if it doesn't already exist.
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            // If a previous save file exists, turn it into a back up.
            if (File.Exists(filePath))
            {
                string backupFilePath = ReturnBackUpFilePath(filePath);

                System.IO.File.Delete(backupFilePath);
                System.IO.File.Move(filePath, backupFilePath);
            }

            // Save the new data proper.
            var file = File.CreateText(filePath);
            file.Write(formatedJSON);
            file.Close();

            //Debug.Log(string.Format("Successfully saved {0}, to {1}.", theCharacter.CharIDAndCharacterName, fileName));
            Debug.Log(string.Format("Successfully saved {0}, at path '{1}'.", theCharacter.CharIDAndCharacterName, filePath));
        }
        catch (Exception theError)
        {
            Debug.LogError(string.Format("Failed to write JSON data for {0}, to file. This character will not be saved.\n\nException text:\n{1}", theCharacter.CharIDAndCharacterName, theError.ToString()));
        }
    }

    public static string LoadJSONCharacterFromFile(string filePath, bool tryBackUpOnError = true)
    {
        string JSONContent = null;

        try
        {
            JSONContent = File.ReadAllText(filePath);
            //Debug.Log(string.Format("Successfully read the character file at path '{0}'.", filePath));
        }
        catch (Exception theError)
        {
            Debug.Log(string.Format("Failed to find, open or read the character file at path '{0}'.\n\nException text:\n{1}", filePath, theError.ToString()));

            // Verify if a backUp exist, and (potentially?) try to load it instead.
            if (tryBackUpOnError)
            {
                string backupFilePath = ReturnBackUpFilePath(filePath);
                if (File.Exists(backupFilePath))
                {
                    // TODO: give the player a pop up option whether to do so.
                    Debug.Log(string.Format("A backup exists for the file at path '{0}'. It could be (re)loaded instead.", filePath));
                    JSONContent = LoadJSONCharacterFromFile(backupFilePath, false);
                }
            }
        }

        return JSONContent;
    }
}
