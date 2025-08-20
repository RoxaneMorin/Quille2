using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Quille;

namespace QuilleUI
{
    public class CCUI_PersonalityTraitsMenu : CCUI_GenericSteppedSelectableButtonMenu
    {
        // Test setup for a character creator's personality trait UI.
        // Procedurally populated from existing PersonalyTraitSOs.


        // PROPERTIES
        public float GetButtonValueFor(PersonalityTraitSO theTSO)
        {
            return ((CCUI_GenericSteppedSelectableButton)theButtonsDict[theTSO]).MyButtonValue;
        }

        public SerializedDictionary<PersonalityTraitSO, float> GetButtonsSOsAndValues()
        {
            SerializedDictionary<PersonalityTraitSO, float> SOsAndValuesDict = new SerializedDictionary<PersonalityTraitSO, float>();

            foreach (CCUI_PersonalityTraitButton button in currentlySelectedButtons)
            {
                SOsAndValuesDict.Add(button.MyPersonalityTraitSO, button.MyButtonValue);
            }

            return SOsAndValuesDict;
        }

        public void SetButtonValuesFromSOFloatDict(SerializedDictionary<PersonalityTraitSO, float> sourceDict)
        {
            ResetValues();

            foreach (KeyValuePair<PersonalityTraitSO, float> keyValuePair in sourceDict )
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


        // UTILITY
        public  void RandomizeValues()
        {
            base.RandomizeValues(Constants_Quille.DEFAULT_PERSONALITY_TRAIT_COUNT);

            PersonalityTraitsMenuUpdated?.Invoke();
        }


        // INIT
        protected override void Init()
        {
            base.Init();

            LoadSOsAndCreateButtons(Constants_PathResources.SO_PATH_PERSONALITYTRAITS);
            LoadDomainsAndCreateFilters(Constants_PathResources.SO_PATH_PERSONALITYTRAITDOMAINS);

            selectionBoxCapacity = Constants_Quille.MAXIMUM_INITIAL_PERSONALITY_TRAIT_COUNT;
        }
    }
}