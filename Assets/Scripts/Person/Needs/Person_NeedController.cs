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
        [SerializeField] NeedController_Data myNeedData;

        // For use by external functions that only know of a NeedSO.
        [SerializeField, SerializedDictionary("BasicNeed SO", "Basic Need")] private SerializedDictionary<BasicNeedSO, BasicNeed> myBasicNeedsMapped;
        [SerializeField, SerializedDictionary("SubjectiveNeed SO", "Subjective Need")] private SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed> mySubjectiveNeedsMapped;

        // General need-related parameters.



        // PROPERTIES & GETTERS/SETTERS
        public BasicNeed[] MyBasicNeeds
        {
            get { return myNeedData.MyBasicNeeds; }
        }
        public SubjectiveNeed[] MySubjectiveNeeds
        {
            get { return myNeedData.MySubjectiveNeeds; }
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
                myNeedData.MyBasicNeeds = myBasicNeedsMapped.Values.ToArray();

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
                myNeedData.MySubjectiveNeeds = mySubjectiveNeedsMapped.Values.ToArray();

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
                myNeedData.MyBasicNeeds = myBasicNeedsMapped.Values.ToArray();
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
                myNeedData.MySubjectiveNeeds = mySubjectiveNeedsMapped.Values.ToArray();
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
            myNeedData = new NeedController_Data();

            CreateBasicNeeds(basicNeedSOs);
            CreateSubjectiveNeeds(subjectiveNeedSOs);

            // TODO: Hook up the modulations here?

            // Start need decay.
            //StartNeedDecay();
        }


        // SET UP

        // Create need arrays and dictionaries from arrays of SOs.
        private void CreateBasicNeeds(BasicNeedSO[] basicNeedSOs)
        {
            myNeedData.MyBasicNeeds = new BasicNeed[basicNeedSOs.Length];
            myBasicNeedsMapped = new SerializedDictionary<BasicNeedSO, BasicNeed>();

            for (int i = 0; i < basicNeedSOs.Length; i++)
            {
                myNeedData.MyBasicNeeds[i] = new BasicNeed(basicNeedSOs[i]);
                myBasicNeedsMapped.Add(basicNeedSOs[i], myNeedData.MyBasicNeeds[i]);

                myNeedData.MyBasicNeeds[i].OnBNReachedThreshold += OnBasicNeedReachedThreshold;
                myNeedData.MyBasicNeeds[i].OnBNFailure += OnBasicNeedFailure;
                myNeedData.MyBasicNeeds[i].OnBNLeftThreshold += OnBasicNeedLeftThreshold;

                //myBasicNeeds[i].Init(myBasePerson);
                // TODO: Init modulators here?

                Debug.Log(myNeedData.MyBasicNeeds[i].ToString());
            }
        }
        private void CreateSubjectiveNeeds(SubjectiveNeedSO[] subjectiveNeedSOs)
        {
            myNeedData.MySubjectiveNeeds = new SubjectiveNeed[subjectiveNeedSOs.Length];
            mySubjectiveNeedsMapped = new SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed>();

            for (int i = 0; i < subjectiveNeedSOs.Length; i++)
            {
                myNeedData.MySubjectiveNeeds[i] = new SubjectiveNeed(subjectiveNeedSOs[i]);
                mySubjectiveNeedsMapped.Add(subjectiveNeedSOs[i], myNeedData.MySubjectiveNeeds[i]);

                myNeedData.MySubjectiveNeeds[i].ONSNReachedThreshold += OnSubjectiveNeedReachThreshold;
                myNeedData.MySubjectiveNeeds[i].OnSNFailure += OnSubjectiveNeedFailure;
                myNeedData.MySubjectiveNeeds[i].OnSNLeftThreshold += OnSubjectiveNeedLeftThreshold;

                //mySubjectiveNeeds[i].Init(myBasePerson);
                // TODO: Init modulators here?

                Debug.Log(myNeedData.MySubjectiveNeeds[i].ToString());
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
