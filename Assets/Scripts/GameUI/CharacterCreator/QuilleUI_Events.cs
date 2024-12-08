using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QuilleUI
{
    // GENERIC TAB EVENTS
    public delegate void ActiveTabUpdate(CCUI_GenericTab newActiveTab);


    // CHARACTER CREATOR EVENTS
    public delegate void NameMenuUpdate();

    public delegate void PersonalityAxeSliderUpdate(Quille.PersonalityAxeSO relevantPersonalityAxeSO);
    public delegate void SteppedButtonUpdate(CCUI_GenericSteppedButton relevantSteppedButton, bool shouldItMove);

    public delegate void PersonalityAxesMenuUpdate();
    public delegate void PersonalityTraitsMenuUpdate();
    public delegate void DrivesMenuUpdate();

    public delegate void TargetPersonModified(Quille.Person theTargetedPerson);
}
