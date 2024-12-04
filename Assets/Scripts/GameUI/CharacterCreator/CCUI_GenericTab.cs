using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace QuilleUI
{
    public class CCUI_GenericTab : MonoBehaviour
    {
        // Test setup for a generic tab in the character creator.


        // VARIABLES
        [Header("References")]
        [SerializeField] private CCUI_GenericMenu myParentMenu;
        [SerializeField] private Button myButton;

        [Header("Utility")]
        [SerializeField] private string tabName;
        [SerializeField] private Color buttonColourDefault;
        [SerializeField] private Color buttonColourActive;


        // PROPERTIES
        public Button MyButtom { get { return myButton; } }



        // METHODS

        // EVENT LISTENERS
        public void OnSiblingActivated(CCUI_GenericTab activatedTab)
        {
            //Debug.Log(this.name + " received OnSiblingActivated for " + activatedTab.name);

            if (this != activatedTab)
            {
                DeactivateMe();
            }
        }


        // UTILITY
        public void ActivateMe()
        {
            // Update the corresponding button.
            myButton.image.color = buttonColourActive;

            gameObject.SetActive(true);
        }
        public void DeactivateMe()
        {
            // Update the corresponding button.
            myButton.image.color = buttonColourDefault;

            gameObject.SetActive(false);
        }


        // INIT
        private void Init()
        {
            // Find parent menu & register to its event.
            myParentMenu = gameObject.GetComponentInParent<CCUI_GenericMenu>(true);

            // Find buttons if none have been assigned?
            if (!myButton)
            {
                Button[] potentialButtons = myParentMenu.GetComponentsInChildren<Button>(true).Where(button => button.CompareTag("UI_TabButton")).ToArray();
                myButton = potentialButtons.First(button => button.name.Contains(tabName));

                // Registering to the Unity envent doesn't seem possible.
            }
        }


        // BUILT IN
        void Start()
        {
            Init();
        }
    }
}