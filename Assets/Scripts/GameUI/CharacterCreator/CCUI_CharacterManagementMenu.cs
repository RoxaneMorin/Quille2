using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quille;

namespace QuilleUI
{
    public class CCUI_CharacterManagementMenu : MonoBehaviour
    {
        // VARIABLES
        [Header("References")]
        [SerializeField] private QuilleFactory_Temp quilleFactory; 
        [SerializeField] private CCUI_FileSelectionMenu fileSelectionMenu;



        // METHODS

        // EVENT LISTENERS
        private void OnCharacterFileToLoadPicked(string filePath)
        {
            quilleFactory.LoadCharacter(filePath);
        }

        // UI ACTIONS
        public void OnCreateNewCharacterClicked()
        {
            quilleFactory.CreateNewCharacter();
        }

        public void OnSaveCharacterClicked()
        {
            quilleFactory.SaveCharacter();
        }

        public void OnLoadCharacterClicked()
        {
            if (fileSelectionMenu.gameObject.activeSelf)
            {
                fileSelectionMenu.DeactivateAndClear();
            }
            else
            {
                fileSelectionMenu.ActivateAndFill();
            }
        }


        // INIT
        public void Init()
        {
            if (fileSelectionMenu)
            {
                fileSelectionMenu.OnFilePicked += OnCharacterFileToLoadPicked;
            }
        }


        // BUILT IN
        private void Start()
        {
            Init();
        }
    }
}

