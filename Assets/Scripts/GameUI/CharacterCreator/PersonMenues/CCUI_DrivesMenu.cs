using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Quille;

namespace QuilleUI
{
    public class CCUI_DrivesMenu : CCUI_GenericSteppedSelectableButtonMenu
    {
        // Test setup for a character creator's drive UI.
        // Procedurally populated from existing DriveSOs.


        // PROPERTIES
        public float GetButtonValueFor(DriveSO theTSO)
        {
            return ((CCUI_GenericSteppedSelectableButton)theButtonsDict[theTSO]).MyButtonValue;
        }

        public SerializedDictionary<DriveSO, float> GetButtonsSOsAndValues()
        {
            SerializedDictionary<DriveSO, float> SOsAndValuesDict = new SerializedDictionary<DriveSO, float>();

            foreach (CCUI_DriveButton button in currentlySelectedButtons)
            {
                SOsAndValuesDict.Add(button.MyDriveSO, button.MyButtonValue);
            }

            return SOsAndValuesDict;
        }

        public void SetButtonValuesFromSOFloatDict(SerializedDictionary<DriveSO, float> sourceDict)
        {
            ResetValues();

            foreach (KeyValuePair<DriveSO, float> keyValuePair in sourceDict)
            {
                if (theButtonsDict.ContainsKey(keyValuePair.Key))
                {
                    ((CCUI_GenericSteppedSelectableButton)theButtonsDict[keyValuePair.Key]).Select(keyValuePair.Value);
                    currentlySelectedButtons.Add(theButtonsDict[keyValuePair.Key]);
                }
            }

            PositionSelectedButtons();
        }


        // EVENTS
        public event DrivesMenuUpdate DrivesMenuUpdated;



        // METHODS

        // EVENT LISTENERS
        public override void OnSelectableButtonUpdated(CCUI_GenericSelectableButton theUpdatedButton, bool shouldItMove)
        {
            base.OnSelectableButtonUpdated(theUpdatedButton, shouldItMove);

            DrivesMenuUpdated?.Invoke();
        }


        // UTILITY
        public override void RandomizeValues(int numberToSelect)
        {
            ResetValues();

            CCUI_GenericSelectableButton[] permittedButtons = theButtons.Where(button => !button.IsForbidden).ToArray();
            int numberOfButtons = permittedButtons.Length;

            List<int> IDsToSelect = RandomizeValues(numberOfButtons, numberToSelect);

            int i = 0;
            foreach (int ID in IDsToSelect)
            {
                currentlySelectedButtons.Add(permittedButtons[ID]);

                // Pick one full drive, two half ones.
                float value = (i == 1 ? 1f : 0.5f);
                ((CCUI_GenericSteppedSelectableButton)permittedButtons[ID]).Select(value);

                i++;
            }

            PositionSelectedButtons();
        }
        public void RandomizeValues()
        {
            RandomizeValues(Constants_Quille.DEFAULT_DRIVES_COUNT);

            DrivesMenuUpdated?.Invoke();
        }


        // INIT
        protected override void Init()
        {
            base.Init();

            LoadSOsAndCreateButtons(Constants_PathResources.SO_PATH_DRIVES);
            LoadDomainsAndCreateFilters(Constants_PathResources.SO_PATH_DRIVEDOMAINS);

            selectionBoxCapacity = Constants_Quille.MAXIMUM_INITIAL_DRIVES_COUNT;
        }
    }

}
