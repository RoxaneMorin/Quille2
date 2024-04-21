using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

using System.IO;

namespace Quille
{
    public class PersonalityController : MonoBehaviour
    {
        // Very likely to rewrite this class.


        // VARIABLES/PARAMS

        // Management
        // TODO: Move to a Factory script.
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


        // Interests and preferences.
        [SerializeField, SerializedDictionary("Interest", "Score")]
        private SerializedDictionary<InterestSO, float> myInterests;
        //Scores range between 1 (loved) and -1 (hated).
        // A score of zero means the character know of this interest but is indifferent to it.
        // Interests that do not appear in the dictionary are unknown.

        // Misc favourites
        // Colours, styles, etc.



        // PROPERTIES
        // Personality axe scores.
        public float GetAxeScore(PersonalityAxeSO targetPersonalityAxe)
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
        public void SetAxeScore(PersonalityAxeSO targetPersonalityAxe, float value)
        {
            if (myPersonalityAxes.ContainsKey(targetPersonalityAxe))
            {
                if (value < -Constants.PERSONALITY_HALF_SPAN)
                    myPersonalityAxes[targetPersonalityAxe] = -Constants.PERSONALITY_HALF_SPAN;
                else if (value > Constants.PERSONALITY_HALF_SPAN)
                    myPersonalityAxes[targetPersonalityAxe] = Constants.PERSONALITY_HALF_SPAN;
                else
                    myPersonalityAxes[targetPersonalityAxe] = value;
            }
        }

        // Personality trait scores.
        public float GetTraitScore(PersonalityTraitSO targetPersonalityTrait)
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
        public void SetTraitScore(PersonalityTraitSO targetPersonalityTrait, float value)
        {
            if (value < Constants.PERSONALITY_HALF_SPAN/2)
            {
                if (value <= 0)
                {
                    Debug.Log(string.Format("Scores for personality traits should not be null or negative. {0} will be rounded up to half intensity.", targetPersonalityTrait.name));
                    // TODO: simply ignore the value instead?
                }
                // TODO: display the warning message here?
                myPersonalityTraits[targetPersonalityTrait] = Constants.PERSONALITY_HALF_SPAN / 2;
            }
            else
            {
                myPersonalityTraits[targetPersonalityTrait] = Constants.PERSONALITY_HALF_SPAN;
            }
        }



        // METHODS

        // Init.
        // TODO: move this to a Quille Factory script.
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


        private void SerializeToJSON(string filePath)
        {
            string json = JsonUtility.ToJson(this);
            File.WriteAllText(filePath, json);
        }


        // Built in.

        // Start is called before the first frame update
        void Start()
        {
            LoadAndCreatePersonalityAxes();

            SerializeToJSON(Application.dataPath + "/personality_data.json");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
