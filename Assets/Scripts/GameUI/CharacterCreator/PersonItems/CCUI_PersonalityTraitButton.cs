using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuilleUI
{
    public class CCUI_PersonalityTraitButton : CCUI_GenericSteppedSelectableButton
    {
        // Test setup for a character creator's individual personality trait UI.


        // PARAMETERS
        internal Quille.PersonalityTraitSO MyPersonalityTraitSO { get { return (Quille.PersonalityTraitSO)mySO; } set { mySO = value; } }



        // METHODS

        // INIT
        public override void Init(ScriptableObject sourceSO)
        {
            base.Init(sourceSO);

            if (mySO is Quille.PersonalityTraitSO)
            {
                Quille.PersonalityTraitSO myPersonalityTraitSO = MyPersonalityTraitSO;

                myIcon.sprite = myPersonalityTraitSO.traitIcon;
                myCaption.text = myPersonalityTraitSO.TraitName;

                gameObject.name = string.Format("PersonalityTraitButton_{0}", myPersonalityTraitSO.TraitName);
            }
        }
    }
}

