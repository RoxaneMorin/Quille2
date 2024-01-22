using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Quille
{
    public class PersonalityController : MonoBehaviour
    {
        // VARIABLES/PARAMS

        // Management
        [SerializeField]
        private string folderPath = "ScriptableObjects/Personality/Axes";
        private PersonalityAxeSO[] personalityAxes;

        [SerializeField, SerializedDictionary("Personality Axe", "Score")]
        private SerializedDictionary<PersonalityAxeSO, float> myPersonalityAxes;
        // Should be private, but is not visible in editor when it is.



        // PROPERTIES
        public float GetScore(PersonalityAxeSO targetPersonalityAxe)
        {
            if (myPersonalityAxes.ContainsKey(targetPersonalityAxe))
            {
                return myPersonalityAxes[targetPersonalityAxe];
            }
            else
            { 
                return 0;
            }
        }
        public void SetScore(PersonalityAxeSO targetPersonalityAxe, float value)
        {
            if (myPersonalityAxes.ContainsKey(targetPersonalityAxe))
            {
                if (value < -Constants.AXE_HALF_SPAN)
                    myPersonalityAxes[targetPersonalityAxe] = -Constants.AXE_HALF_SPAN;
                else if (value > Constants.AXE_HALF_SPAN)
                    myPersonalityAxes[targetPersonalityAxe] = Constants.AXE_HALF_SPAN;
                else
                    myPersonalityAxes[targetPersonalityAxe] = value;
            }
        }



        // METHODS

        // Init.
        private void LoadAndCreatePersonalityAxes()
        {
            personalityAxes = Resources.LoadAll<PersonalityAxeSO>(folderPath);

            foreach (PersonalityAxeSO personalityAxe in personalityAxes)
            {
                // Init them with random values for now.
                if (!myPersonalityAxes.ContainsKey(personalityAxe))
                {
                    myPersonalityAxes.Add(personalityAxe, Random.Range(-1f, 1f));
                }
            }
        }


        // Built in.

        // Start is called before the first frame update
        void Start()
        {
            LoadAndCreatePersonalityAxes();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
