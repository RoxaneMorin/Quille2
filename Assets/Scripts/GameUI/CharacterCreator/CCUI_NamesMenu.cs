using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace QuilleUI
{
    public class CCUI_NamesMenu : MonoBehaviour
    {
        // Test setup for the naming component of the character creation UI.


        // VARIABLES
        [Header("References")]
        [SerializeField] private TMP_InputField inputFieldFirstName;
        [SerializeField] private TMP_InputField inputFieldLastName;
        [SerializeField] private TMP_InputField inputFieldNickname;


        // PROPERTIES
        public string InputFieldFirstName { get { return inputFieldFirstName.text; } set { inputFieldFirstName.SetTextWithoutNotify(value); } }
        public string InputFieldLastName { get { return inputFieldLastName.text; } set { inputFieldLastName.SetTextWithoutNotify(value); } }
        public string InputFieldNickname { get { return inputFieldNickname.text; } set { inputFieldNickname.SetTextWithoutNotify(value); } }


        // EVENTS
        public event NameMenuUpdate NameMenuUpdated;



        // METHODS

        // UTILITY
        public void OnNameMenuUpdated()
        {
            NameMenuUpdated?.Invoke();
        }
    }
}