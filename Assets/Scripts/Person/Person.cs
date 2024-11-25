using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Quille
{
    // The root monoBehaviour for human characters.
    // Not JSON serialized.

    [System.Serializable]
    //[RequireComponent(typeof(Person_NeedController)), RequireComponent(typeof(Person_AI))] // Add more as they are needed.
    public class Person : MonoBehaviour
    {
        // VARIABLES

        // Data holders.
        [SerializeField] private Person_Character myPersonCharacter;

        // Controllers.
        [SerializeField] private Person_NeedController myNeedController;
        [SerializeField] private Person_AI myPersonAI;


        // PROPERTIES & GETTERS/SETTERS

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


        private void SerializeToJSON(string filePath)
        {
            string json = JsonUtility.ToJson(this, true);

            File.WriteAllText(filePath, json);

            Debug.Log(string.Format("Serialized at path {0}.", filePath));
        }



        // Built in.

        // Start is called before the first frame update
        void Start()
        {
            Init();

            //SerializeToJSON(Application.dataPath + "/personality_data.json");

            //string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
            //System.IO.File.WriteAllText(Application.dataPath + "/test_quille.json", jsonString);
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.B))
            //{
            //    myNeedController.ModulateBasicNeeds(this);
            //    myNeedController.ModulateSubjectiveNeeds(this);
            //}
        }
    }
}
