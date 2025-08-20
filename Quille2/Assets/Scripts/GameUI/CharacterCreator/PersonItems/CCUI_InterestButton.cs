using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Quille;

namespace QuilleUI
{
    public class CCUI_InterestButton : CCUI_GenericSelectableButton
    {
        // Test setup for a character creator's indivudal interest UI.
        // When selected, a slider component will appear.


        // VARIABLES
        [Header("References")]
        [SerializeField] protected SliderRadial mySlider;
        [SerializeField] protected Graphic myHandle;

        [Header("Resources")]
        [SerializeField] Gradient myColourGradient;

        [Header("Snapping")]
        [SerializeField] bool stepValues = true;
        [SerializeField, Tooltip("Rounds to 100/this value.")] int roundingValue = 8;


        // TODO: put together a radial slider, it'll look better.
        // TODO: colour the slider like the steppedbuttons' frames are?


        // PARAMETERS
        internal InterestSO MyInterestSO { get { return (InterestSO)mySO; } set { mySO = value; } }
        internal KeyValuePair<Quille.InterestSO, float> MyAxeSOAndValue { get { return new KeyValuePair<InterestSO, float>(MyInterestSO, MySliderValue); } }

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
        public override event SelectableButtonUpdate SelectableButtonUpdated;



        // METHODS

        // EVENT LISTENERS
        public override void OnButtonClicked()
        {
            // If the button is not selected, unselect it, and viseversa.
            if (!isSelected)
            {
                Select();
                SelectableButtonUpdated?.Invoke(this, true);
            }
            else
            {
                Unselect();
                SelectableButtonUpdated?.Invoke(this, true);
            }
        }
        public void OnInterestSliderUpdated()
        {
            StepValue();
            SetColourByValue();
            RegenerateCaption();

            SelectableButtonUpdated?.Invoke(this, false);
        }


        // UTILITY
        public override void Select()
        {
            Select(0);
        }
        public void Select(float value)
        {
            base.Select();

            MySliderValueWithoutNotify = value;
            mySlider.gameObject.SetActive(true);

            RegenerateCaption();
        }
        public override void Unselect()
        {
            base.Unselect();

            MySliderValueWithoutNotify = 0;
            mySlider.gameObject.SetActive(false);
        }

        protected override string MakeNewCaption()
        {
            return string.Format("{0} ({1})", MyInterestSO.ItemName, MySliderValue);
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
            myHandle.color = myColourGradient.Evaluate(mySlider.normalizedValue);
        }

        public virtual void RandomizeValueAndSelect()
        {
            // Bias in favour of positive results.
            if (RandomExtended.RangeInt(1, 3) > 1)
            {
                Select(RandomExtended.RangeStepped(0.125f, 1f, 0.125f));
            }
            else
            {
                Select(RandomExtended.RangeStepped(-1f, -0.125f, 0.125f));
            }
            
            SetColourByValue();
            RegenerateCaption();
        }
        public void ResetValue()
        {
            MySliderValueWithoutNotify = 0;

            SetColourByValue();
            RegenerateCaption();
        }


        // INIT
        protected override void FetchComponents()
        {
            base.FetchComponents();

            mySlider = gameObject.GetComponentInChildren<SliderRadial>(true);
            myHandle = mySlider.targetGraphic;
        }

        public override void Init(PersonalityItemSO sourceSO)
        {
            base.Init(sourceSO);

            if (mySO is InterestSO)
            {
                Quille.InterestSO myInterestSO = MyInterestSO;

                myIcon.sprite = myInterestSO.ItemIcon;
                myCaption.text = myInterestSO.ItemName;
                myDefaultCaption = myCaption.text;

                gameObject.name = string.Format("Interest_{0}", myInterestSO.ItemName);
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

