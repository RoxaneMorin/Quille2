using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace QuilleUI
{
    public class CCUI_InterestsMenu : CCUI_GenericSelectableButtonMenu
    {
        // Test setup for a character creator's interest UI.
        // Procedurally populated from existing InterestSO.
        // Sadly repeats a lot from the GenericSteppedButtonMenu without being able to inherit from it :/ maybe I should rework that stuff.


        // PROPERTIES
        public float GetSliderValueFor(Quille.InterestSO theISO)
        {
            return ((CCUI_InterestButton)theButtonsDict[theISO]).MySliderValue;
        }

        public SerializedDictionary<Quille.InterestSO, float> GetButtonsSOsAndValues()
        {
            SerializedDictionary<Quille.InterestSO, float> SOsAndValuesDict = new SerializedDictionary<Quille.InterestSO, float>();

            foreach (CCUI_InterestButton button in currentlySelectedButtons)
            {
                SOsAndValuesDict.Add(button.MyInterestSO, button.MySliderValue);
            }

            return SOsAndValuesDict;
        }

        public void SetButtonValuesFromSOFloatDict(SerializedDictionary<Quille.InterestSO, float> sourceDict)
        {
            ResetValues();

            foreach (KeyValuePair<Quille.InterestSO, float> keyValuePair in sourceDict)
            {
                if (theButtonsDict.ContainsKey(keyValuePair.Key))
                {
                    ((CCUI_InterestButton)theButtonsDict[keyValuePair.Key]).Select(keyValuePair.Value);
                    currentlySelectedButtons.Add(theButtonsDict[keyValuePair.Key]);
                }
            }

            PositionSelectedButtons();
        }


        // EVENTS
        public event InterestsMenuUpdate InterestsMenuUpdated;



        // METHODS

        // EVENT LISTENERS
        public override void OnSelectableButtonUpdated(CCUI_GenericSelectableButton theUpdatedButton, bool shouldItMove)
        {
            base.OnSelectableButtonUpdated(theUpdatedButton, shouldItMove);

            InterestsMenuUpdated?.Invoke();
        }


        // UTILITY
        public void RandomizeValues()
        {
            CCUI_GenericSelectableButton[] permittedButtons = theButtons.Where(button => !button.IsForbidden).ToArray();
            int numberOfButtons = permittedButtons.Length;

            List<int> IDsToSelect = base.RandomizeValues(numberOfButtons, Quille.Constants.DEFAULT_INTEREST_COUNT);

            foreach (int ID in IDsToSelect)
            {
                currentlySelectedButtons.Add(theButtons[ID]);
                ((CCUI_InterestButton)theButtons[ID]).RandomizeValueAndSelect();
            }

            PositionSelectedButtons();

            InterestsMenuUpdated?.Invoke();
        }


        // INIT
        protected override void Init()
        {
            base.Init();
            LoadSOsAndCreateButtons(PathConstants.SO_PATH_INTERESTS);
        }


        // BUILT IN
        void Start()
        {
            Init();
        }
    }
}

