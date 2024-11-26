using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace QuilleUI
{
    public class PersonalityAxesMenu : MonoBehaviour
    {
        // Test setup for a character creator's personality axe UI.
        // Procedurally populated from existing PersonalyAxeSOs.
        // Handles the local saving and loading of an incomplete character using Person's Json serialization system.


        // VARIABLES

        [SerializeField] private Transform prefab;

        [SerializeField] private float shiftDown = 80f;

        [SerializeField] private Canvas ownerCanvas;
        [SerializeField] private Transform initialTransform;
        [SerializeField] private RectTransform initialRectTransform;

        [SerializeField] private PersonalityAxeSlider[] theSliders;
        private SerializedDictionary<Quille.PersonalityAxeSO, PersonalityAxeSlider> theSlidersDict; // Should these two be combined into one?
        [SerializeField] private Transform[] theSlidersTransforms;



        // METHODS

        // UTILITY

        // Get
        public KeyValuePair<Quille.PersonalityAxeSO, float>[] ReturnAxeSOValuePairs()
        {
            return theSliders.Select(slider => slider.MyAxeSOAndValue).ToArray();
        }
        public SerializedDictionary<Quille.PersonalityAxeSO, float> ReturnAxeSOValueDict()
        {
            SerializedDictionary<Quille.PersonalityAxeSO, float> AxeSoValueDict = new SerializedDictionary<Quille.PersonalityAxeSO, float>();
            foreach (PersonalityAxeSlider slider in theSliders)
            {
                AxeSoValueDict.Add(slider.MyPersonalityAxeSO, slider.MySliderValue);
            }
            return AxeSoValueDict;
        }

        // Set
        public void SetAxeSOValuePairs(KeyValuePair<Quille.PersonalityAxeSO, float>[] keyValuePairs)
        {
            foreach (KeyValuePair<Quille.PersonalityAxeSO, float> keyValuePair in keyValuePairs)
            {
                theSlidersDict[keyValuePair.Key].MySliderValue = keyValuePair.Value;
            }
        }
        public void SetAxeSOValuePairs(SerializedDictionary<Quille.PersonalityAxeSO, float> sourceDict)
        {
            foreach (KeyValuePair<Quille.PersonalityAxeSO, float> keyValuePair in sourceDict)
            {
                theSlidersDict[keyValuePair.Key].MySliderValue = keyValuePair.Value;
            }
        }

        // Randomize
        public void RandomizeValues()
        {
            foreach (PersonalityAxeSlider slider in theSliders)
            {
                slider.MySliderValue = Random.Range(-1f, 1f);
            }
        }


        // INIT
        private void FetchComponents()
        {
            ownerCanvas = GetComponentInParent<Canvas>();
            initialTransform = gameObject.transform;
            initialRectTransform = (RectTransform)initialTransform;
        }
        private void Init()
        {
            FetchComponents();

            Quille.PersonalityAxeSO[] personalityAxes = Resources.LoadAll<Quille.PersonalityAxeSO>(PathConstants.SO_PATH_PERSONALITYAXES);
            int nofOfAxes = personalityAxes.Length;

            theSliders = new QuilleUI.PersonalityAxeSlider[nofOfAxes];
            theSlidersDict = new SerializedDictionary<Quille.PersonalityAxeSO, PersonalityAxeSlider>();
            theSlidersTransforms = new Transform[nofOfAxes];

            for (int i = 0; i < nofOfAxes; i++)
            {
                theSlidersTransforms[i] = Instantiate<Transform>(prefab, ownerCanvas.transform);
                theSliders[i] = theSlidersTransforms[i].GetComponent<QuilleUI.PersonalityAxeSlider>();
                theSlidersDict.Add(personalityAxes[i], theSliders[i]);

                // Set the slider's personalityAxe GO.
                theSliders[i].MyPersonalityAxeSO = personalityAxes[i];
                theSliders[i].Init();

                // Name the game object.
                string axeName = personalityAxes[i].AxeName;
                theSlidersTransforms[i].name = string.Format("PersonalityAxe_{0}", axeName);

                // Change its parent & position.
                theSlidersTransforms[i].SetParent(this.transform);
                RectTransform myRectTransform = theSlidersTransforms[i].GetComponent<RectTransform>();
                myRectTransform.anchoredPosition = new Vector2(initialRectTransform.anchoredPosition.x, i * shiftDown);
            }
        }


        // BUILT IN
        void Start()
        {
            Init();
        }
    }
}
