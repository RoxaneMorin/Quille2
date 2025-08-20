using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuilleUI
{
    public class BasicNeedBars : MonoBehaviour
    {
        // Test setup for the creation and display of basic need bars.


        // VARIABLES
        
        [SerializeField] private Transform prefab;
        [SerializeField] private Gradient colourByFill;

        [SerializeField] private Canvas ownerCanvas;
        [SerializeField] private Quille.Person_NeedController targetNeedController;

        [SerializeField] private Transform[] needbarTransforms;
        [SerializeField] private Dictionary<Image, Quille.BasicNeed> needbarFillTargets;

        private bool init = false;


        // METHODS

        void CreateNeedBars()
        {
            //Debug.Log("Creating need bars");

            // Create the data structures.
            int numberOfNeeds = targetNeedController.MyBasicNeeds.Length;
            needbarTransforms = new Transform[numberOfNeeds];
            needbarFillTargets = new Dictionary<Image, Quille.BasicNeed>();

            for (int i = 0; i < numberOfNeeds; i++)
            {
                needbarTransforms[i] = Instantiate<Transform>(prefab, ownerCanvas.transform);

                // Name the root gameObject.
                string needName = targetNeedController.MyBasicNeeds[i].NeedName;
                needbarTransforms[i].name = string.Format("NeedBar_{0}", needName);

                // Change its position.
                RectTransform myRectTransform = needbarTransforms[i].GetComponent<RectTransform>();
                myRectTransform.position = new Vector3(myRectTransform.position.x, -60f + myRectTransform.position.y - 60f * i, myRectTransform.position.z);

                // Change the display text.
                TMPro.TextMeshProUGUI title = needbarTransforms[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                title.text = needName;

                // Fetch the fill bars, add them to the dictonary.
                Transform needBarFillTransform = needbarTransforms[i].Find("NeedBarFill");
                Image needBarFillImage = needBarFillTransform.GetComponent<Image>();

                needbarFillTargets.Add(needBarFillImage, targetNeedController.MyBasicNeeds[i]);
            }

            init = true;
        }

        void UpdateNeedBarFill()
        {
            foreach (KeyValuePair<Image, Quille.BasicNeed> need in needbarFillTargets)
            {
                need.Key.fillAmount = need.Value.LevelCurrentAsPercentage;
                need.Key.color = colourByFill.Evaluate(need.Key.fillAmount);

                // Manage colour gradient per need per character?
            }
        }

        //

        // Start is called before the first frame update
        void Start()
        {
            // Fetch component references.
            ownerCanvas = GetComponent<Canvas>();

            Invoke("CreateNeedBars", 3f);
        }

        // Update is called once per frame
        void Update()
        {
            if (init)
            {
                // TODO: do this via event instead?
                UpdateNeedBarFill();
            }
        }
    }
}

