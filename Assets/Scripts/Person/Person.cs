using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using World;

namespace Quille
{
    // The root monoBehaviour for human characters.
    // Not JSON serialized in itself, but handles the serialization of its data holders and controllers.


    [System.Serializable]
    [RequireComponent(typeof(Person_NeedController)), RequireComponent(typeof(Person_AI))] // Add more as they are needed.
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
       public int CharID { get { return charID; } internal set { charID = value; } } // TODO: make sure the Setter is safe.
       public string CharIDAndCharacterName { get { return string.Format("Character #{0}, {1}", charID, myPersonCharacter.FirstNickAndLastName); } }

        // Data holders.
        public Person_Character MyPersonCharacter { get { return myPersonCharacter; } }

         // Controllers.
        public Person_NeedController MyNeedController { get { return myNeedController; } }
        // Should we be able to return the AI one?



        // METHODS

        // UTILITY
        internal void ResetMe()
        {
            //TODO: move this somewhere safer?

            charID = 0; // TODO: better ID management.

            myPersonCharacter = new Person_Character();

            FetchComponents();
            myNeedController.Init();
            myNeedController.enabled = false;
            myPersonAI.Init();
            myPersonAI.enabled = false;
        }


        // SAVE & LOAD
        // TODO: move this to its on partial segment?
        // Or else, to the serialization helper?
        public void SaveToJSON(SaveType saveTo = SaveType.CurrentWorld)
        {
            // TODO: save characters without IDs in a non-world folder?

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
                Debug.LogError(string.Format("Failed to convert the game data of {0}, to JSON. This character will not be saved.\n\nException text:\n{1}", CharIDAndCharacterName, theError.ToString()));
                return;
            }

            // Testing: save the sting to a variable.
            string formatedJSON = jsonPerson.ToString(Formatting.Indented);

            // Create and write the save file.
            string fileName = CreateJSONFileNameForSaveType(saveTo);
            SerializationHelper.SaveJSONCharacterToFile(this, fileName, formatedJSON, saveTo);
        }

        public void LoadFromJSON(string sourceFilePath)
        {
            string sourceJSON = SerializationHelper.LoadJSONCharacterFromFile(sourceFilePath);

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
                Debug.LogError(string.Format("Failed to parse JSON data at {0}. This character will not be loaded.\n\nException text:\n{1}", sourceFilePath, theError.ToString()));
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
                Debug.LogError(string.Format("Failed to rebuild game data from the JSON data at {0}. This character will not be loaded.\n\nException text:\n{1}", sourceFilePath, theError.ToString()));
                return;
            }

            Debug.Log(string.Format("Successfully (re)loaded {0}, from {1}.", CharIDAndCharacterName, sourceFilePath));
        }

        private string CreateJSONFileNameFromID()
        {
            return string.Format("CharID_{0}{1}", charID, Constants_Serialization.SUFFIX_JSON);
        }
        private string CreateJSONFileNameFromName()
        {
            string safeCharacterName = MyPersonCharacter.FirstNickAndLastName.StripComplexChars();
            return string.Format("{0}_{1}{2}", DateTime.Now.ToString("yyMMddHHmmss"), safeCharacterName, Constants_Serialization.SUFFIX_JSON);
        }
        private string CreateJSONFileNameForSaveType(SaveType saveType)
        {
            return saveType == SaveType.CurrentWorld ? CreateJSONFileNameFromID() : CreateJSONFileNameFromName();
        }


        // INIT
        private void Init()
        {
            // Create a new Person_Character.
            // myPersonCharacter = new Person_Character();

            // Fetch our various components.
            FetchComponents();
        }

        private void FetchComponents()
        {
            myNeedController = GetComponent<Person_NeedController>();
            myPersonAI = GetComponent<Person_AI>();
        }


        // BUILT IN

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            // Test stuff.
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    TempJsonString = SaveToJSON();
            //}

            //if (Input.GetKeyDown(KeyCode.Z))
            //{
            //    LoadFromJSON(TempJsonString);
            //}
        }

    }
}
