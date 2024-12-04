using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace QuilleUI
{
    public class CCUI_PersonalityTraitsMenu : CCUI_GenericSteppedButtonMenu
    {
        // Test setup for a character creator's personality trait UI.
        // Procedurally populated from existing PersonalyTraitSOs.


        // PROPERTIES
        public float GetButtonValueFor(Quille.PersonalityTraitSO theTSO)
        {
            return theButtonsDict[theTSO].MyButtonValue;
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
                    theButtonsDict[keyValuePair.Key].Select(keyValuePair.Value);
                    currentlySelectedButtons.Add(theButtonsDict[keyValuePair.Key]);
                }
            }

            PositionSelectedButtons();
        }


        // EVENTS
        public event PersonalityTraitsMenuUpdate PersonalityTraitsMenuUpdated;



        // METHODS

        // EVENT LISTENERS
        public override void OnSteppedButtonUpdated(CCUI_GenericSteppedButton theUpdatedButton, bool shouldItMove)
        {
            base.OnSteppedButtonUpdated(theUpdatedButton, shouldItMove);

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
        public  void RandomizeValues()
        {
            base.RandomizeValues(Quille.Constants.DEFAULT_PERSONALITY_TRAIT_COUNT);

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