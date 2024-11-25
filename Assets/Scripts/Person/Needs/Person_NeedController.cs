using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using AYellowpaper.SerializedCollections;
using UnityEngine;


namespace Quille
{
    [System.Serializable]
    public partial class Person_NeedController : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] private BasicNeed[] myBasicNeeds;
        [SerializeField] private SubjectiveNeed[] mySubjectiveNeeds;

        // For use by external functions that only know of a NeedSO.
        [SerializeField, SerializedDictionary("BasicNeed SO", "Basic Need")] private SerializedDictionary<BasicNeedSO, BasicNeed> myBasicNeedsMapped;
        [SerializeField, SerializedDictionary("SubjectiveNeed SO", "Subjective Need")] private SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed> mySubjectiveNeedsMapped;

        // General need-related parameters.



        // PROPERTIES & GETTERS/SETTERS
        public BasicNeed[] MyBasicNeeds
        {
            get { return myBasicNeeds; }
        }
        public SubjectiveNeed[] MySubjectiveNeeds
        {
            get { return mySubjectiveNeeds; }
        }

        public BasicNeed GetBasicNeed(BasicNeedSO basicNeedSO)
        {
            if (myBasicNeedsMapped.ContainsKey(basicNeedSO))
            {
                return myBasicNeedsMapped[basicNeedSO];
            }
            else
            {
                return null;
            }
        }
        public SubjectiveNeed GetSubjectiveNeed(SubjectiveNeedSO subjectiveNeedSO)
        {
            if (mySubjectiveNeedsMapped.ContainsKey(subjectiveNeedSO))
            {
                return mySubjectiveNeedsMapped[subjectiveNeedSO];
            }
            else
            {
                return null;
            }
        }

        // TODO: test these.
        public void AddBasicNeed(BasicNeedSO basicNeedSO)
        {
            if (myBasicNeedsMapped.ContainsKey(basicNeedSO))
            {
                Debug.Log(string.Format("{0}'s NeedController already contains a basic need {1}. It will not be added.", gameObject.name, basicNeedSO.NeedName));
            }
            else
            {
                BasicNeed tempNeed = new BasicNeed(basicNeedSO);
                tempNeed.OnBNReachedThreshold += OnBasicNeedReachedThreshold;
                tempNeed.OnBNFailure += OnBasicNeedFailure;
                tempNeed.OnBNLeftThreshold += OnBasicNeedLeftThreshold;

                myBasicNeedsMapped.Add(basicNeedSO, tempNeed);
                myBasicNeeds = myBasicNeedsMapped.Values.ToArray();

                // TODO: modulate here?
            }
        }
        public void AddSubjectiveNeed(SubjectiveNeedSO subjectiveNeedSO)
        {
            if (mySubjectiveNeedsMapped.ContainsKey(subjectiveNeedSO))
            {
                Debug.Log(string.Format("{0}'s NeedController already contains a subjective need {1}. It will not be added.", gameObject.name, subjectiveNeedSO.NeedName));
            }
            else
            {
                SubjectiveNeed tempNeed = new SubjectiveNeed(subjectiveNeedSO);
                tempNeed.ONSNReachedThreshold += OnSubjectiveNeedReachThreshold;
                tempNeed.OnSNFailure += OnSubjectiveNeedFailure;
                tempNeed.OnSNLeftThreshold += OnSubjectiveNeedLeftThreshold;

                mySubjectiveNeedsMapped.Add(subjectiveNeedSO, tempNeed);
                mySubjectiveNeeds = mySubjectiveNeedsMapped.Values.ToArray();

                // TODO: modulate here?
            }
        }

        // TODO: test these.
        public void RemoveBasicNeed(BasicNeedSO basicNeedSO)
        {
            if (myBasicNeedsMapped.ContainsKey(basicNeedSO))
            {
                // Not sure if this is necessary.
                myBasicNeedsMapped[basicNeedSO].OnBNReachedThreshold -= OnBasicNeedReachedThreshold;
                myBasicNeedsMapped[basicNeedSO].OnBNFailure -= OnBasicNeedFailure;
                myBasicNeedsMapped[basicNeedSO].OnBNLeftThreshold -= OnBasicNeedLeftThreshold;

                myBasicNeedsMapped.Remove(basicNeedSO);
                myBasicNeeds = myBasicNeedsMapped.Values.ToArray();
            }
            else
            {
                Debug.Log(string.Format("{0}'s NeedController did not contain the basic need {1}. Nothing to remove.", gameObject.name, basicNeedSO.NeedName));
            }
        }
        public void RemoveSubjectiveNeed(SubjectiveNeedSO subjectiveNeedSO)
        {
            if (mySubjectiveNeedsMapped.ContainsKey(subjectiveNeedSO))
            {
                // Not sure if this is necessary.
                mySubjectiveNeedsMapped[subjectiveNeedSO].ONSNReachedThreshold -= OnSubjectiveNeedReachThreshold;
                mySubjectiveNeedsMapped[subjectiveNeedSO].OnSNFailure -= OnSubjectiveNeedFailure;
                mySubjectiveNeedsMapped[subjectiveNeedSO].OnSNLeftThreshold -= OnSubjectiveNeedLeftThreshold;

                mySubjectiveNeedsMapped.Remove(subjectiveNeedSO);
                mySubjectiveNeeds = mySubjectiveNeedsMapped.Values.ToArray();
            }
            else
            {
                Debug.Log(string.Format("{0}'s NeedController did not contain the basic need {1}. Nothing to remove.", gameObject.name, subjectiveNeedSO.NeedName));
            }
        }



        // METHODS

        // INIT

        public void Init() // For use during testing.
        {
            BasicNeedSO[] basicNeedSOs = Resources.LoadAll<BasicNeedSO>("ScriptableObjects/Needs/Basic");
            SubjectiveNeedSO[] subjectiveNeedSOs = Resources.LoadAll<SubjectiveNeedSO>("ScriptableObjects/Needs/Subjective");

            Init(basicNeedSOs, subjectiveNeedSOs);
        }

        public void Init(BasicNeedSO[] basicNeedSOs, SubjectiveNeedSO[] subjectiveNeedSOs)
        {
            CreateBasicNeeds(basicNeedSOs);
            CreateSubjectiveNeeds(subjectiveNeedSOs);

            // TODO: Hook up the modulations here?

            // Start need decay.
            //StartNeedDecay();
        }

        public void ClearArraysAndDicts() // For use during testing;
        {
            myBasicNeeds = null;
            myBasicNeedsMapped = null;

            mySubjectiveNeeds = null;
            mySubjectiveNeedsMapped = null;
        }


        // SET UP

        // Create need arrays and dictionaries from arrays of SOs.
        private void CreateBasicNeeds(BasicNeedSO[] basicNeedSOs)
        {
            myBasicNeeds = new BasicNeed[basicNeedSOs.Length];
            myBasicNeedsMapped = new SerializedDictionary<BasicNeedSO, BasicNeed>();

            for (int i = 0; i < basicNeedSOs.Length; i++)
            {
                myBasicNeeds[i] = new BasicNeed(basicNeedSOs[i]);
                myBasicNeedsMapped.Add(basicNeedSOs[i], myBasicNeeds[i]);

                myBasicNeeds[i].OnBNReachedThreshold += OnBasicNeedReachedThreshold;
                myBasicNeeds[i].OnBNFailure += OnBasicNeedFailure;
                myBasicNeeds[i].OnBNLeftThreshold += OnBasicNeedLeftThreshold;

                //myBasicNeeds[i].Init(myBasePerson);
                // TODO: Init modulators here?

                Debug.Log(myBasicNeeds[i].ToString());
            }
        }
        private void CreateSubjectiveNeeds(SubjectiveNeedSO[] subjectiveNeedSOs)
        {
            mySubjectiveNeeds = new SubjectiveNeed[subjectiveNeedSOs.Length];
            mySubjectiveNeedsMapped = new SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed>();

            for (int i = 0; i < subjectiveNeedSOs.Length; i++)
            {
                mySubjectiveNeeds[i] = new SubjectiveNeed(subjectiveNeedSOs[i]);
                mySubjectiveNeedsMapped.Add(subjectiveNeedSOs[i], mySubjectiveNeeds[i]);

                mySubjectiveNeeds[i].ONSNReachedThreshold += OnSubjectiveNeedReachThreshold;
                mySubjectiveNeeds[i].OnSNFailure += OnSubjectiveNeedFailure;
                mySubjectiveNeeds[i].OnSNLeftThreshold += OnSubjectiveNeedLeftThreshold;

                //mySubjectiveNeeds[i].Init(myBasePerson);
                // TODO: Init modulators here?

                Debug.Log(mySubjectiveNeeds[i].ToString());
            }
        }
         

        // BUILT IN

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            // TODO: clear out when testing is done.

            if (Input.GetKeyDown(KeyCode.V))
            {
                StartNeedDecay();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                RandomizeNeedChangeRates(-0.3f, 0.1f);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                SortAndPrintNeedInfo();
            }
        }
    }
}
