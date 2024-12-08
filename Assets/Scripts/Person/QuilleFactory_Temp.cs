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
        [SerializeField] private QuilleUI.CCUI_PersonalityTraitsMenu sourcePersonalityTraitsMenu;
        [SerializeField] private QuilleUI.CCUI_DrivesMenu sourceDrivesMenu;
        [SerializeField] private QuilleUI.CCUI_InterestsMenu sourceInterestsMenu;


        [Header("Misc")]
        [SerializeField, TextAreaAttribute(50, 100)] private string tempJSON;


        // EVENTS
        public event QuilleUI.TargetPersonModified TargetPersonWasModified;



        // METHODS

        // UPDATE PERSON FROM UI VALUES
        public void UpdatePersonFromUI()
        {
            UpdatePersonNamesFromUI();

            UpdatePersonPersonalityAxesFromUI(false);
            UpdatePersonPersonalityTraitsFromUI(false);
            UpdatePersonDrivesFromUI(false);
            UpdatePersonInterestsFromUI(false);

            TargetPersonWasModified?.Invoke(currentPerson);
        }
        public void UpdatePersonNamesFromUI()
        {
            currentPerson.MyPersonCharacter.FirstName = sourceNamesMenu.InputFieldFirstName;
            currentPerson.MyPersonCharacter.LastName = sourceNamesMenu.InputFieldLastName;
            currentPerson.MyPersonCharacter.NickName = sourceNamesMenu.InputFieldNickname;
        }
        public void UpdatePersonPersonalityAxeFromUI(Quille.PersonalityAxeSO thePASO)
        {
            UpdatePersonPersonalityAxeFromUI(true, thePASO);
        }
        public void UpdatePersonPersonalityAxeFromUI(bool throwEvent, Quille.PersonalityAxeSO thePASO)
        {
            float sliderValue = sourcePersonalityAxesMenu.GetSliderValueFor(thePASO);
            currentPerson.MyPersonCharacter.SetAxeScore(thePASO, sliderValue);

            if (throwEvent)
            {
                TargetPersonWasModified?.Invoke(currentPerson);
            }
        }
        public void UpdatePersonPersonalityAxesFromUI()
        {
            UpdatePersonPersonalityAxesFromUI(true);
        }
        public void UpdatePersonPersonalityAxesFromUI(bool throwEvent)
        {
            currentPerson.MyPersonCharacter.SetAxeScoreDict(sourcePersonalityAxesMenu.GetSlidersSOsAndValues());

            if (throwEvent)
            {
                TargetPersonWasModified?.Invoke(currentPerson);
            }
        }
        public void UpdatePersonPersonalityTraitsFromUI()
        {
            UpdatePersonPersonalityTraitsFromUI(true);
        }
        public void UpdatePersonPersonalityTraitsFromUI(bool throwEvent)
        {
            currentPerson.MyPersonCharacter.SetTraitScoreDict(sourcePersonalityTraitsMenu.GetButtonsSOsAndValues());

            if (throwEvent)
            {
                TargetPersonWasModified?.Invoke(currentPerson);
            }
        }
        public void UpdatePersonDrivesFromUI()
        {
            UpdatePersonDrivesFromUI(true);
        }
        public void UpdatePersonDrivesFromUI(bool throwEvent)
        {
            currentPerson.MyPersonCharacter.SetDriveScoreDict(sourceDrivesMenu.GetButtonsSOsAndValues());

            if (throwEvent)
            {
                TargetPersonWasModified?.Invoke(currentPerson);
            }
        }
        public void UpdatePersonInterestsFromUI()
        {
            UpdatePersonInterestsFromUI(true);
        }
        public void UpdatePersonInterestsFromUI(bool throwEvent)
        {
            currentPerson.MyPersonCharacter.SetInterestScoreDict(sourceInterestsMenu.GetButtonsSOsAndValues());

            if (throwEvent)
            {
                TargetPersonWasModified?.Invoke(currentPerson);
            }
        }


        // UPDATE UI VALUES FROM PERSON
        private void UpdateUIFromPerson()
        {
            UpdateUINamesFromPerson();
            UpdateUIPersonalityAxesFromPerson();
            UpdateUIPersonalityTraitsFromPerson();
            UpdateUIDrivesFromPerson();
            UpdateUIInterestsFromPerson();

            TargetPersonWasModified?.Invoke(currentPerson);
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

        private void UpdateUIPersonalityTraitsFromPerson()
        {
            sourcePersonalityTraitsMenu.SetButtonValuesFromSOFloatDict(currentPerson.MyPersonCharacter.GetTraitScoreDict());
        }

        private void UpdateUIDrivesFromPerson()
        {
            sourceDrivesMenu.SetButtonValuesFromSOFloatDict(currentPerson.MyPersonCharacter.GetDriveScoreDict());
        }

        private void UpdateUIInterestsFromPerson()
        {
            sourceInterestsMenu.SetButtonValuesFromSOFloatDict(currentPerson.MyPersonCharacter.GetInterestScoreDict());
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

            sourcePersonalityTraitsMenu.PersonalityTraitsMenuUpdated += UpdatePersonPersonalityTraitsFromUI;

            sourceDrivesMenu.DrivesMenuUpdated += UpdatePersonDrivesFromUI;

            sourceInterestsMenu.InterestsMenuUpdated += UpdatePersonInterestsFromUI;

            TargetPersonWasModified += sourcePersonalityTraitsMenu.OnTargetPersonModified;
        }


        // BUILT IN
        void Start()
        {
            Init();

            CreateNewCharacter();
        }
    }
}

