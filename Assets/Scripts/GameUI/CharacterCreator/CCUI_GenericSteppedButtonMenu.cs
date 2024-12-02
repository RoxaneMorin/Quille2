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

        [SerializeField] protected Transform steppedButtonPrefab;
        [SerializeField] protected int countPerRow;
        [SerializeField] protected float rowShift;
        [SerializeField] protected float containerPadding;

        [Header("References")]
        [SerializeField] protected Canvas ownerCanvas;
        [SerializeField] protected RectTransform selectedButtonsContainerTransform;
        [SerializeField] protected RectTransform allButtonsContainerTransform;
        [SerializeField] protected Transform initialButtonTransform;

        [SerializeField] protected CCUI_GenericSteppedButton[] theButtons;
        [SerializeField] protected SerializedDictionary<ScriptableObject, CCUI_GenericSteppedButton> theButtonsDict;
        [SerializeField] protected Transform[] theButtonsTransforms;



        // METHODS

        // EVENT LISTENERS
        public virtual void OnSteppedButtonUpdated(CCUI_GenericSteppedButton theUpdatedButton)
        {

        }

        // UTILITY
        //abstract public void RandomizeValues();
        // randomise both selection & values?
        //abstract public void ResetValues();
        // unselect all?


        // INIT
        protected virtual void FetchComponents()
        {
            ownerCanvas = GetComponentInParent<Canvas>();

            if (!initialButtonTransform)
            {
                initialButtonTransform = gameObject.transform;
            }

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

            float containerWidthPadded = allButtonsContainerTransform.rect.width - (2 * containerPadding);
            float containerHalfWidthPadded = containerWidthPadded / 2;

            float initialXPos = allButtonsContainerTransform.anchoredPosition.x - containerHalfWidthPadded;
            float initialYPos = ((RectTransform)initialButtonTransform).anchoredPosition.y;
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
                    thisButtonsRectTransform.anchoredPosition = new UnityEngine.Vector2(initialXPos + k * distanceBetweenButtons, initialYPos + j * rowShift);

                    // Initialize the button.
                    theButtons[i].Init(theSOs[i]);

                    // Subscribe to its event.

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