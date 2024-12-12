using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuilleUI
{
    public class CCUI_DriveButton : CCUI_GenericSteppedSelectableButton
    {
        // Test setup for a character creator's individual drive UI.


        // PARAMETERS
        internal Quille.DriveSO MyDriveSO { get { return (Quille.DriveSO)mySO; } set { mySO = value; } }



        // METHODS

        // OVERRIDES
        protected override string MakeNewCaption()
        {
            return string.Format("{0} ({1})", MyDriveSO.DriveName, MyButtonValue);
        }

        // INIT
        public override void Init(ScriptableObject sourceSO)
        {
            base.Init(sourceSO);

            if (mySO is Quille.DriveSO)
            {
                Quille.DriveSO myDriveSO = MyDriveSO;

                myIcon.sprite = myDriveSO.driveIcon;
                myCaption.text = myDriveSO.DriveName;
                myDefaultCaption = myCaption.text;

                gameObject.name = string.Format("DriveButton_{0}", myDriveSO.DriveName);
            }
        }
    }
}
