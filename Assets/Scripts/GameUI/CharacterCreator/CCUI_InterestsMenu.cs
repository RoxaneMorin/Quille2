using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace QuilleUI
{
    public class CCUI_InterestsMenu : MonoBehaviour
    {
        // Test setup for a character creator's interest UI.
        // Procedurally populated from existing InterestSO.
        // Sadly repeats a lot from the GenericSteppedButtonMenu without being able to inherit from it :/ maybe I should rework that stuff.


        // VARIABLES
        [Header("Parameters")]
        [SerializeField] protected Transform interestButtonPrefab;
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
        [SerializeField] protected CCUI_InterestButton[] theButtons;
        [SerializeField] protected SerializedDictionary<ScriptableObject, CCUI_InterestButton> theButtonsDict;
        [SerializeField] protected Transform[] theButtonsTransforms;
        [Space]
        [SerializeField] protected List<CCUI_InterestButton> currentlySelectedButtons;
        // A HashSet would be better, but doesn't show up in editor :/


        // PROPERTIES
        public float GetSliderValueFor(Quille.InterestSO theISO)
        {
            return theButtonsDict[theISO].MySliderValue;
        }

        public SerializedDictionary<Quille.InterestSO, float> GetButtonsSOsAndValues()
        {
            SerializedDictionary<Quille.InterestSO, float> SOsAndValuesDict = new SerializedDictionary<Quille.InterestSO, float>();

            foreach (CCUI_InterestButton button in currentlySelectedButtons)
            {
                SOsAndValuesDict.Add(button.MyInterestSO, button.MySliderValue);
            }

            return SOsAndValuesDict;
        }

        public void SetButtonValuesFromSOFloatDict(SerializedDictionary<Quille.InterestSO, float> sourceDict)
        {
            ResetValues();

            foreach (KeyValuePair<Quille.InterestSO, float> keyValuePair in sourceDict)
            {
                if (theButtonsDict.ContainsKey(keyValuePair.Key))
                {
                    theButtonsDict[keyValuePair.Key].Select(keyValuePair.Value);
                    currentlySelectedButtons.Add(theButtonsDict[keyValuePair.Key]);
                }
            }

            PositionSelectedButtons();
        }


        // EVENTS
        public event InterestsMenuUpdate InterestsMenuUpdated;



        // METHODS

        // EVENT LISTENERS
        public virtual void OnInterestButtonUpdated(CCUI_InterestButton theUpdatedButton, bool shouldItMove)
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

            InterestsMenuUpdated?.Invoke();
        }


        // UTILITY
        protected virtual void MoveButtonToSelected(CCUI_InterestButton theTargetButton)
        {
            currentlySelectedButtons.Add(theTargetButton);
            PositionSelectedButtons();
        }
        protected virtual void RemoveButtonFromSelected(CCUI_InterestButton theTargetButton)
        {
            theTargetButton.ChangeParentAndPosition(allButtonsContainerTransform, theTargetButton.MyDefaultPosition);

            currentlySelectedButtons.Remove(theTargetButton);
            PositionSelectedButtons();
        }

        protected virtual void PositionSelectedButtons()
        {
            int expectedNumberOfButtons = Quille.Constants.DEFAULT_INTEREST_COUNT;
            float distanceBetweenButtons = containerWidthPadded / (expectedNumberOfButtons - 1);

            int i = 0;
            foreach (CCUI_InterestButton button in currentlySelectedButtons)
            {
                Vector2 newPosition = new Vector2(initialXPos + i * distanceBetweenButtons, selectedShift);
                button.ChangeParentAndPosition(selectedButtonsContainerTransform, newPosition);

                i++;
            }
        }

        public void RandomizeValues()
        {
            RandomizeValues(Quille.Constants.DEFAULT_INTEREST_COUNT);

            InterestsMenuUpdated?.Invoke();
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
            foreach (CCUI_InterestButton button in currentlySelectedButtons)
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

        protected virtual void LoadSOsAndCreateButtons()
        {
            Quille.InterestSO[] theSOs = Resources.LoadAll<Quille.InterestSO>(PathConstants.SO_PATH_INTERESTS);
            int noOfSOs = theSOs.Length;

            theButtons = new CCUI_InterestButton[noOfSOs];
            theButtonsDict = new SerializedDictionary<ScriptableObject, CCUI_InterestButton>();
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

                    theButtonsTransforms[i] = Instantiate<Transform>(interestButtonPrefab, initialButtonTransform);
                    theButtons[i] = theButtonsTransforms[i].GetComponent<CCUI_InterestButton>();
                    theButtonsDict.Add(theSOs[i], theButtons[i]);

                    // Set the button's parent and positione.
                    RectTransform thisButtonsRectTransform = theButtonsTransforms[i].GetComponent<RectTransform>();
                    thisButtonsRectTransform.SetParent(allButtonsContainerTransform, false);
                    thisButtonsRectTransform.anchoredPosition = new Vector2(initialXPos + k * distanceBetweenButtons, initialYPos + j * rowShift);

                    // Initialize the button.
                    theButtons[i].Init(theSOs[i]);

                    // Subscribe to its event.
                    theButtons[i].InterestButtonUpdated += OnInterestButtonUpdated;

                    i++;
                }
            }
        }

        protected virtual void Init()
        {
            FetchComponents();
            LoadSOsAndCreateButtons();
        }


        // BUILT IN
        void Start()
        {
            Init();
        }
    }
}

