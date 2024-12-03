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

        public SerializedDictionary<Quille.PersonalityTraitSO, float> SetButtonsSOsAndValues()
        {
            return currentlySelectedButtons.ToSerializedDictionary(button => ((CCUI_PersonalityTraitButton)button).MyPersonalityTraitSO, button => button.MyButtonValue);
        }



        // METHODS

        // INIT
        protected override void Init()
        {
            base.Init();
            LoadSOsAndCreateButtons(PathConstants.SO_PATH_PERSONALITYTRAITS);
        }
    }
}
