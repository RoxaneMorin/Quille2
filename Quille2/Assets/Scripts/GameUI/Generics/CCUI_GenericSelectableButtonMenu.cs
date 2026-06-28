using AYellowpaper.SerializedCollections;
using Quille;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace QuilleUI
{
    public class CCUI_GenericSelectableButtonMenu : MonoBehaviour
    {
        // Generic menu where selectableButtons are automatically generated, and can be selected.


        // VARIABLES
        [Header("Parameters")]
        [SerializeField] protected Transform buttonPrefab;
        [SerializeField] protected float buttonContainerPadding;
        [SerializeField] protected int buttonCountPerRow;
        [SerializeField] protected float buttonSpacingBetweenRows;
        [Space]
        [SerializeField] protected Transform domainFilterPrefab;
        [SerializeField] protected float domainFilterSpacing;
        [Space]
        [SerializeField] protected int selectionBoxCapacity;
        [SerializeField] protected float selectedShift;
        [SerializeField] protected float selectedMaximumSpacing;

        [Header("References")]
        [SerializeField] protected Canvas ownerCanvas;
        [SerializeField] protected RectTransform selectedButtonsContainerTransform;
        [SerializeField] protected RectTransform allButtonsContainerTransform;
        [Space]
        [SerializeField] protected Transform initialButtonTransform;
        [SerializeField] protected Transform initialDomainTransform;

        protected float containerWidthPadded;
        protected float containerHalfWidthPadded;
        protected float initialButtonXPos;
        protected float initialButtonYPos;

        protected float containerWidthDomainPadded;
        protected float initialDomainXPos;

        [Header("The Stuff")]
        [SerializeField] protected CCUI_GenericSelectableButton[] theButtons;
        [SerializeField] protected SerializedDictionary<ScriptableObject, CCUI_GenericSelectableButton> theButtonsDict;
        [SerializeField] protected Transform[] theButtonsTransforms;
        [Space]
        [SerializeField] protected CCUI_DomainFilter[] theDomainsFilters;
        [SerializeField] protected Transform[] theDomainsFiltersTransforms;
        [Space]
        [SerializeField] protected List<CCUI_GenericSelectableButton> currentlySelectedButtons;
        [SerializeField] protected CCUI_DomainFilter currentlyActiveFilter;
        // A HashSet would be better, but doesn't show up in editor :/


        // PROPERTIES
        public bool SelectionBoxAtCapacity { get { return currentlySelectedButtons.Count >= selectionBoxCapacity; } }


        // EVENTS
        public event DomainFilterUpdate DomainFilterUpdated;



        // METHODS

        // EVENT LISTENERS
        public virtual void OnSelectableButtonUpdated(CCUI_GenericSelectableButton theUpdatedButton, bool shouldItMove)
        {
            if (shouldItMove)
            {
                if (theUpdatedButton.IsSelected)
                {
                    MoveButtonToSelected(theUpdatedButton);
                }
                else
                {
                    RemoveButtonFromSelected(theUpdatedButton);
                }
            }
        }
        public virtual void OnDomainFilterUpdated(CCUI_DomainFilter theDomainFilter)
        {
            currentlyActiveFilter = currentlyActiveFilter == theDomainFilter ? null : theDomainFilter;
            DomainFilterUpdated?.Invoke(currentlyActiveFilter);
        }

        public virtual void OnTargetPersonModified(Person theTargetPerson)
        {
            // Does this happen too often?
            foreach (CCUI_GenericSelectableButton button in theButtons)
            {
                button.PermitIfValid(theTargetPerson, SelectionBoxAtCapacity);
            }
        }


        // UTILITY
        protected virtual void MoveButtonToSelected(CCUI_GenericSelectableButton theTargetButton)
        {
            currentlySelectedButtons.Add(theTargetButton);
            PositionSelectedButtons();
        }
        protected virtual void RemoveButtonFromSelected(CCUI_GenericSelectableButton theTargetButton)
        {
            theTargetButton.ChangeParentAndPosition(allButtonsContainerTransform, theTargetButton.MyDefaultPosition);

            currentlySelectedButtons.Remove(theTargetButton);
            PositionSelectedButtons();
        }

        protected virtual void PositionSelectedButtons()
        {
            int numberOfButtons = currentlySelectedButtons.Count;
            float expectedDistanceBetweenButtons = containerWidthPadded / (numberOfButtons - 1);

            int i = 0;
            if (expectedDistanceBetweenButtons > selectedMaximumSpacing)
            {
                // Place the buttons around the center.
                float selectedButtonContainerXTransform = selectedButtonsContainerTransform.anchoredPosition.x;
                float localInitialXPos = selectedButtonContainerXTransform - selectedMaximumSpacing * ((numberOfButtons - 1) / 2f);

                foreach (CCUI_GenericSelectableButton button in currentlySelectedButtons)
                {
                    Vector2 newPosition = new Vector2(localInitialXPos + i * selectedMaximumSpacing, selectedShift);
                    button.ChangeParentAndPosition(selectedButtonsContainerTransform, newPosition);

                    i++;
                }
            }
            else
            { 
                // Place them going from one side to the other.
                foreach (CCUI_GenericSelectableButton button in currentlySelectedButtons)
                {
                    Vector2 newPosition = new Vector2(initialButtonXPos + i * expectedDistanceBetweenButtons, selectedShift);
                    button.ChangeParentAndPosition(selectedButtonsContainerTransform, newPosition);

                    i++;
                }
            }
        }

        public void ForbidUnselectedButtons()
        {
            foreach (CCUI_GenericSelectableButton button in theButtons)
            {
                if (!button.IsSelected)
                {
                    button.Forbid();
                }
            }
        }
        public void PermitUnselectedButtons()
        {
            foreach (CCUI_GenericSelectableButton button in theButtons)
            {
                if (!button.IsSelected)
                {
                    button.Permit();
                }
            }
        }

        public virtual List<int> RandomizeValues(int numberOfButtons, int numberToSelect)
        {
            int minNumberToSelect = Mathf.Min(numberToSelect, numberOfButtons);
            return RandomExtended.NonRepeatingIntegersInRange(0, numberOfButtons, minNumberToSelect);
        }
        public virtual void RandomizeValues(int numberToSelect)
        {
            ResetValues();

            CCUI_GenericSelectableButton[] permittedButtons = theButtons.Where(button => !button.IsForbidden && !button.IsExlcudedByCurrentFilter).ToArray();
            int numberOfButtons = permittedButtons.Length;

            List<int> IDsToSelect = RandomizeValues(numberOfButtons, numberToSelect);

            foreach (int ID in IDsToSelect)
            {
                currentlySelectedButtons.Add(permittedButtons[ID]);
                permittedButtons[ID].Select();
            }

            PositionSelectedButtons();
        }

        public virtual void ResetValues()
        {
            foreach (CCUI_GenericSelectableButton button in currentlySelectedButtons)
            {
                button.Unselect();
                button.ChangeParentAndPosition(allButtonsContainerTransform, button.MyDefaultPosition);
            }

            currentlySelectedButtons.Clear();
        }


        // INIT
        protected virtual void FetchComponents()
        {
            ownerCanvas = GetComponentInParent<Canvas>();

            if (!initialButtonTransform)
            {
                initialButtonTransform = gameObject.transform;
            }

            // And calculate values for reuse.
            containerWidthPadded = allButtonsContainerTransform.rect.width - (2 * buttonContainerPadding);
            containerHalfWidthPadded = containerWidthPadded / 2;

            initialButtonXPos = allButtonsContainerTransform.anchoredPosition.x - containerHalfWidthPadded;
            initialButtonYPos = ((RectTransform)initialButtonTransform).anchoredPosition.y;

            containerWidthDomainPadded = allButtonsContainerTransform.rect.width - (2 * domainFilterSpacing);
            initialDomainXPos = -containerWidthDomainPadded / 2;
        }

        protected virtual void LoadSOsAndCreateButtons(string SOsResourcePath)
        {
            PersonalityItemSO[] theSOs = Resources.LoadAll<PersonalityItemSO>(SOsResourcePath);
            theSOs = theSOs.OrderBy(so => so.MenuSortingIndex).ToArray();
            int noOfSOs = theSOs.Length;

            theButtons = new CCUI_GenericSelectableButton[noOfSOs];
            theButtonsDict = new SerializedDictionary<ScriptableObject, CCUI_GenericSelectableButton>();
            theButtonsTransforms = new RectTransform[noOfSOs];

            // Calculate the necessry values for generating the grid of buttons.
            int noOfRows = Mathf.CeilToInt((float)noOfSOs / buttonCountPerRow);
            float distanceBetweenButtons = containerWidthPadded / (buttonCountPerRow - 1);

            // Generate the buttons.
            int i = 0;
            for (int j = 0; j < noOfRows; j++)
            {
                for (int k = 0; k < buttonCountPerRow; k++)
                {
                    // Break 
                    if (i == noOfSOs)
                    {
                        break;
                    }

                    theButtonsTransforms[i] = Instantiate<Transform>(buttonPrefab, initialButtonTransform);
                    theButtons[i] = theButtonsTransforms[i].GetComponent<CCUI_GenericSelectableButton>();
                    theButtonsDict.Add(theSOs[i], theButtons[i]);

                    // Set the button's parent and position.
                    RectTransform thisButtonsRectTransform = theButtonsTransforms[i].GetComponent<RectTransform>();
                    thisButtonsRectTransform.SetParent(allButtonsContainerTransform, false);
                    thisButtonsRectTransform.anchoredPosition = new Vector2(initialButtonXPos + k * distanceBetweenButtons, initialButtonYPos + j * buttonSpacingBetweenRows);

                    // Initialize the button.
                    theButtons[i].gameObject.SetActive(true);
                    theButtons[i].Init(theSOs[i]);
 
                    // Event subscriptions.
                    theButtons[i].SelectableButtonUpdated += OnSelectableButtonUpdated;
                    DomainFilterUpdated += theButtons[i].OnDomainFilterUpdated;

                    i++;
                }
            }
        }
        protected virtual void LoadDomainsAndCreateFilters(string SOsResourcePath)
        {
            if (domainFilterPrefab)
            {
                PersonalityItemDomainSO[] theDomainSOs = Resources.LoadAll<PersonalityItemDomainSO>(SOsResourcePath);
                theDomainSOs = theDomainSOs.OrderBy(so => so.MenuSortingIndex).ToArray();
                int noOfDomains = theDomainSOs.Length;

                // Create the arrays.
                theDomainsFiltersTransforms = new Transform[noOfDomains];
                theDomainsFilters = new CCUI_DomainFilter[noOfDomains];

                // Calculate reused values.
                float horizontalSpaceAvailable = containerWidthDomainPadded - (domainFilterSpacing * (noOfDomains - 1f));
                float domainFilterWidth = horizontalSpaceAvailable / noOfDomains;
                float distanceBetweenDomainFilters = domainFilterWidth + domainFilterSpacing;

                RectTransform initialDomainRectTransform = (RectTransform)initialDomainTransform;

                // Create the UI elements.
                for (int i = 0; i < noOfDomains; i++)
                {
                    theDomainsFiltersTransforms[i] = Instantiate<Transform>(domainFilterPrefab, initialDomainTransform);
                    theDomainsFilters[i] = theDomainsFiltersTransforms[i].GetComponent<CCUI_DomainFilter>();

                    // Set the domainFilter's parent and position.
                    RectTransform thisDomainFiltersRectTransform = theDomainsFiltersTransforms[i].GetComponent<RectTransform>();
                    thisDomainFiltersRectTransform.SetParent(allButtonsContainerTransform, false);
                    thisDomainFiltersRectTransform.anchoredPosition = new Vector2(initialDomainXPos + i * distanceBetweenDomainFilters, initialDomainRectTransform.anchoredPosition.y);
                    thisDomainFiltersRectTransform.sizeDelta = new Vector2(domainFilterWidth, thisDomainFiltersRectTransform.sizeDelta.y);

                    // Init & subscribe to events.
                    theDomainsFilters[i].Init(theDomainSOs[i], theButtonsDict);
                    theDomainsFilters[i].DomainFilterClicked += OnDomainFilterUpdated;
                    DomainFilterUpdated += theDomainsFilters[i].OnActiveDomainFilterUpdated;
                }
            }
        }

        protected virtual void Init()
        {
            FetchComponents();

            //LoadDomainsAndCreateButtons();
            //LoadDomainsAndCreateFilters() called from child classes;
        }


        // BUILT IN
        void Start()
        {
            Init();
        }
    }
}

