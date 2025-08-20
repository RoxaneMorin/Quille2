using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuilleUI
{
    public class CCUI_AdditionalNameInputField : MonoBehaviour
    {
        // VARIALBES/PARAMS
        [Header("References")]
        [SerializeField] private TMPro.TMP_InputField myInputField;
        [SerializeField] private Button myDeletionButton;


        // PROPERTIES
        public string MyInputFieldText { get { return myInputField.text; } set { myInputField.text = value; } }


        // EVENTS
        public event AdditionalNameInputFieldUpdate OnAdditionalNameInputFieldUpdated;
        public event AdditionalNameDeleted OnAdditionalNameDeleted;



        // METHODS

        // EVENT LISTENERS
        public void OnInputFieldUpdated()
        {
            OnAdditionalNameInputFieldUpdated?.Invoke(this);
        }
        public void OnDeletionButtonClicked()
        {
            OnAdditionalNameDeleted?.Invoke(this);
        }


        // INIT
        private void FetchComponents()
        {
            myInputField = GetComponent<TMPro.TMP_InputField>();
            myDeletionButton = GetComponentInChildren<Button>();
        }
        public void Init(string initialText = null)
        {
            FetchComponents();

            gameObject.name = "AdditionalNameInputField";
            if (initialText != null)
            {
                MyInputFieldText = initialText;
            }
        }
    }
}
