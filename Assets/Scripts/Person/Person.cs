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
        private string CreateJSONFileName()
        {
            return string.Format("CharID_{0}.json", charID);
        }

        private string SaveToJSON()
        {
            // TODO: what happens when bits of the character are missing?
            string jsonPersonCharacter = myPersonCharacter.SaveToJSON();
            string jsonNeedController = myNeedController.SaveToJSON();

            JObject jsonPerson = new JObject();

            jsonPerson.Add("charID", charID);
            jsonPerson.Add("PersonCharacter", JObject.Parse(jsonPersonCharacter));
            jsonPerson.Add("NeedController", JObject.Parse(jsonNeedController));

            string formatedJSON = jsonPerson.ToString(Formatting.Indented);
            //Debug.Log(formatedJSON);

            // TODO: set proper, potentially customisable save location
            // TODO: create and rely on a ressource that track world info, character IDs, etc.
            string fileName = Constants.DEFAULT_CHARACTER_SAVE_LOCATION + CreateJSONFileName();

            // TODO: add a try block just to be safe.
            // TODO: make a backup of the previous save file if one exists.
            // TODO: handle what happens when a character is renamed.
            var file = File.CreateText(fileName) ;
            file.Write(formatedJSON);
            file.Close();

            Debug.Log("Sucessfully saved to JSON.");

            return formatedJSON;
        }

        private void LoadFromJSON(string sourceJSON)
        {
            // TODO: chose what file to load.

            JObject jsonPerson = JObject.Parse(sourceJSON);

            string jsonCharID = jsonPerson.GetValue("charID").ToString();
            string jsonPersonCharacter = jsonPerson.GetValue("PersonCharacter").ToString(Formatting.Indented);
            string jsonNeedController = jsonPerson.GetValue("NeedController").ToString(Formatting.Indented);


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

            Debug.Log("Sucessfully loaded from JSON.");
        }
    }
}
