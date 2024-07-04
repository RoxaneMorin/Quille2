using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Newtonsoft.Json;

using System.IO;

namespace Quille
{
    [System.Serializable]
    public class PersonalityController : MonoBehaviour
    {
        // Very likely to rewrite this class.

        // TODO: change into PersonIdentity or the like, a basic C# object.


        // VARIABLES/PARAMS

        // Management
        // TODO: Move to a Factory script.
        //[SerializeField]
        private string folderPath = "ScriptableObjects/Personality/Axes";
        private PersonalityAxeSO[] personalityAxes;


        // Personality scores.
        [SerializeField, SerializedDictionary("Personality Axe", "Score")]
        private SerializedDictionary<PersonalityAxeSO, float> myPersonalityAxes;

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
            if (value < -Constants.PERSONALITY_HALF_SPAN)
                myPersonalityAxes[targetPersonalityAxe] = -Constants.PERSONALITY_HALF_SPAN;
            else if (value > Constants.PERSONALITY_HALF_SPAN)
                myPersonalityAxes[targetPersonalityAxe] = Constants.PERSONALITY_HALF_SPAN;
            else
                myPersonalityAxes[targetPersonalityAxe] = value;
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

        // Interest scores.
        public float GetInterestScore(InterestSO targetInterestSO)
        {
            if (myInterests.ContainsKey(targetInterestSO))
            {
                return myInterests[targetInterestSO];
            }
            else
            {
                return 0; 
                // TODO: how to differentiate from unknown and known-but-neutral interests?
            }
        }
        public void SetInterestScore(InterestSO targetInterestSO, float value)
        {
            if (value < -Constants.INTEREST_HALF_SPAN)
                myInterests[targetInterestSO] = -Constants.PERSONALITY_HALF_SPAN;
            else if (value > Constants.INTEREST_HALF_SPAN)
                myInterests[targetInterestSO] = Constants.PERSONALITY_HALF_SPAN;
            else
                myInterests[targetInterestSO] = value;
        }



        // CONSTRUCTORS
        public PersonalityController()
        {
            myPersonalityAxes = new SerializedDictionary<PersonalityAxeSO, float>();
            myPersonalityTraits = new SerializedDictionary<PersonalityTraitSO, float>();
            myInterests = new SerializedDictionary<InterestSO, float>();
        }




        // METHODS

        // INIT
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

                    Debug.Log(personalityAxe.name);
                }
            }
        }



        // BUILT IN

        // Start is called before the first frame update
        void Start()
        {
            LoadAndCreatePersonalityAxes();

            //string jsonString = JsonUtility.ToJson(this, true);
            //System.IO.File.WriteAllText("test_quille_personality.json", jsonString);

            //string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //});
            //System.IO.File.WriteAllText(Application.dataPath + "/test_quille.json", jsonString);

        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
