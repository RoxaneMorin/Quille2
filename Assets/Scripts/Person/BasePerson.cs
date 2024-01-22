using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // Add more as they are needed.
    [RequireComponent(typeof(PersonalityController)), RequireComponent(typeof(NeedController)), RequireComponent(typeof(PersonAI))]
    public class BasePerson : MonoBehaviour
    {
        // VARIABLES

        // Generic information.
        [SerializeField] private int charID;
        [SerializeField] private string firstName, lastName, nickName;
        [SerializeField] private List<string> secondaryNames; // In case of multiple middle names :p not sure it'll actually be useful.

        // Controllers and other compotents.
        [SerializeField] private PersonalityController myPersonalityController;
        [SerializeField] private NeedController myNeedController;
        [SerializeField] private PersonAI myPersonAI;



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
        public PersonalityController MyPersonalityController { get { return myPersonalityController; } }
        public NeedController MyNeedController { get { return myNeedController; } }
        // Should we be able to return the AI one?



        // METHODS

        // Init.
        private void Init()
        {
            // Fetch our various components.
            myPersonalityController = GetComponent<PersonalityController>();
            myNeedController = GetComponent<NeedController>();
            myPersonAI = GetComponent<PersonAI>();
        }


        // Built in.

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                myNeedController.ModulateSubjectiveNeeds(this);
            }
        }
    }
}
