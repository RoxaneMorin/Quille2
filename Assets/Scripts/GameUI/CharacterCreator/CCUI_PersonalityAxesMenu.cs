using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuilleUI
{
    public class CCUI_PersonalityAxesMenu : MonoBehaviour
    {
        // Test setup for a character creator's personality axe UI.
        // Procedurally populated from existing PersonalyAxeSOs.
        // Handles the local saving and loading of an incomplete character using Person's JSON serialization system.


        // VARIABLES

        [SerializeField] private Transform sliderPrefab;
        [SerializeField] private float shiftDown = 80f;

        [Header("References")]
        [SerializeField] private Canvas ownerCanvas;
        [SerializeField] private Transform baseSliderTransform;

        [SerializeField] private CCUI_PersonalityAxe[] theSliders;
        private SerializedDictionary<Quille.PersonalityAxeSO, CCUI_PersonalityAxe> theSlidersDict; // Should these two be combined into one?
        [SerializeField] private Transform[] theSlidersTransforms;


        // PROPERTIES
        public float ReturnSliderValueFromPASO(Quille.PersonalityAxeSO thePASO)
        {
            return theSlidersDict[thePASO].MySliderValue;
        }

        public SerializedDictionary<Quille.PersonalityAxeSO, float> ReturnAxeSOValueDict()
        {
            SerializedDictionary<Quille.PersonalityAxeSO, float> AxeSoValueDict = new SerializedDictionary<Quille.PersonalityAxeSO, float>();
            foreach (CCUI_PersonalityAxe slider in theSliders)
            {
                AxeSoValueDict.Add(slider.MyPersonalityAxeSO, slider.MySliderValue);
            }
            return AxeSoValueDict;
        }

        public void SetAxeSOValuePairs(SerializedDictionary<Quille.PersonalityAxeSO, float> sourceDict)
        {

            foreach (KeyValuePair<Quille.PersonalityAxeSO, CCUI_PersonalityAxe> keyValuePair in theSlidersDict)
            {
                if (sourceDict.ContainsKey(keyValuePair.Key))
                {
                    theSlidersDict[keyValuePair.Key].MySlider.SetValueWithoutNotify(sourceDict[keyValuePair.Key]);
                }
                else
                {
                    theSlidersDict[keyValuePair.Key].MySlider.SetValueWithoutNotify(0);
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
            foreach (CCUI_PersonalityAxe slider in theSliders)
            {
                slider.RandomizeValue();
            }

            PersonalityAxesMenuUpdated?.Invoke();
        }

        public void ResetValues()
        {
            foreach (CCUI_PersonalityAxe slider in theSliders)
            {
                slider.ResetValue();
            }

            PersonalityAxesMenuUpdated?.Invoke();
        }


        // INIT
        private void FetchComponents()
        {
            ownerCanvas = GetComponentInParent<Canvas>();

            if (!baseSliderTransform)
            {
                baseSliderTransform = gameObject.transform;
            }
            
        }
        private void Init()
        {
            FetchComponents();

            Quille.PersonalityAxeSO[] personalityAxes = Resources.LoadAll<Quille.PersonalityAxeSO>(PathConstants.SO_PATH_PERSONALITYAXES);
            int nofOfAxes = personalityAxes.Length;

            theSliders = new QuilleUI.CCUI_PersonalityAxe[nofOfAxes];
            theSlidersDict = new SerializedDictionary<Quille.PersonalityAxeSO, CCUI_PersonalityAxe>();
            theSlidersTransforms = new RectTransform[nofOfAxes];

            for (int i = 0; i < nofOfAxes; i++)
            {
                theSlidersTransforms[i] = Instantiate<Transform>(sliderPrefab, baseSliderTransform);
                theSliders[i] = theSlidersTransforms[i].GetComponent<QuilleUI.CCUI_PersonalityAxe>();
                theSlidersDict.Add(personalityAxes[i], theSliders[i]);

                // Set the slider's personalityAxe GO.
                theSliders[i].MyPersonalityAxeSO = personalityAxes[i];
                theSliders[i].Init();

                // Name the game object.
                string axeName = personalityAxes[i].AxeName;
                theSlidersTransforms[i].name = string.Format("PersonalityAxe_{0}", axeName);

                // Change its parent & position.
                RectTransform thisSlidersRectTransform = theSlidersTransforms[i].GetComponent<RectTransform>();
                thisSlidersRectTransform.anchoredPosition = new Vector2(((RectTransform)baseSliderTransform).anchoredPosition.x, i * shiftDown);

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
