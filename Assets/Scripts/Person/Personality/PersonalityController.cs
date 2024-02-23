using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Quille
{
    public class PersonalityController : MonoBehaviour
    {
        // Very likely to rewrite this class.


        // VARIABLES/PARAMS

        // Management
        [SerializeField]
        private string folderPath = "ScriptableObjects/Personality/Axes";
        private PersonalityAxeSO[] personalityAxes;

        // Personality scores.
        [SerializeField, SerializedDictionary("Personality Axe", "Score")]
        private SerializedDictionary<PersonalityAxeSO, float> myPersonalityAxes;
        // Should be private, but is not visible in editor when it is.

        [SerializeField, SerializedDictionary("Personality Trait", "Score")]
        private SerializedDictionary<PersonalityTraitSO, float> myPersonalityTraits;
        // Scores should be limited to 0.5 or 1.
        // A trait with a score of zero should be pruned away.





        // PROPERTIES
        // Personality traits.
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

        // Personality scores.
        public float GetScore(PersonalityTraitSO targetPersonalityTrait)
        {
            if (myPersonalityTraits.ContainsKey(targetPersonalityTrait))
            {
                return myPersonalityTraits[targetPersonalityTrait];
            }
            else
            {
                return 0;
            }
        }
        public void SetScore(PersonalityTraitSO targetPersonalityTrait, float value)
        {
            if (value < Constants.AXE_HALF_SPAN/2)
            {
                if (value <= 0)
                {
                    Debug.Log(string.Format("Scores for personality traits should not be null or negative. {0} will be rounded up to half intensity.", targetPersonalityTrait.name));
                }
                myPersonalityTraits[targetPersonalityTrait] = Constants.AXE_HALF_SPAN / 2;
            }
            else
            {
                myPersonalityTraits[targetPersonalityTrait] = Constants.AXE_HALF_SPAN;
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
