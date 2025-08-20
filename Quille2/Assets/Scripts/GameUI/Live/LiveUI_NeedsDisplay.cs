using AYellowpaper.SerializedCollections;
using Quille;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuilleUI
{
    public class LiveUI_NeedsDisplay : MonoBehaviour
    {
        // Menu/display for basic and subjective need. Responsible for creating the various need bars.



        // VARIABLES
        [Header("Parameters")]
        [SerializeField] private Transform basicNeedBarPrefab;
        [SerializeField] private Transform subjectiveNeedBarPrefabLeft;
        [SerializeField] private Transform subjectiveNeedBarPrefabRight;

        [SerializeField] private float basicNeedBarShift; // Horizontal?
        [SerializeField] private float subjectiveNeedBarShift; // Vertical?

        [Header("References")]
        [SerializeField] private Canvas ownerCanvas;
        [Space]
        [SerializeField] private RectTransform initialBasicNeedBarTransform;
        [SerializeField] private RectTransform initialSubjectiveNeedBarsTransform;
        [Space]
        [SerializeField] private Quille.Person_NeedController targetNeedController;

        [Header("The Stuff")]
        [SerializeField] private Transform[] basicNeedBarTransforms; // Are these useful?
        [SerializeField] private Transform[] subjectiveNeedBarTransforms;
        private Dictionary<Quille.BasicNeedSO, LiveUI_NeedBar> mappedNeedBars;



        // TODO: methods to switch the targetNeedController, unsubscribing to one person's events and subscribing to a new one's.
        // TODO: create without initializing the bars themselves?


        // METHODS

        // INIT
        private void FetchComponents()
        {
            ownerCanvas = GetComponentInParent<Canvas>();

            if (initialBasicNeedBarTransform == null)
            {
                initialBasicNeedBarTransform = (RectTransform)transform;
            }
            if (initialSubjectiveNeedBarsTransform == null)
            {
                initialSubjectiveNeedBarsTransform = (RectTransform)transform;
            }
        }

        private void PopulateNeedBars()
        {
            basicNeedBarTransforms = new Transform[targetNeedController.MyBasicNeeds.Length];
            subjectiveNeedBarTransforms = new Transform[targetNeedController.MySubjectiveNeeds.Length * 2];
            mappedNeedBars = new Dictionary<Quille.BasicNeedSO, LiveUI_NeedBar>();

            for (int i = 0; i < targetNeedController.MyBasicNeeds.Length; i++)
            {
                // Create and init the need bar.
                basicNeedBarTransforms[i] = Instantiate<Transform>(basicNeedBarPrefab, ownerCanvas.transform);
                LiveUI_NeedBar tempBasicNeedBar = basicNeedBarTransforms[i].gameObject.GetComponent<LiveUI_NeedBar>();
                mappedNeedBars.Add(targetNeedController.MyBasicNeeds[i].NeedSO, tempBasicNeedBar);

                tempBasicNeedBar.Init(targetNeedController.MyBasicNeeds[i]);
                targetNeedController.OnBNLevelCurrentUpdate += OnTargetBNLevelUpdated;


                // Set its position.
                RectTransform thisNeedBarsRectTransform = basicNeedBarTransforms[i].GetComponent<RectTransform>();
                thisNeedBarsRectTransform.anchoredPosition = new Vector2(initialBasicNeedBarTransform.anchoredPosition.x + i * basicNeedBarShift, initialBasicNeedBarTransform.anchoredPosition.y);

                // TODO: more precise positioning.
            }



            for (int i = 0; i < targetNeedController.MySubjectiveNeeds.Length; i++)
            {
                int iPlus = i + targetNeedController.MySubjectiveNeeds.Length;

                // Create and init the need bars.
                subjectiveNeedBarTransforms[i] = Instantiate<Transform>(subjectiveNeedBarPrefabLeft, ownerCanvas.transform);
                LiveUI_NeedBar tempSubjectiveNeedBarLeft = subjectiveNeedBarTransforms[i].gameObject.GetComponent<LiveUI_NeedBar>();

                subjectiveNeedBarTransforms[iPlus] = Instantiate<Transform>(subjectiveNeedBarPrefabRight, ownerCanvas.transform);
                LiveUI_NeedBar tempSubjectiveNeedBarRight = subjectiveNeedBarTransforms[iPlus].gameObject.GetComponent<LiveUI_NeedBar>();

                mappedNeedBars.Add(targetNeedController.MySubjectiveNeeds[i].SubneedSOLeft, tempSubjectiveNeedBarLeft);
                mappedNeedBars.Add(targetNeedController.MySubjectiveNeeds[i].SubneedSORight, tempSubjectiveNeedBarRight);

                tempSubjectiveNeedBarLeft.Init(targetNeedController.MySubjectiveNeeds[i].SubneedLeft);
                tempSubjectiveNeedBarRight.Init(targetNeedController.MySubjectiveNeeds[i].SubneedRight);

                targetNeedController.OnSNLevelCurrentUpdate += OnTargetNeedLevelUpdated;


                // Set their positions.
                RectTransform leftNeedBarsRectTransform = subjectiveNeedBarTransforms[i].GetComponent<RectTransform>();
                RectTransform rightNeedBarsRectTransform = subjectiveNeedBarTransforms[iPlus].GetComponent<RectTransform>();

                leftNeedBarsRectTransform.anchoredPosition = new Vector2(initialSubjectiveNeedBarsTransform.anchoredPosition.x, initialSubjectiveNeedBarsTransform.anchoredPosition.y + i * subjectiveNeedBarShift);
                rightNeedBarsRectTransform.anchoredPosition = new Vector2(initialSubjectiveNeedBarsTransform.anchoredPosition.x + 300, initialSubjectiveNeedBarsTransform.anchoredPosition.y + i * subjectiveNeedBarShift);
            }
        }

        public void Init()
        {
            FetchComponents();

            if (targetNeedController != null)
            {
                PopulateNeedBars();
            }
        }



        // EVENT LISTENERS

        void OnTargetBNLevelUpdated(BasicNeedSO needIdentity, float needLevelCurrent, float needLevelCurrentAsPercentage)
        {
            mappedNeedBars[needIdentity].UpdateFill(needLevelCurrent, needLevelCurrentAsPercentage);
        }
        void OnTargetNeedLevelUpdated(SubjectiveNeedSO needIdentity, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage)
        {
            mappedNeedBars[needIdentity.NeedSOLeft].UpdateFill(needLevelCurrent.Item1, needLevelCurrentAsPercentage.Item1);
            mappedNeedBars[needIdentity.NeedSORight].UpdateFill(needLevelCurrent.Item2, needLevelCurrentAsPercentage.Item2);
        }




        // BUILT IN
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            //foreach (Quille.BasicNeed need in targetNeedController.MyBasicNeeds)
            //{
            //    Debug.Log(need);
            //}
        }


        //
    } 
}