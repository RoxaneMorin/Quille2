using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace QuilleUI
{
    public class CCUI_InterestButton : CCUI_GenericSelectableButton
    {
        // Test setup for a character creator's indivudal interest UI.
        // When selected, a slider component will appear.


        // VARIABLES
        [SerializeField] protected Quille.InterestSO myInterestSO;

        [Header("References")]
        [SerializeField] protected Slider mySlider;
        [SerializeField] protected Graphic myHandle;

        [Header("Resources")]
        [SerializeField] Gradient myColourGradient;
        [SerializeField] Vector2 myCaptionPositionUnselected;
        [SerializeField] Vector2 myCaptionPositionSelected;

        [Header("Snapping")]
        [SerializeField] bool stepValues = true;
        [SerializeField, Tooltip("Rounds to 100/this value.")] int roundingValue = 8;


        // TODO: put together a radial slider, it'll look better.
        // TODO: colour the slider like the steppedbuttons' frames are?


        // PARAMETERS
        internal Quille.InterestSO MyInterestSO { get { return myInterestSO; } set { myInterestSO = value; } }
        internal KeyValuePair<Quille.InterestSO, float> MyAxeSOAndValue { get { return new KeyValuePair<Quille.InterestSO, float>(MyInterestSO, MySliderValue); } }

        internal float MySliderValue { get { return mySlider.value; } set { mySlider.value = value; } }
        internal float MySliderValueWithoutNotify
        {
            set
            {
                mySlider.SetValueWithoutNotify(value);
                StepValue();
                SetColourByValue();
            }
        }


        // EVENTS
        public event InterestButtonUpdate InterestButtonUpdated;



        // METHODS

        // EVENT LISTENERS
        public override void OnButtonClicked()
        {
            // If the button is not selected, unselect it, and viseversa.

            if (!isSelected)
            {
                Select();
                InterestButtonUpdated?.Invoke(this, true);
            }
            else
            {
                Unselect();
                InterestButtonUpdated?.Invoke(this, true);
            }
        }
        public void OnInterestSliderUpdated()
        {
            StepValue();
            SetColourByValue();

            InterestButtonUpdated?.Invoke(this, false);
        }


        // UTILITY
        public override void Select()
        {
            Select(0);
        }
        public virtual void Select(float value)
        {
            isSelected = true;
            MySliderValueWithoutNotify = value;

            mySlider.gameObject.SetActive(true);
            myCaption.rectTransform.anchoredPosition = myCaptionPositionSelected;
        }
        public override void Unselect()
        {
            base.Unselect();
            MySliderValueWithoutNotify = 0;

            mySlider.gameObject.SetActive(false);
            myCaption.rectTransform.anchoredPosition = myCaptionPositionUnselected;
        }

        private void StepValue()
        {
            if (stepValues)
            {
                mySlider.SetValueWithoutNotify(Mathf.Round(mySlider.value * roundingValue) / roundingValue);
            }
        }
        private void SetColourByValue()
        {
            myHandle.color = myColourGradient.Evaluate((mySlider.value + 1) / 2);
        }

        public virtual void RandomizeValueAndSelect()
        {
            Select(RandomExtended.RangeStepped(-1f, 1f, 0.125f));
        }
        public void ResetValue()
        {
            MySliderValueWithoutNotify = 0;
            SetColourByValue();
        }


        // INIT
        protected override void FetchComponents()
        {
            base.FetchComponents();

            mySlider = gameObject.GetComponentInChildren<Slider>(true);
            myHandle = mySlider.targetGraphic;
        }

        public virtual void Init(Quille.InterestSO sourceSO)
        {
            base.Init();

            myInterestSO = sourceSO;

            if (myInterestSO)
            {
                myIcon.sprite = myInterestSO.interestIcon;
                myCaption.text = myInterestSO.InterestName;

                gameObject.name = string.Format("Interest_{0}", myInterestSO.InterestName);
            }

            mySlider.gameObject.SetActive(false);
        }


        // BUILT IN
        void Start()
        {
            //Init(myInterestSO);
        }
    }
}

