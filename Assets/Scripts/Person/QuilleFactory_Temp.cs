using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class QuilleFactory_Temp : MonoBehaviour
    {
        // A tentative sketch of the character creator's factory script.


        // VARIABLES
        [SerializeField] private Person currentPerson;

        [Header("References")]
        [SerializeField] private QuilleUI.CCUI_NamesMenu sourceNamesMenu;
        [SerializeField] private QuilleUI.CCUI_PersonalityAxesMenu sourcePersonalityAxesMenu;


        [Header("Misc")]
        [SerializeField, TextAreaAttribute(50, 100)] private string tempJSON;



        // METHODS

        // UPDATE PERSON FROM UI VALUES
        public void UpdatePersonFromUI()
        {
            UpdatePersonNamesFromUI();
            UpdatePersonPersonalityAxesFromUI();
        }
        public void UpdatePersonNamesFromUI()
        {
            currentPerson.MyPersonCharacter.FirstName = sourceNamesMenu.InputFieldFirstName;
            currentPerson.MyPersonCharacter.LastName = sourceNamesMenu.InputFieldLastName;
            currentPerson.MyPersonCharacter.NickName = sourceNamesMenu.InputFieldNickname;
        }
        public void UpdatePersonPersonalityAxeFromUI(Quille.PersonalityAxeSO thePASO)
        {
            float sliderValue = sourcePersonalityAxesMenu.GetSliderValueFor(thePASO);
            currentPerson.MyPersonCharacter.SetAxeScore(thePASO, sliderValue);
        }
        public void UpdatePersonPersonalityAxesFromUI()
        {
            currentPerson.MyPersonCharacter.SetAxeScoreDict(sourcePersonalityAxesMenu.SetSlidersSOsAndValues());
        }


        // UPDATE UI VALUES FROM PERSON
        private void UpdateUIFromPerson()
        {
            UpdateUINamesFromPerson();
            UpdateUIPersonalityAxesFromPerson();
        }

        private void UpdateUINamesFromPerson()
        {
            sourceNamesMenu.InputFieldFirstName = currentPerson.MyPersonCharacter.FirstName;
            sourceNamesMenu.InputFieldLastName = currentPerson.MyPersonCharacter.LastName;
            sourceNamesMenu.InputFieldNickname = currentPerson.MyPersonCharacter.NickName;
        }

        private void UpdateUIPersonalityAxesFromPerson()
        {
            sourcePersonalityAxesMenu.SetSliderValuesFromSOFloatDict(currentPerson.MyPersonCharacter.GetAxeScoreDict());
        }


        // UI ACTIONS
        public void CreateNewCharacter()
        {
            currentPerson.ResetMe();
            // TODO: generate and assign a new charID.

            UpdateUIFromPerson();
        }

        public void SaveCharacter()
        {
            tempJSON = currentPerson.SaveToJSON();
        }

        public void LoadCharacter()
        {
            currentPerson.LoadFromJSON(tempJSON);
            UpdateUIFromPerson();
        }


        // INIT
        private void Init()
        {
            // Subscribe to menu events.
            sourceNamesMenu.NameMenuUpdated += UpdatePersonNamesFromUI;

            sourcePersonalityAxesMenu.PersonalityAxeSliderUpdated += UpdatePersonPersonalityAxeFromUI;
            sourcePersonalityAxesMenu.PersonalityAxesMenuUpdated += UpdatePersonPersonalityAxesFromUI;
        }


        // BUILT IN
        void Start()
        {
            Init();

            CreateNewCharacter();
        }
    }
}

