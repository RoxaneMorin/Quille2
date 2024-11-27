using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Quille
{
    // The root monoBehaviour for human characters.
    // Not JSON serialized in itself, but handles the serialization of its data holders and controllers.


    [System.Serializable]
    //[RequireComponent(typeof(Person_NeedController)), RequireComponent(typeof(Person_AI))] // Add more as they are needed.
    public class Person : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] private int charID;
        // TODO: determine whether and how to use this unique numerical ID.

        // Data holders.
        [SerializeField] private Person_Character myPersonCharacter;

        // Controllers.
        [SerializeField] private Person_NeedController myNeedController;
        [SerializeField] private Person_AI myPersonAI;


        public string TempJsonString;


        // PROPERTIES & GETTERS/SETTERS
       public int CharID { get { return charID; } }

        // Data holders.
        public Person_Character MyPersonCharacter { get { return myPersonCharacter; } }

         // Controllers.
        public Person_NeedController MyNeedController { get { return myNeedController; } }
        // Should we be able to return the AI one?

        

        // CONSTRUCTORS
        //public Person()
        //{

        //}



        // METHODS

        // Init.
        private void Init()
        {
            // Fetch our various components.
            myNeedController = GetComponent<Person_NeedController>();
            myPersonAI = GetComponent<Person_AI>();
        }


        // BUILT IN.

        // Start is called before the first frame update
        void Start()
        {
            Init();

            
        }

        // Update is called once per frame
        void Update()
        {
            // Test stuff.
            if (Input.GetKeyDown(KeyCode.A))
            {
                TempJsonString = SaveToJSON();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                LoadFromJSON(TempJsonString);
            }
        }



        // SAVE & LOAD
        private string SaveToJSON()
        {
            // Init variables.
            JObject jsonPerson;
            string jsonPersonCharacter;
            string jsonNeedController;

            // Try to collect and convert this person's various components to JSON.
            try
            {
                jsonPersonCharacter = myPersonCharacter.SaveToJSON();
                jsonNeedController = myNeedController.SaveToJSON();

                jsonPerson = new JObject();

                jsonPerson.Add("charID", charID);
                jsonPerson.Add("PersonCharacter", JObject.Parse(jsonPersonCharacter));
                jsonPerson.Add("NeedController", JObject.Parse(jsonNeedController));
            }
            catch (Exception theError)
            {
                Debug.LogError(string.Format("Failed to convert the game data of {0}, to JSON. This character will not be saved.\n\nException text:\n{1}", GetCharacterNameAndCharID(), theError.ToString()));
                return null;
            }

            // Testing: save the sting to a variable.
            string formatedJSON = jsonPerson.ToString(Formatting.Indented);

            // Create and write the save file.
            try 
            {
                // TODO: set proper, potentially customisable save location
                // TODO: create and rely on a ressource that track world info, character IDs, etc.
                string fileName = CreateJSONFileName();
                string filePath = Constants.DEFAULT_CHARACTER_SAVE_LOCATION + fileName;

                // Create the save directory if it doesn't already exist.
                if (!System.IO.Directory.Exists(Constants.DEFAULT_CHARACTER_SAVE_LOCATION))
                {
                    System.IO.Directory.CreateDirectory(Constants.DEFAULT_CHARACTER_SAVE_LOCATION);
                }

                // If a previous save file exists, turn it into a back up.
                if (File.Exists(filePath))
                {
                    string backupFilePath = Constants.DEFAULT_CHARACTER_SAVE_LOCATION + CreateJSONBakFileName();

                    System.IO.File.Delete(backupFilePath);
                    System.IO.File.Move(filePath, backupFilePath);
                }

                // Save the new data proper.
                var file = File.CreateText(filePath);
                file.Write(formatedJSON);
                file.Close();

                Debug.Log(string.Format("Successfully saved {0}, to {1}.", GetCharacterNameAndCharID(), fileName));

                return formatedJSON;
            }
            catch (Exception theError)
            {
                Debug.LogError(string.Format("Failed to write JSON data for {0}, to file. This character will not be saved.\n\nException text:\n{1}", GetCharacterNameAndCharID(), theError.ToString()));
                return null;
            }
        }

        private void LoadFromJSON(string sourceJSON)
        {
            // TODO: chose what file to load. Likely will be done elsewhere/by the general save game loader.
            //string fileName = "placeholderFileName.json";
            string fileName = "CharID_0.json";

            //string[] allCharacterJSONs = Directory.GetFiles(Constants.DEFAULT_CHARACTER_SAVE_LOCATION);
            //foreach (string fileFound in allCharacterJSONs)
            //{
            //    Debug.Log(fileFound);
            //}
            // Directory.EnumerateFiles instead?

            // TODO: on exception, check if a backup exists and try to load that instead.
            //string backupFilePath = string.Format("{0}{1}.bak", Constants.DEFAULT_CHARACTER_SAVE_LOCATION, fileName);
            //if (File.Exists(backupFilePath))
            //{
            //    Debug.Log(string.Format("A backup exists for the file {0}. It could be (re)loaded instead. ", fileName));
            //    // TODO: give the player a pop up option whether to do so.
            //}

            // Init variables.
            JObject jsonPerson;
            string jsonCharID;
            string jsonPersonCharacter;
            string jsonNeedController;

            // Try to parse the JSON string.
            try
            {
                jsonPerson = JObject.Parse(sourceJSON);

                jsonCharID = jsonPerson.GetValue("charID").ToString();
                jsonPersonCharacter = jsonPerson.GetValue("PersonCharacter").ToString(Formatting.Indented);
                jsonNeedController = jsonPerson.GetValue("NeedController").ToString(Formatting.Indented);
            }
            catch(JsonException theError)
            {
                Debug.LogError(string.Format("Failed to parse JSON data from {0}. This character will not be loaded.\n\nException text:\n{1}", fileName, theError.ToString()));
                return;
            }

            // Try to reconstruct the character.
            try
            {
                // charID.
                charID = int.Parse(jsonCharID);

                // Person_Character.
                if (myPersonCharacter != null)
                {
                    myPersonCharacter.LoadFromJSON(jsonPersonCharacter);
                }
                else
                {
                    myPersonCharacter = Person_Character.CreateFromJSON(jsonPersonCharacter);
                }

                // Need_Controller
                if (myNeedController != null)
                {
                    myNeedController.LoadFromJSON(jsonNeedController);
                }
                else
                {
                    myNeedController = Person_NeedController.CreateFromJSON(jsonNeedController, gameObject);
                }
            }
            catch (Exception theError)
            {
                Debug.LogError(string.Format("Failed to rebuild game data from the JSON data of {0}. This character will not be loaded.\n\nException text:\n{1}", fileName, theError.ToString()));
                return;
            }

            Debug.Log(string.Format("Successfully (re)loaded {0}, from {1}.", GetCharacterNameAndCharID(), fileName));
        }

        private string CreateJSONFileName()
        {
            return string.Format("CharID_{0}.json", charID);
        }

        private string CreateJSONBakFileName()
        {
            return string.Format("CharID_{0}.json.bak", charID);
        }

        // TODO: make this into a true getter? Reformat/reword if necessary.
        private string GetCharacterNameAndCharID()
        {
            return string.Format("Character #{0}, {1}", charID, myPersonCharacter.FirstNickAndLastName);
        }
    }
}
