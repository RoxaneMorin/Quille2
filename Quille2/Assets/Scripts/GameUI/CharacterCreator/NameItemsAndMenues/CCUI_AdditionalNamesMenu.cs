using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace QuilleUI
{
    public class CCUI_AdditionalNamesMenu : MonoBehaviour
    {
        // VARIALBES/PARAMS
        [Header("Parameters")]
        [SerializeField] private Transform inputFieldPrefab;
        [SerializeField] private float initialHeight = 120f;
        [SerializeField] private float shiftUp = 80f;

        [Header("References")]
        [SerializeField] private Canvas ownerCanvas;
        [SerializeField] private RectTransform myRectTransform;
        [SerializeField] private RectTransform inputFieldContainerTransform;
        //[SerializeField] private Button addAdditionalNameButton;
        [SerializeField] private Transform initialInputFieldTransform;
        
        [Header("The Stuff")]
        [SerializeField] private int currentNumberOfNames;
        [SerializeField] private List<CCUI_AdditionalNameInputField> theInputFields;
        [SerializeField] private List<Transform> theInputFieldsTransforms;


        // PROPERTIES
        public List<string> InputFieldsAdditionalNames 
        { 
            get
            { 
                return theInputFields == null ? null : theInputFields.Select(inputField => inputField.MyInputFieldText).Where(name => !string.IsNullOrWhiteSpace(name)).ToList(); 
            }
            set
            {
                RecreateNameInputFieldsFromNameList(value);
            }
        }


        // EVENTS
        public event AdditionalNameMenuUpdate AdditionalNameMenuUpdated;



        // METHODS

        // EVENT LISTENERS
        private void OnAdditionalNameInputFieldUpdated(CCUI_AdditionalNameInputField relevantInputField)
        {
            AdditionalNameMenuUpdated?.Invoke();
        }
        private void OnAdditionalNameInputFieldDeleted(CCUI_AdditionalNameInputField targetInputField)
        {
            DeleteSpecificNameInputField(targetInputField);

            AdditionalNameMenuUpdated?.Invoke();
        }


        // UTILITY
        public void Activate()
        {
            gameObject.SetActive(true);

            // Create new if emppty.
            if (theInputFields.Count == 0)
            {
                AddNameInputField();
            }
        }
        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void CreateNameInputField(int index, string initialText = null)
        {
            // Create the new input field.
            Vector2 initialInputFieldPosition = ((RectTransform)initialInputFieldTransform).anchoredPosition;

            theInputFieldsTransforms.Add(Instantiate<Transform>(inputFieldPrefab, initialInputFieldTransform));
            theInputFields.Add(theInputFieldsTransforms[index].GetComponent<CCUI_AdditionalNameInputField>());

            // Set the field's parent and position.
            RectTransform thisInputFieldsRectTransform = theInputFieldsTransforms[index].GetComponent<RectTransform>();
            thisInputFieldsRectTransform.SetParent(inputFieldContainerTransform, false);
            thisInputFieldsRectTransform.anchoredPosition = initialInputFieldPosition + new Vector2(0, index * shiftUp);

            // Init and subscribe to its events.
            theInputFields[index].Init(initialText);
            theInputFields[index].OnAdditionalNameInputFieldUpdated += OnAdditionalNameInputFieldUpdated;
            theInputFields[index].OnAdditionalNameDeleted += OnAdditionalNameInputFieldDeleted;
        }
        private void RelayoutExistingNameInputFields()
        {
            // Adjust the menu's height based on the number of input fields.
            float newHeight = initialHeight + Mathf.Abs(shiftUp) * (currentNumberOfNames - 1);
            myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

            Vector2 initialInputFieldPosition = ((RectTransform)initialInputFieldTransform).anchoredPosition;

            if (initialInputFieldTransform)
            {
                for (int i = 0; i < currentNumberOfNames; i++)
                {
                    // Set the field's parent and position.
                    RectTransform thisInputFieldsRectTransform = theInputFieldsTransforms[i].GetComponent<RectTransform>();
                    thisInputFieldsRectTransform.SetParent(inputFieldContainerTransform, false);
                    thisInputFieldsRectTransform.anchoredPosition = initialInputFieldPosition + new Vector2(0, i * shiftUp);
                }
            }
        }

        private void AddNameInputField()
        {
            currentNumberOfNames += 1;

            if (initialInputFieldTransform)
            {
                int index = currentNumberOfNames - 1;

                // Adjust the menu's height based on the number of input fields.
                float newHeight = initialHeight + Mathf.Abs(shiftUp) * (index);
                myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

                // Create the lists if they don't yet exist.
                if (theInputFields == null)
                {
                    theInputFields = new List<CCUI_AdditionalNameInputField>();
                }
                if (theInputFieldsTransforms == null)
                {
                    theInputFieldsTransforms = new List<Transform>();
                }

                CreateNameInputField(index);
            }
        }
        public void DeleteSpecificNameInputField(CCUI_AdditionalNameInputField targetInputField)
        {
            currentNumberOfNames -= 1;

            theInputFields.Remove(targetInputField);
            theInputFieldsTransforms.Remove(targetInputField.transform);
            Destroy(targetInputField.gameObject);

            // Close menu if emppty.
            if (currentNumberOfNames == 0)
            {
                Deactivate();
            }
            else
            {
                // Resize window and shift other input fields down.
                RelayoutExistingNameInputFields();
            }
        }

        private void RecreateNameInputFieldsFromNameList(List<string> sourceNames)
        {
            ClearAllNameInputFields();

            currentNumberOfNames = (sourceNames == null || sourceNames.Count == 0)  ? 1 : sourceNames.Count;

            if (initialInputFieldTransform)
            {
                // Adjust the menu's height based on the number of input fields.
                float newHeight = initialHeight + Mathf.Abs(shiftUp) * (currentNumberOfNames - 1);
                myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

                // Create the lists if they don't yet exist.
                if (theInputFields == null)
                {
                    theInputFields = new List<CCUI_AdditionalNameInputField>();
                }
                if (theInputFieldsTransforms == null)
                {
                    theInputFieldsTransforms = new List<Transform>();
                }

                if (currentNumberOfNames == 1)
                {
                    CreateNameInputField(0);
                }
                else
                {
                    for (int i = 0; i < currentNumberOfNames; i++)
                    {
                        CreateNameInputField(i, sourceNames[i]);
                    }
                }
            }
        }
        private void ClearAllNameInputFields()
        {
            currentNumberOfNames = 0;

            foreach (CCUI_AdditionalNameInputField inputField in theInputFields)
            {
                Destroy(inputField.gameObject);
            }

            theInputFields = null;
            theInputFieldsTransforms = null;
        }


        // INIT
        private void FetchComponents()
        {
            ownerCanvas = GetComponentInParent<Canvas>();
            myRectTransform = GetComponent<RectTransform>();
            //addAdditionalNameButton = GetComponentInChildren<Button>();
        }
        private void Init()
        {
            FetchComponents();
        }


        // BUILT IN
        private void Start()
        {
            Init();
        }
    }
}
