using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;

namespace Quille
{
    public class PersonFactory_TempAxesOnly : MonoBehaviour
    {
        // TODO: an actual Person factory divorced from the one personality axe menu.

        // VARIABLES

        // UI Sources
        [SerializeField] private QuilleUI.CCUI_PersonalityAxesMenu sourcePersonalityAxesMenu;

        // Quille parts
        [SerializeField] private Person_Character personCharacter;

        [SerializeField, TextAreaAttribute(1, 100)] private string jsonString;


        // METHODS

        // CHARACTER HANDLING
        public void CreateCharacter()
        {
            SerializedDictionary<Quille.PersonalityAxeSO, float> axeScoresFromMenu = sourcePersonalityAxesMenu.SetSlidersSOsAndValues();

            personCharacter = new Person_Character(myPersonalityAxes: axeScoresFromMenu);
        }

        public void SaveCharacter()
        {
            CreateCharacter();

            jsonString = JsonConvert.SerializeObject(personCharacter, Formatting.Indented);
            Debug.Log(jsonString);

            // TODO: actually write to file.
        }

        public void LoadCharacter()
        {
            // TODO: actually read from file.

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                try
                {
                    personCharacter = JsonConvert.DeserializeObject<Person_Character>(jsonString);
                    sourcePersonalityAxesMenu.SetSliderValuesFromSOFloatDict(personCharacter.GetAxeScoreDict());
                }
                catch (Exception error)
                {
                    Debug.LogError(string.Format("An error occured trying to deserialize or load a Person_Character:\n{0}", error.ToString()));
                }
            }
        }


        // INIT
        private void FetchComponents()
        {
            sourcePersonalityAxesMenu = GetComponentInChildren<QuilleUI.CCUI_PersonalityAxesMenu>(); 
        }

        // BUILT IN
        void Start()
        {
            FetchComponents();
        }
    }
}

