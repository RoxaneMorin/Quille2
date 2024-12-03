using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace QuilleUI
{
    public class CCUI_PersonalityAxesMenu : MonoBehaviour
    {
        // Test setup for a character creator's personality axe UI.
        // Procedurally populated from existing PersonalyAxeSOs.


        // VARIABLES
        [Header("Parameters")]
        [SerializeField] private Transform sliderPrefab;
        [SerializeField] private float shiftDown = 80f;

        [Header("References")]
        [SerializeField] private Canvas ownerCanvas;
        [SerializeField] protected RectTransform allSlidersContainerTransform;
        [SerializeField] private Transform initialSliderTransform;

        [Header("The Stuff")]
        [SerializeField] private CCUI_PersonalityAxeSlider[] theSliders;
        private SerializedDictionary<Quille.PersonalityAxeSO, CCUI_PersonalityAxeSlider> theSlidersDict;
        [SerializeField] private Transform[] theSlidersTransforms;


        // PROPERTIES
        public float GetSliderValueFor(Quille.PersonalityAxeSO thePASO)
        {
            return theSlidersDict[thePASO].MySliderValue;
        }

        public SerializedDictionary<Quille.PersonalityAxeSO, float> GetSlidersSOsAndValues()
        {
            return theSliders.ToSerializedDictionary(slider => slider.MyPersonalityAxeSO, slider => slider.MySliderValue);
        }

        public void SetSliderValuesFromSOFloatDict(SerializedDictionary<Quille.PersonalityAxeSO, float> sourceDict)
        {
            foreach (CCUI_PersonalityAxeSlider slider in theSliders)
            {
                if (sourceDict.ContainsKey(slider.MyPersonalityAxeSO))
                {
                    slider.MySliderValueWithoutNotify = sourceDict[slider.MyPersonalityAxeSO];
                }
                else
                {
                    slider.MySliderValueWithoutNotify = 0f;
                }
            }
        }


        // EVENTS
        public event PersonalityAxeSliderUpdate PersonalityAxeSliderUpdated;
        public event PersonalityAxesMenuUpdate PersonalityAxesMenuUpdated;



        // METHODS

        // EVENT LISTENERS
        public void OnPersonalityAxeSliderUpdated(Quille.PersonalityAxeSO slidersPersonalityAxeSO)
        {
            PersonalityAxeSliderUpdated?.Invoke(slidersPersonalityAxeSO);
        }


        // UTILITY
        public void RandomizeValues()
        {
            foreach (CCUI_PersonalityAxeSlider slider in theSliders)
            {
                slider.RandomizeValue();
            }

            PersonalityAxesMenuUpdated?.Invoke();
        }

        public void ResetValues()
        {
            foreach (CCUI_PersonalityAxeSlider slider in theSliders)
            {
                slider.ResetValue();
            }

            PersonalityAxesMenuUpdated?.Invoke();
        }


        // INIT
        private void FetchComponents()
        {
            ownerCanvas = GetComponentInParent<Canvas>();

            if (!initialSliderTransform)
            {
                initialSliderTransform = gameObject.transform;
            }
            
        }
        private void Init()
        {
            FetchComponents();

            Quille.PersonalityAxeSO[] personalityAxes = Resources.LoadAll<Quille.PersonalityAxeSO>(PathConstants.SO_PATH_PERSONALITYAXES);
            int nofOfAxes = personalityAxes.Length;

            theSliders = new QuilleUI.CCUI_PersonalityAxeSlider[nofOfAxes];
            theSlidersDict = new SerializedDictionary<Quille.PersonalityAxeSO, CCUI_PersonalityAxeSlider>();
            theSlidersTransforms = new RectTransform[nofOfAxes];

            Vector2 initialSliderPosition = ((RectTransform)initialSliderTransform).anchoredPosition;

            for (int i = 0; i < nofOfAxes; i++)
            {
                theSlidersTransforms[i] = Instantiate<Transform>(sliderPrefab, initialSliderTransform);
                theSliders[i] = theSlidersTransforms[i].GetComponent<QuilleUI.CCUI_PersonalityAxeSlider>();
                theSlidersDict.Add(personalityAxes[i], theSliders[i]);

                //  Set the slider's parent & position.
                RectTransform thisSlidersRectTransform = theSlidersTransforms[i].GetComponent<RectTransform>();
                thisSlidersRectTransform.SetParent(allSlidersContainerTransform, false);
                thisSlidersRectTransform.anchoredPosition = initialSliderPosition + new Vector2(0, i * shiftDown);

                // Initialize it.
                theSliders[i].Init(personalityAxes[i]);

                // Subscribe to slider event.
                theSliders[i].PersonalityAxeSliderUpdated += OnPersonalityAxeSliderUpdated;
            }
        }


        // BUILT IN
        void Start()
        {
            Init();
        }
    }
}
