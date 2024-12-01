using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace QuilleUI
{
    public class CCUI_GenericMenu: MonoBehaviour
    {
        // Test setup for a generic menu of the character creator, containing various submenues/tabs.


        // VARIABLES
        [Header("References")]
        [SerializeField] private Canvas myOwnerCanvas;
        [SerializeField] private CCUI_GenericTab[] myTabs;
        [SerializeField] private Button[] myTabsButtons; // is this necessary?

        [Header("Utility")]
        [SerializeField] private CCUI_GenericTab activeTab;


        // EVENTS
        public event ActiveTabUpdate ActiveTabUpdated;



        // METHODS

        // UTILITY
        public void ActivateTab(CCUI_GenericTab targetTab)
        {
            // Activate the relevant tab.
            activeTab = targetTab;
            activeTab.ActivateMe();

            // Raise the event for others to deactivate.
            ActiveTabUpdated?.Invoke(activeTab);
        }


        // INIT
        private void FetchComponents()
        {
            myOwnerCanvas = GetComponentInParent<Canvas>(true);

            myTabs = gameObject.transform.GetComponentsInChildren<CCUI_GenericTab>(true);
            myTabsButtons = myTabs.Select(tab => tab.MyButtom).ToArray();
        }

        private void Init()
        {
            FetchComponents();

            ActivateTab(myTabs[0]);
        }


        // BUILT IN
        void Start()
        {
            Init();
        }
    }
}
