using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuilleUI
{
    public class CCUI_FileSelectionButton : MonoBehaviour
    {
        // VARIALBES/PARAMS
        [Header("Parameters")]
        [SerializeField] private string myFilePath;
        [SerializeField] private string myFileName;

        [Header("References")]
        [SerializeField] private Button myButton;
        [SerializeField] private TMPro.TextMeshProUGUI myTextGUI;


        // EVENTS
        public event FilePicked OnFilePicked;



        // METHODS

        // EVENT LISTENERS
        public void OnFileSelectionButtonClicked()
        {
            OnFilePicked?.Invoke(myFilePath);
        }

        // INIT
        private void FetchComponents()
        {
            myButton = GetComponentInChildren<Button>();
            myTextGUI = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }
        public void Init(string sourceFilePath)
        {
            FetchComponents();

            myFilePath = sourceFilePath;
            myFileName = myFilePath.Split('\\').Last();

            gameObject.name = string.Format("FileButton_{0}", myFileName);
            myTextGUI.text = myFileName;
        }
    }
}


