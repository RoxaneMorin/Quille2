using AYellowpaper.SerializedCollections;
using Quille;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace QuilleUI
{
    public class CCUI_DomainFilter : MonoBehaviour
    {
        // UI element that filters the item associated with its domain upon click.


        // VARIABLES
        [SerializeField] protected PersonalityItemDomainSO mySO;
        [SerializeField] protected bool isActive;

        [Header("References")]
        [SerializeField] protected Button myButton;
        [SerializeField] protected TMPro.TextMeshProUGUI myButtonsCaption;
        //[SerializeField] protected CCUI_GenericSelectableButton[] buttonsInMyDomain;


        // PROPERTIES
        public PersonalityItemDomainSO MyDomain { get { return mySO; } }


        // EVENTS
        public event DomainFilterUpdate DomainFilterClicked;


        // METHODS

        // EVENT RECEIVERS
        public void OnDomainFilterClicked()
        {
            if (!isActive)
            {
                isActive = true;
                //myButtonsCaption.fontStyle = TMPro.FontStyles.UpperCase;
            }
            else
            {
                isActive = false;
                //myButtonsCaption.fontStyle = TMPro.FontStyles.LowerCase;
            }

            DomainFilterClicked?.Invoke(this);
        }

        // INIT
        protected virtual void FetchComponents()
        {
            myButton = gameObject.GetComponent<Button>();
            myButtonsCaption = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
        }

        public virtual void Init(PersonalityItemDomainSO sourceSO, SerializedDictionary<ScriptableObject, CCUI_GenericSelectableButton> sourceButtonsDict)
        {
            FetchComponents();

            mySO = sourceSO;
            myButtonsCaption.text = mySO.DomainName;
            myButton.image.color = mySO.DomainColour;

            //buttonsInMyDomain = mySO.ItemsInThisDomain.Select(item => sourceButtonsDict[item]).ToArray();

            gameObject.name = string.Format("DomainFilter_{0}", mySO.DomainName);
        }
    }
}
