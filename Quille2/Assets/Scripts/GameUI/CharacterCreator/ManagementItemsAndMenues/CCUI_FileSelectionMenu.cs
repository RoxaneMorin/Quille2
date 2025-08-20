using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuilleUI
{
    public class CCUI_FileSelectionMenu : MonoBehaviour
    {
        // Test set up for the character creation menu's load file UI.
        // TODO: make a universal version that isn't bound to characters in particular?


        // VARIABLES
        [Header("Parameters")]
        [SerializeField] private Transform buttonPrefab;
        [SerializeField] private float initialHeight = 120f;
        [SerializeField] private float shiftDown = 80f;

        [Header("References")]
        [SerializeField] private Canvas ownerCanvas;
        [SerializeField] private RectTransform myRectTransform;
        [SerializeField] private RectTransform buttonContainerTransform;
        [SerializeField] private Transform initialButtonTransform;

        [Header("The Stuff")]
        [SerializeField] private CCUI_FileSelectionButton[] theButtons;
        [SerializeField] private Transform[] theButtonsTransforms;


        // EVENTS
        public event FilePicked OnFilePicked;



        // METHODS

        // EVENT LISTENERS
        private void OnFileToSelectPicked(string filePath)
        {
            OnFilePicked?.Invoke(filePath);
            DeactivateAndClear();
        }

        // UTILITY
        public void ActivateAndFill()
        {
            gameObject.SetActive(true);
            Init();
        }
        public void DeactivateAndClear()
        {
            foreach (CCUI_FileSelectionButton button in theButtons)
            {
                Destroy(button.gameObject);
            }
            gameObject.SetActive(false);
        }


        // INIT
        private void FetchComponents()
        {
            ownerCanvas = GetComponentInParent<Canvas>();
            myRectTransform = GetComponent<RectTransform>();
        }
        private void Init()
        {
            FetchComponents();

            if (initialButtonTransform)
            {
                string[] filepaths = SerializationHelper.ReturnAllCharacterFilePathsAt(SaveType.Backpack);
                int noOfFilepaths = filepaths.Length;

                // Adjust the base menu's height based on the number of buttons.
                float newHeight = initialHeight + Mathf.Abs(shiftDown) * (noOfFilepaths - 1);
                myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

                // Create the buttons proper.
                theButtons = new CCUI_FileSelectionButton[noOfFilepaths];
                theButtonsTransforms = new RectTransform[noOfFilepaths];

                Vector2 initialButtonPosition = ((RectTransform)initialButtonTransform).anchoredPosition;

                for (int i = 0; i < noOfFilepaths; i++)
                {
                    theButtonsTransforms[i] = Instantiate<Transform>(buttonPrefab, initialButtonTransform);
                    theButtons[i] = theButtonsTransforms[i].GetComponent<CCUI_FileSelectionButton>();

                    //  Set the button's parent & position.
                    RectTransform thisButtonsRectTransform = theButtonsTransforms[i].GetComponent<RectTransform>();
                    thisButtonsRectTransform.SetParent(buttonContainerTransform, false);
                    thisButtonsRectTransform.anchoredPosition = initialButtonPosition + new Vector2(0, i * shiftDown);

                    // Initialize the button & subscribe to its event.
                    theButtons[i].Init(filepaths[i]);
                    theButtons[i].OnFilePicked += OnFileToSelectPicked;
                }
            }
        }

        private void Start()
        {
            //Init();
            gameObject.SetActive(false);
        }
    }
}

