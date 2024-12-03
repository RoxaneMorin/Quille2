using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace QuilleUI 
{
    public class CCUI_PersonalityAxeSlider : MonoBehaviour
    {
        // Test setup for a character creator's individual personality axe UI.
        // Includes little cosmetic effects ~~


        // VARIABLES
        [SerializeField] Quille.PersonalityAxeSO myPersonalityAxeSO;

        [Header("Resources")]
        [SerializeField] Slider mySlider;
        [SerializeField] Graphic myHandle;

        [SerializeField] Image myIconLeft;
        [SerializeField] Image myIconRight;
        [SerializeField] TMPro.TextMeshProUGUI myCaptionLeft;
        [SerializeField] TMPro.TextMeshProUGUI myCaptionRight;
        [SerializeField] Gradient myColourGradient;

        // Snapping
        [Header("Snapping")]
        [SerializeField] bool stepValues = true;
        [SerializeField, Tooltip("Rounds to 100/this value.")] int roundingValue = 8;


        // PARAMETERS
        internal Quille.PersonalityAxeSO MyPersonalityAxeSO { get { return myPersonalityAxeSO; } set { myPersonalityAxeSO = value; } }

        internal Slider MySlider { get { return mySlider; } }

        internal float MySliderValue { get { return mySlider.value; } set { mySlider.value = value; } }
        internal float MySliderValueWithoutNotify { set { mySlider.SetValueWithoutNotify(value);
                                                          StepValue();
                                                          SetColourByValue();
                                                  } }

        internal KeyValuePair<Quille.PersonalityAxeSO, float> MyAxeSOAndValue { get { return new KeyValuePair<Quille.PersonalityAxeSO, float>(MyPersonalityAxeSO, MySliderValue); } }


        // EVENTS
        public event PersonalityAxeSliderUpdate PersonalityAxeSliderUpdated;



        // METHODS

        // EVENT LISTENERS
        public void OnSliderValueUpdated()
        {
            StepValue();
            SetColourByValue();

            PersonalityAxeSliderUpdated?.Invoke(myPersonalityAxeSO);
        }


        // UTILITY
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

        public void ResetValue()
        {
            MySliderValueWithoutNotify = 0;
            SetColourByValue();
        }
        public void RandomizeValue()
        {
            MySliderValueWithoutNotify = RandomExtended.RangeStepped(-1f, 1f, 0.125f);
            SetColourByValue();
        }
        

        // INIT
        void FetchComponents()
        {
            mySlider = gameObject.GetComponent<Slider>();
            myHandle = mySlider.targetGraphic;

            myIconLeft = transform.Find("IconLeft").gameObject.GetComponent<Image>();
            myIconRight = transform.Find("IconRight").gameObject.GetComponent<Image>();
            myCaptionLeft = transform.Find("CaptionLeft").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            myCaptionRight = transform.Find("CaptionRight").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

        }
        public void Init(Quille.PersonalityAxeSO sourcePersonalityAxeSO)
        {
            myPersonalityAxeSO = sourcePersonalityAxeSO;

            FetchComponents();

            if (myPersonalityAxeSO)
            {
                myIconLeft.sprite = myPersonalityAxeSO.axeIconLeft;
                myIconRight.sprite = myPersonalityAxeSO.axeIconRight;

                myCaptionLeft.text = myPersonalityAxeSO.AxeNameLeft;
                myCaptionRight.text = myPersonalityAxeSO.AxeNameRight;

                gameObject.name = string.Format("PersonalityAxe_{0}", myPersonalityAxeSO.AxeName);
            }
        }


        // BUILT IN
        void Start()
        {
            //Init(null);
        }
    }
}

