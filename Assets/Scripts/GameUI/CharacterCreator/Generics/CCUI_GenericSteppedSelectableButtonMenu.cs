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
        public override void RandomizeValues(int numberToSelect)
        {
            CCUI_GenericSelectableButton[] permittedButtons = theButtons.Where(button => !button.IsForbidden).ToArray();
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