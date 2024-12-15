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

        [Header("References")]
        [SerializeField] protected Button myButton;
        [SerializeField] protected TMPro.TextMeshProUGUI myButtonsCaption;

        // stuff in this domain?



        // METHODS

        // INIT
        protected virtual void FetchComponents()
        {
            myButton = gameObject.GetComponent<Button>();
            myButtonsCaption = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
        }

        public virtual void Init(PersonalityItemDomainSO sourceSO)
        {
            FetchComponents();

            mySO = sourceSO;
            myButtonsCaption.text = mySO.DomainName;
            myButton.image.color = mySO.DomainColour;

            // Collect the stuff in this domain?
        }
    }
}
