using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Quille;

namespace QuilleUI
{
    public class CCUI_DriveButton : CCUI_GenericSteppedSelectableButton
    {
        // Test setup for a character creator's individual drive UI.


        // PARAMETERS
        internal DriveSO MyDriveSO { get { return (DriveSO)mySO; } set { mySO = value; } }



        // METHODS

        // OVERRIDES
        public override void PermitIfCompatible(Quille.Person theTargetPerson, bool selectionBoxAtCapacity)
        {
            if (MyDriveSO.IsCompatibleWithPerson(theTargetPerson) && (isSelected || !selectionBoxAtCapacity))
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
            return string.Format("{0} ({1})", MyDriveSO.ItemName, MyButtonValue);
        }

        // INIT
        public override void Init(PersonalityItemSO sourceSO)
        {
            base.Init(sourceSO);

            if (mySO is DriveSO)
            {
                DriveSO myDriveSO = MyDriveSO;

                myIcon.sprite = myDriveSO.ItemIcon;
                myCaption.text = myDriveSO.ItemName;
                myDefaultCaption = myCaption.text;

                gameObject.name = string.Format("DriveButton_{0}", myDriveSO.ItemName);
            }
        }
    }
}
