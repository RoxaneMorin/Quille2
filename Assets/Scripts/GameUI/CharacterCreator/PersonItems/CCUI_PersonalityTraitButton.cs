using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Quille;

namespace QuilleUI
{
    public class CCUI_PersonalityTraitButton : CCUI_GenericSteppedSelectableButton
    {
        // Test setup for a character creator's individual personality trait UI.


        // PARAMETERS
        internal Quille.PersonalityTraitSO MyPersonalityTraitSO { get { return (PersonalityTraitSO)mySO; } set { mySO = value; } }



        // METHODS

        // OVERRIDES
        public override void PermitIfCompatible(Quille.Person theTargetPerson, bool selectionBoxAtCapacity)
        {
            if (MyPersonalityTraitSO.IsCompatibleWithPerson(theTargetPerson) && (isSelected || !selectionBoxAtCapacity))
            {
                Permit();
            }
            else
            {
                Forbid();
            }
        }

        protected override string MakeNewCaption()
        {
            return string.Format("{0} ({1})", MyPersonalityTraitSO.ItemName, MyButtonValue);
        }

        // INIT
        public override void Init(PersonalityItemSO sourceSO)
        {
            base.Init(sourceSO);

            if (mySO is PersonalityTraitSO)
            {
               PersonalityTraitSO myPersonalityTraitSO = MyPersonalityTraitSO;

                myIcon.sprite = myPersonalityTraitSO.ItemIcon;
                myCaption.text = myPersonalityTraitSO.ItemName;
                myDefaultCaption = myCaption.text;

                gameObject.name = string.Format("PersonalityTraitButton_{0}", myPersonalityTraitSO.ItemName);
            }
        }
    }
}

