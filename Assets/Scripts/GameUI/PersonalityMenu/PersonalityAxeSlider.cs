using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace QuilleUI 
{
    public class PersonalityAxeSlider : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] Quille.PersonalityAxeSO myPersonalityAxeSO;

        [Header("Resources")]
        [SerializeField] Slider mySlider;
        [SerializeField] Graphic myHandle;

        [SerializeField] Image myIconLeft;
        [SerializeField] Image myIconRight;
        [SerializeField] TMPro.TextMeshProUGUI myCaptionLeft;
        [SerializeField] TMPro.TextMeshProUGUI myCaptionRight;
        [SerializeField] Color myColourLeft;
        [SerializeField] Color myColourCenter;
        [SerializeField] Color myColourRight;

        // Snapping
        [Header("Snapping")]
        [SerializeField] bool stepped = true;
        [SerializeField, Tooltip("Rounds to 100/this value.")] int roundingValue = 8;
        [SerializeField] bool snapToSpecificValues = false;
        [SerializeField] float[] specificValues = { -0.75f, -0.25f, 0.25f, 0.75f };
        [SerializeField, Tooltip("Rounds to 100/this value.")] int specificRoundingValue = 20;



        // PARAMETERS
        internal Quille.PersonalityAxeSO MyPersonalityAxeSO { get { return myPersonalityAxeSO; } set { myPersonalityAxeSO = value; } }

        internal float MySliderValue { get { return mySlider.value; } set { mySlider.value = value; } }

        internal KeyValuePair<Quille.PersonalityAxeSO, float> MyAxeSOAndValue { get { return new KeyValuePair<Quille.PersonalityAxeSO, float>(MyPersonalityAxeSO, MySliderValue); } }



        // CONSTRUCTOR
        public PersonalityAxeSlider(Quille.PersonalityAxeSO correspondingPersonalityAxe)
        {
            myPersonalityAxeSO = correspondingPersonalityAxe;
            Init();
        }



        // METHODS

        // UTILITY
        public void RoundValue()
        {
            if (stepped)
            {
                if (snapToSpecificValues)
                {
                    float temp = Mathf.Round(mySlider.value * specificRoundingValue) / specificRoundingValue;
                    mySlider.value = specificValues.Contains(temp) ? temp : Mathf.Round(mySlider.value * roundingValue) / roundingValue;
                }
                else
                {
                    mySlider.value = Mathf.Round(mySlider.value * roundingValue) / roundingValue;
                }
            }
        }
        public void SetColourByValue()
        {
            if (mySlider.value < 0)
            {
                myHandle.color = Color.Lerp(myColourCenter, myColourLeft, Mathf.Abs(mySlider.value));
            }
            else
                myHandle.color = Color.Lerp(myColourCenter, myColourRight, mySlider.value);
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
        public void Init()
        {
            FetchComponents();

            if (myPersonalityAxeSO)
            {
                myIconLeft.sprite = myPersonalityAxeSO.axeIconLeft;
                myIconRight.sprite = myPersonalityAxeSO.axeIconRight;

                myCaptionLeft.text = myPersonalityAxeSO.AxeNameLeft;
                myCaptionRight.text = myPersonalityAxeSO.AxeNameRight;
            }
        }


        // BUILT IN
        void Start()
        {
            //Init();
        }
    }
}

