 using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QuilleUI
{
    public class LiveUI_NeedBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Display element for an individual need gauge, used for both basic and subjective needs.


        // VARIABLES
        [Header("Parameters")]
        [SerializeField] private Gradient colourByFill;

        [Header("Components and References")]
        [SerializeField] private TMPro.TextMeshProUGUI myNeedTitle;
        [SerializeField] private Image myNeedIcon;
        [SerializeField] private Image myFillImage;
        [SerializeField] private GameObject myInfoBox; // Should this be a transform instead?
        [SerializeField] private TMPro.TextMeshProUGUI myInfoText;

        private Quille.BasicNeedSO myTargetNeedSO;
        private float currentFillLevelAsPercentage;



        // METHODS

        // INIT
        private void FetchComponents()
        {
            myNeedTitle = transform.Find("NeedTitle").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            myNeedIcon = transform.Find("NeedIcon").gameObject.GetComponent<Image>();
            myFillImage = transform.Find("NeedBarFill").gameObject.GetComponent<Image>();

            myInfoBox = transform.Find("NeedInfoBox").gameObject;
            myInfoText = myInfoBox.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }

        public void Init(Quille.BasicNeed targetNeed)
        {
            FetchComponents();

            // Set up shop with the target need.
            myTargetNeedSO = targetNeed.NeedSO;
            gameObject.name = string.Format("NeedBar_{0}", myTargetNeedSO.NeedName);
            myNeedTitle.text = myTargetNeedSO.NeedName;
            myNeedIcon.sprite = myTargetNeedSO.NeedIcon;

            UpdateFill(targetNeed.LevelCurrent, targetNeed.LevelCurrentAsPercentage);

            myInfoBox.SetActive(false);
        }


        // UTILITY
        public void UpdateFill(float newFillLevel, float newFillLevelAsPercentage)
        {
            currentFillLevelAsPercentage = newFillLevelAsPercentage;

            myFillImage.fillAmount = currentFillLevelAsPercentage;
            myFillImage.color = colourByFill.Evaluate(currentFillLevelAsPercentage);

            myInfoText.text = string.Format("{0} : {1}%", myTargetNeedSO.NeedName, newFillLevel);
        }


        // EVENT LISTENERS
        public void OnTargetNeedUpdated(Quille.BasicNeedSO needIdentity, float needLevelCurrent, float needLevelCurrentAsPercentage)
        {
            Debug.Log(string.Format("In NeedBar's event for {0}, now at {1}%.", needIdentity, needLevelCurrent));

            UpdateFill(needLevelCurrent, needLevelCurrentAsPercentage);
        }


        // BUILT IN
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            myInfoBox.SetActive(true);

            // TODO: move the info box to/over the cursor's location?
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            myInfoBox.SetActive(false);
        }
    }

}