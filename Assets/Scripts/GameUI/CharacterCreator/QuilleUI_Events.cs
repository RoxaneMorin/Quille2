using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QuilleUI
{
    // GENERIC TAB EVENTS
    public delegate void ActiveTabUpdate(CCUI_Tab newActiveTab);


    // CHARACTER CREATOR EVENTS
    public delegate void NameMenuUpdate();

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
