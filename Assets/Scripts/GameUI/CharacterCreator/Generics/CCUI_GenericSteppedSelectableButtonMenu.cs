using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace QuilleUI
{
    public class CCUI_GenericSteppedSelectableButtonMenu : CCUI_GenericSelectableButtonMenu
    {
        // METHODS

        // UTILITY
        public virtual List<int> RandomizeValues(int numberOfButtons, int numberToSelect)
        {
            ResetValues();

            int minNumberToSelect = Mathf.Min(numberToSelect, numberOfButtons);
            return RandomExtended.NonRepeatingIntegersInRange(0, numberOfButtons, minNumberToSelect);
        }

        public new void RandomizeValues(int numberToSelect)
        {
            CCUI_GenericSelectableButton[] permittedButtons = theButtons.Where(button => !((CCUI_GenericSteppedSelectableButton)button).IsForbidden).ToArray();
            int numberOfButtons = permittedButtons.Length;

            List<int> IDsToSelect = RandomizeValues(numberOfButtons, numberToSelect);

            foreach (int ID in IDsToSelect)
            {
                currentlySelectedButtons.Add(theButtons[ID]);
                ((CCUI_GenericSteppedSelectableButton)theButtons[ID]).RandomizeValueAndSelect();
            }

            PositionSelectedButtons();
        }
    }
}

