using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace QuilleUI
{
    public class CCUI_GenericSelectableButtonMenu : MonoBehaviour
    {
        // Generic menu where selectableButtons are automatically generated, and can be selected.


        // VARIABLES
        [Header("Parameters")]
        [SerializeField] protected Transform buttonPrefab;
        [SerializeField] protected int countPerRow;
        [SerializeField] protected float rowShift;
        [SerializeField] protected float containerPadding;
        [SerializeField] protected float selectedShift;
        [SerializeField] protected float selectedMaximumSpacing;

        [Header("References")]
        [SerializeField] protected Canvas ownerCanvas;
        [SerializeField] protected RectTransform selectedButtonsContainerTransform;
        [SerializeField] protected RectTransform allButtonsContainerTransform;
        [SerializeField] protected Transform initialButtonTransform;

        protected float containerWidthPadded;
        protected float containerHalfWidthPadded;
        protected float initialXPos;
        protected float initialYPos;

        [Header("The Stuff")]
        [SerializeField] protected CCUI_GenericSelectableButton[] theButtons;
        [SerializeField] protected SerializedDictionary<ScriptableObject, CCUI_GenericSelectableButton> theButtonsDict;
        [SerializeField] protected Transform[] theButtonsTransforms;
        [Space]
        [SerializeField] protected List<CCUI_GenericSelectableButton> currentlySelectedButtons;
        // A HashSet would be better, but doesn't show up in editor :/



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
                    Vector2 newPosition = new Vector2(initialXPos + i * expectedDistanceBetweenButtons, selectedShift);
                    button.ChangeParentAndPosition(selectedButtonsContainerTransform, newPosition);

                    i++;
                }
            } 
        }

        public virtual List<int> RandomizeValues(int numberOfButtons, int numberToSelect)
        {
            ResetValues();

            int minNumberToSelect = Mathf.Min(numberToSelect, numberOfButtons);
            return RandomExtended.NonRepeatingIntegersInRange(0, numberOfButtons, minNumberToSelect);
        }
        public virtual void RandomizeValues(int numberToSelect)
        {
            CCUI_GenericSelectableButton[] permittedButtons = theButtons.Where(button => !button.IsForbidden).ToArray();
            int numberOfButtons = permittedButtons.Length;

            List<int> IDsToSelect = RandomizeValues(numberOfButtons, numberToSelect);

            foreach (int ID in IDsToSelect)
            {
                currentlySelectedButtons.Add(theButtons[ID]);
                theButtons[ID].Select();
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
            containerWidthPadded = allButtonsContainerTransform.rect.width - (2 * containerPadding);
            containerHalfWidthPadded = containerWidthPadded / 2;

            initialXPos = allButtonsContainerTransform.anchoredPosition.x - containerHalfWidthPadded;
            initialYPos = ((RectTransform)initialButtonTransform).anchoredPosition.y;
        }

        protected virtual void LoadSOsAndCreateButtons(string SOsResourcePath)
        {
            ScriptableObject[] theSOs = Resources.LoadAll<ScriptableObject>(SOsResourcePath);
            int noOfSOs = theSOs.Length;

            theButtons = new CCUI_GenericSelectableButton[noOfSOs];
            theButtonsDict = new SerializedDictionary<ScriptableObject, CCUI_GenericSelectableButton>();
            theButtonsTransforms = new RectTransform[noOfSOs];

            // Calculate the necessry values for generating the grid of buttons.
            int noOfRows = Mathf.CeilToInt((float)noOfSOs / countPerRow);
            float distanceBetweenButtons = containerWidthPadded / (countPerRow - 1);

            // Generate the buttons.
            int i = 0;
            for (int j = 0; j < noOfRows; j++)
            {
                for (int k = 0; k < countPerRow; k++)
                {
                    // Break 
                    if (i == noOfSOs)
                    {
                        break;
                    }

                    theButtonsTransforms[i] = Instantiate<Transform>(buttonPrefab, initialButtonTransform);
                    theButtons[i] = theButtonsTransforms[i].GetComponent<CCUI_GenericSelectableButton>();
                    theButtonsDict.Add(theSOs[i], theButtons[i]);

                    // Set the button's parent and positione.
                    RectTransform thisButtonsRectTransform = theButtonsTransforms[i].GetComponent<RectTransform>();
                    thisButtonsRectTransform.SetParent(allButtonsContainerTransform, false);
                    thisButtonsRectTransform.anchoredPosition = new Vector2(initialXPos + k * distanceBetweenButtons, initialYPos + j * rowShift);

                    // Initialize the button.
                    theButtons[i].gameObject.SetActive(true);
                    theButtons[i].Init(theSOs[i]);
 
                    // Subscribe to its event.
                    theButtons[i].SelectableButtonUpdated += OnSelectableButtonUpdated;

                    i++;
                }
            }
        }

        protected virtual void Init()
        {
            FetchComponents();
            //LoadSOsAndCreateButtons() called from child classes;
        }


        // BUILT IN
        void Start()
        {
            Init();
        }
    }
}

