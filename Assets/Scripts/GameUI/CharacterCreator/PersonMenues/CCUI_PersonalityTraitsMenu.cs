using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace QuilleUI
{
    public class CCUI_PersonalityTraitsMenu : CCUI_GenericSelectableButtonMenu
    {
        // Test setup for a character creator's personality trait UI.
        // Procedurally populated from existing PersonalyTraitSOs.


        // PROPERTIES
        public float GetButtonValueFor(Quille.PersonalityTraitSO theTSO)
        {
            return ((CCUI_GenericSteppedSelectableButton)theButtonsDict[theTSO]).MyButtonValue;
        }

        public SerializedDictionary<Quille.PersonalityTraitSO, float> GetButtonsSOsAndValues()
        {
            SerializedDictionary<Quille.PersonalityTraitSO, float> SOsAndValuesDict = new SerializedDictionary<Quille.PersonalityTraitSO, float>();

            foreach (CCUI_PersonalityTraitButton button in currentlySelectedButtons)
            {
                SOsAndValuesDict.Add(button.MyPersonalityTraitSO, button.MyButtonValue);
            }

            return SOsAndValuesDict;
        }

        public void SetButtonValuesFromSOFloatDict(SerializedDictionary<Quille.PersonalityTraitSO, float> sourceDict)
        {
            ResetValues();

            foreach (KeyValuePair<Quille.PersonalityTraitSO, float> keyValuePair in sourceDict )
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
        public event PersonalityTraitsMenuUpdate PersonalityTraitsMenuUpdated;



        // METHODS

        // EVENT LISTENERS
        public override void OnSelectableButtonUpdated(CCUI_GenericSelectableButton theUpdatedButton, bool shouldItMove)
        {
            base.OnSelectableButtonUpdated(theUpdatedButton, shouldItMove);

            PersonalityTraitsMenuUpdated?.Invoke();
        }

        public void OnTargetPersonModified(Quille.Person theTargetPerson)
        {
            // TODO: is there a cleaner/less repetitive way to do this?

            //Debug.Log(string.Format("{0} sees that the targetPerson, {1}, was modified.", this.name, theTargetPerson.name));

            foreach (CCUI_PersonalityTraitButton button in theButtons)
            {
                if (button.MyPersonalityTraitSO.ForbiddenToPerson(theTargetPerson))
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
        protected override void PositionSelectedButtons()
        {
            PositionSelectedButtons(Quille.Constants.DEFAULT_PERSONALITY_TRAIT_COUNT);
        }

        public  void RandomizeValues()
        {
            List<int> IDsToSelect = base.RandomizeValues(Quille.Constants.DEFAULT_PERSONALITY_TRAIT_COUNT);

            foreach (int ID in IDsToSelect)
            {
                currentlySelectedButtons.Add(theButtons[ID]);
                ((CCUI_GenericSteppedSelectableButton)theButtons[ID]).RandomizeValueAndSelect();
            }

            PositionSelectedButtons();

            PersonalityTraitsMenuUpdated?.Invoke();
        }


        // INIT
        protected override void Init()
        {
            base.Init();
            LoadSOsAndCreateButtons(PathConstants.SO_PATH_PERSONALITYTRAITS);
        }
    }
}