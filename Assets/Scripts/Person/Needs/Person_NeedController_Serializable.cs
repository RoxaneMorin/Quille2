using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public partial class Person_NeedController : MonoBehaviour
    {
        [System.Serializable]
        public class NeedController_Data
        {
            // VARIABLES
            [SerializeField] private BasicNeed[] myBasicNeeds;
            [SerializeField] private SubjectiveNeed[] mySubjectiveNeeds;



            // PROPERTIES & GETTERS/SETTERS
            public BasicNeed[] MyBasicNeeds
            {
                get { return myBasicNeeds; }
                set { myBasicNeeds = value; }
            }
            public SubjectiveNeed[] MySubjectiveNeeds
            {
                get { return mySubjectiveNeeds; }
                set { mySubjectiveNeeds = value; }
            }



            // CONSTRUCTOR
        }
    }
}