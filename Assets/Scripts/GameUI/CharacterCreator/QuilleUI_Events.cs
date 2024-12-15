using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QuilleUI
{
    // GENERIC TAB EVENTS
    public delegate void ActiveTabUpdate(CCUI_Tab newActiveTab);

    // SAVE AND LOAD EVENTS
    public delegate void FilePicked(string filePath);

    // CHARACTER CREATOR EVENTS

    // NAME MENUS
    public delegate void AdditionalNameInputFieldUpdate(CCUI_AdditionalNameInputField relevantInputField);
    public delegate void AdditionalNameDeleted(CCUI_AdditionalNameInputField relevantInputField);

    public delegate void AdditionalNameMenuUpdate();
    public delegate void NameMenuUpdate();

    // PERSONALITY MENUS
    public delegate void PersonalityAxesMenuUpdate();
    public delegate void PersonalityTraitsMenuUpdate();
    public delegate void DrivesMenuUpdate();
    public delegate void InterestsMenuUpdate();

    public delegate void SelectableButtonUpdate(CCUI_GenericSelectableButton relevantButton, bool shouldItMove);
    public delegate void SteppedButtonUpdate(CCUI_GenericSteppedSelectableButton relevantSteppedButton, bool shouldItMove);

    public delegate void InterestButtonUpdate(CCUI_InterestButton relevantInterestButton, bool shouldItMove);
    public delegate void PersonalityAxeSliderUpdate(Quille.PersonalityAxeSO relevantPersonalityAxeSO);


    public delegate void TargetPersonModified(Quille.Person theTargetedPerson);
}
