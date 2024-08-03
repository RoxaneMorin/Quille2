using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;

namespace Quille
{
    public class PersonFactory_Temp : MonoBehaviour
    {
        // TODO: an actual Person factory divorced from the one personality axe menu.

        // VARIABLES

        // UI Sources
        [SerializeField] private QuilleUI.PersonalityAxesMenu sourcePersonalityAxesMenu;

        // Quille parts
        [SerializeField] private Person_Character personalityController;

        [SerializeField, TextAreaAttribute(1, 100)] private string jsonString;


        // METHODS

        // CHARACTER HANDLING
        public void CreateCharacter()
        {
            SerializedDictionary<Quille.PersonalityAxeSO, float> axeScoresFromMenu = sourcePersonalityAxesMenu.ReturnAxeSOValueDict();

            personalityController = new Person_Character(axeScoresFromMenu);
        }

        public void SaveCharacter()
        {
            CreateCharacter();

            jsonString = JsonConvert.SerializeObject(personalityController, Formatting.Indented);
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
                    personalityController = JsonConvert.DeserializeObject<Person_Character>(jsonString);
                    sourcePersonalityAxesMenu.SetAxeSOValuePairs(personalityController.GetAxeScoreDict());
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
            sourcePersonalityAxesMenu = GetComponentInChildren<QuilleUI.PersonalityAxesMenu>(); 
        }

        // BUILT IN
        void Start()
        {
            FetchComponents();
        }
    }
}

