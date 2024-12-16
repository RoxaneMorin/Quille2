using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Quille;

namespace QuilleUI
{
    public class CCUI_InterestsMenu : CCUI_GenericSelectableButtonMenu
    {
        // Test setup for a character creator's interest UI.
        // Procedurally populated from existing InterestSO.
        // Sadly repeats a lot from the GenericSteppedButtonMenu without being able to inherit from it :/ maybe I should rework that stuff.


        // PROPERTIES
        public float GetSliderValueFor(InterestSO theISO)
        {
            return ((CCUI_InterestButton)theButtonsDict[theISO]).MySliderValue;
        }

        public SerializedDictionary<InterestSO, float> GetButtonsSOsAndValues()
        {
            SerializedDictionary<InterestSO, float> SOsAndValuesDict = new SerializedDictionary<InterestSO, float>();

            foreach (CCUI_InterestButton button in currentlySelectedButtons)
            {
                SOsAndValuesDict.Add(button.MyInterestSO, button.MySliderValue);
            }

            return SOsAndValuesDict;
        }

        public void SetButtonValuesFromSOFloatDict(SerializedDictionary<InterestSO, float> sourceDict)
        {
            ResetValues();

            foreach (KeyValuePair<InterestSO, float> keyValuePair in sourceDict)
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
            ResetValues();

            CCUI_GenericSelectableButton[] permittedButtons = theButtons.Where(button => !button.IsForbidden && !button.IsExlcudedByCurrentFilter).ToArray();
            int numberOfButtons = permittedButtons.Length;

            List<int> IDsToSelect = base.RandomizeValues(numberOfButtons, Constants_Quille.DEFAULT_INTEREST_COUNT);

            foreach (int ID in IDsToSelect)
            {
                currentlySelectedButtons.Add(permittedButtons[ID]);
                ((CCUI_InterestButton)permittedButtons[ID]).RandomizeValueAndSelect();
            }

            PositionSelectedButtons();

            InterestsMenuUpdated?.Invoke();
        }


        // INIT
        protected override void Init()
        {
            base.Init();

            LoadSOsAndCreateButtons(Constants_PathResources.SO_PATH_INTERESTS);
            LoadDomainsAndCreateFilters(Constants_PathResources.SO_PATH_INTERESTDOMAINS);

            selectionBoxCapacity = Constants_Quille.MAXIMUM_INITIAL_INTEREST_COUNT;
        }


        // BUILT IN
        void Start()
        {
            Init();
        }
    }
}

