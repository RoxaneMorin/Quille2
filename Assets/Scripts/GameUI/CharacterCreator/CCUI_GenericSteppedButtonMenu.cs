using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace QuilleUI
{
    public class CCUI_GenericSteppedButtonMenu : MonoBehaviour
    {
        // Generic menu where steppedButtons are automatically generated, can be selected and scaled.
        // Procedurally populated from a given type of scriptable objects.


        // VARIABLES
        [Header("Parameters")]
        [SerializeField] protected Transform steppedButtonPrefab;
        [SerializeField] protected int countPerRow;
        [SerializeField] protected float rowShift;
        [SerializeField] protected float containerPadding;
        [SerializeField] protected float selectedShift;

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
        [SerializeField] protected CCUI_GenericSteppedButton[] theButtons;
        [SerializeField] protected SerializedDictionary<ScriptableObject, CCUI_GenericSteppedButton> theButtonsDict;
        [SerializeField] protected Transform[] theButtonsTransforms;
        [Space]
        [SerializeField] protected List<CCUI_GenericSteppedButton> currentlySelectedButtons;
        // A HashSet would be better, but doesn't show up in editor :/



        // METHODS

        // EVENT LISTENERS
        public virtual void OnSteppedButtonUpdated(CCUI_GenericSteppedButton theUpdatedButton, bool shouldItMove)
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
        protected virtual void MoveButtonToSelected(CCUI_GenericSteppedButton theTargetButton)
        {
            currentlySelectedButtons.Add(theTargetButton);
            PositionSelectedButtons();
        }
        protected virtual void RemoveButtonFromSelected(CCUI_GenericSteppedButton theTargetButton)
        {
            theTargetButton.ChangeParentAndPosition(allButtonsContainerTransform, theTargetButton.MyDefaultPosition);

            currentlySelectedButtons.Remove(theTargetButton);
            PositionSelectedButtons();
        }

        protected virtual void PositionSelectedButtons()
        {
            int expectedNumberOfButtons = Quille.Constants.DEFAULT_PERSONALITY_TRAIT_COUNT;
            float distanceBetweenButtons = containerWidthPadded / (expectedNumberOfButtons - 1);

            int i = 0;
            foreach (CCUI_GenericSteppedButton button in currentlySelectedButtons)
            {
                Vector2 newPosition = new Vector2(initialXPos + i * distanceBetweenButtons, selectedShift);
                button.ChangeParentAndPosition(selectedButtonsContainerTransform, newPosition);

                i++;
            }
        }

        public virtual void RandomizeValues(int numberToSelect)
        {
            ResetValues();

            int numberOfButtons = theButtons.Length;
            int minNumberToSelect = Mathf.Min(numberToSelect, numberOfButtons);

            List<int> IDsToSelect = RandomExtended.NonRepeatingIntegersInRange(0, numberOfButtons, minNumberToSelect);

            foreach (int ID in IDsToSelect)
            {
                currentlySelectedButtons.Add(theButtons[ID]);

                theButtons[ID].RandomizeValueAndSelect();
            }

            PositionSelectedButtons();
        }

        public virtual void ResetValues()
        {
            foreach (CCUI_GenericSteppedButton button in currentlySelectedButtons)
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

            theButtons = new CCUI_GenericSteppedButton[noOfSOs];
            theButtonsDict = new SerializedDictionary<ScriptableObject, CCUI_GenericSteppedButton>();
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

                    theButtonsTransforms[i] = Instantiate<Transform>(steppedButtonPrefab, initialButtonTransform);
                    theButtons[i] = theButtonsTransforms[i].GetComponent<CCUI_GenericSteppedButton>();
                    theButtonsDict.Add(theSOs[i], theButtons[i]);

                    // Set the button's parent and positione.
                    RectTransform thisButtonsRectTransform = theButtonsTransforms[i].GetComponent<RectTransform>();
                    thisButtonsRectTransform.SetParent(allButtonsContainerTransform, false);
                    thisButtonsRectTransform.anchoredPosition = new Vector2(initialXPos + k * distanceBetweenButtons, initialYPos + j * rowShift);

                    // Initialize the button.
                    theButtons[i].Init(theSOs[i]);

                    // Subscribe to its event.
                    theButtons[i].SteppedButtonUpdated += OnSteppedButtonUpdated;

                    i++;
                }
            }
        }

        protected virtual void Init()
        {
            FetchComponents();

            // Call LoadSOsAndCreateButtons with the relevant SO path in child classes.
        }


        // BUILT IN
        void Start()
        {
            Init();
        }
    }

}