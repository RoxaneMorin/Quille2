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

        [SerializeField] Slider mySlider;
        [SerializeField] Image myIconLeft;
        [SerializeField] Image myIconRight;
        [SerializeField] TMPro.TextMeshProUGUI myCaptionLeft;
        [SerializeField] TMPro.TextMeshProUGUI myCaptionRight;


        // PARAMETERS
        public Quille.PersonalityAxeSO MyPersonalityAxeSO { get { return myPersonalityAxeSO; } set { myPersonalityAxeSO = value; } }

        public float MySliderValue { get { return mySlider.value; } }

        public (Quille.PersonalityAxeSO, float) MyAxeSOAndValue { get { return (MyPersonalityAxeSO, MySliderValue); } }



        // CONSTRUCTOR
        public PersonalityAxeSlider(Quille.PersonalityAxeSO correspondingPersonalityAxe)
        {
            myPersonalityAxeSO = correspondingPersonalityAxe;
            Init();
        }



        // METHODS

        // BUILT IN
        void Start()
        {
            //Init();
        }

        // INIT
        void FetchComponents()
        {
            mySlider = gameObject.GetComponent<Slider>();
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

        // UTILITY
        public void RoundValue()
        {
            float[] specificValues = { -0.75f, -0.25f, 0.25f, 0.75f };
            float temp = Mathf.Round(mySlider.value * 20) / 20;

            mySlider.value = specificValues.Contains(temp) ? temp : Mathf.Round(mySlider.value * 10) / 10;
        }
    }
}

