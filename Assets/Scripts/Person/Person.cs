using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Quille
{
    [System.Serializable]
    [RequireComponent(typeof(Person_NeedController)), RequireComponent(typeof(Person_AI))] // Add more as they are needed.
    public class Person : MonoBehaviour
    {
        // VARIABLES

        // Generic information.
        [SerializeField] private int charID;
        [SerializeField] private string firstName, lastName, nickName;
        [SerializeField] private List<string> secondaryNames; // In case of multiple middle names :p not sure it'll actually be useful.

        // Age and gender?

        // Controllers and other compotents.
        [SerializeField] private Person_Character myPersonalityController;
        [SerializeField] private Person_NeedController myNeedController;
        [SerializeField] private Person_AI myPersonAI;



        // PROPERTIES & GETTERS/SETTERS
        public int CharID { get { return charID; } }

        public string FirstName { get { return firstName; } set { firstName = value; } }
        public string LastName { get { return lastName; } set { lastName = value; } }
        public string NickName { get { return nickName; } set { nickName = value; } }
        public List<string> SecondaryNames { get { return secondaryNames; } set { secondaryNames = value; } } // Not sure about the best way to handle this one :/
        // Check the existance of a secondary name.
        // Add secondary name.
        // Remove secondary name.
        public string FirstAndLastName { get { return string.Format("{0} {1}", firstName, lastName); } } // Name order?
        public string FirstNickAndLastName { get { return string.Format("{0} '{1}' {2}", firstName, nickName, lastName); } }
        public string FullName { get { return string.Format("{0} {1} {2}, '{3}'", firstName, string.Join(" ", secondaryNames), lastName, nickName); } }


        // Controllers.
        public Person_Character MyPersonalityController { get { return myPersonalityController; } }
        public Person_NeedController MyNeedController { get { return myNeedController; } }
        // Should we be able to return the AI one?



        // CONSTRUCTORS
        //public Person()
        //{

        //}


        private void PersonPopulator(Person_Character personalityController)
        {
            myPersonalityController = personalityController;
        }



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
            if (Input.GetKeyDown(KeyCode.B))
            {
                myNeedController.ModulateBasicNeeds(this);
                myNeedController.ModulateSubjectiveNeeds(this);
            }
        }
    }
}
