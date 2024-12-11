using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace QuilleUI
{
    public class CCUI_DrivesMenu : CCUI_GenericSteppedSelectableButtonMenu
    {
        // Test setup for a character creator's drive UI.
        // Procedurally populated from existing DriveSOs.


        // PROPERTIES
        public float GetButtonValueFor(Quille.DriveSO theTSO)
        {
            return ((CCUI_GenericSteppedSelectableButton)theButtonsDict[theTSO]).MyButtonValue;
        }

        public SerializedDictionary<Quille.DriveSO, float> GetButtonsSOsAndValues()
        {
            SerializedDictionary<Quille.DriveSO, float> SOsAndValuesDict = new SerializedDictionary<Quille.DriveSO, float>();

            foreach (CCUI_DriveButton button in currentlySelectedButtons)
            {
                SOsAndValuesDict.Add(button.MyDriveSO, button.MyButtonValue);
            }

            return SOsAndValuesDict;
        }

        public void SetButtonValuesFromSOFloatDict(SerializedDictionary<Quille.DriveSO, float> sourceDict)
        {
            ResetValues();

            foreach (KeyValuePair<Quille.DriveSO, float> keyValuePair in sourceDict)
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

        public void OnTargetPersonModified(Quille.Person theTargetPerson)
        {
            // TODO: is there a cleaner/less repetitive way to do this?

            //Debug.Log(string.Format("{0} sees that the targetPerson, {1}, was modified.", this.name, theTargetPerson.name));

            foreach (CCUI_DriveButton button in theButtons)
            {
                if (button.MyDriveSO.ForbiddenToPerson(theTargetPerson))
                {
                    button.Forbid(button.IsSelected);
                }
                else
                {
                    button.Permit(button.IsSelected);
                }
            }
        }


        // UTILITY
        public void RandomizeValues()
        {
            base.RandomizeValues(Quille.Constants.DEFAULT_DRIVES_COUNT);

            DrivesMenuUpdated?.Invoke();
        }


        // INIT
        protected override void Init()
        {
            base.Init();
            LoadSOsAndCreateButtons(PathConstants.SO_PATH_DRIVES);
        }
    }

}
