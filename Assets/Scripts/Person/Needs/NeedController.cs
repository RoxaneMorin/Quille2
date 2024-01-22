using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class NeedController : MonoBehaviour
    {
        // TEMP
        // Have these be loaded automatically from the resource folder instead?
        // Will have to edit the init functions.
        public BasicNeedSO[] basicNeedSOs;
        public SubjectiveNeedSO[] subjectiveNeedSOs;



        // VARIABLES
        [SerializeField]
        private BasicNeed[] myBasicNeeds;
        [SerializeField]
        private SubjectiveNeed[] mySubjectiveNeeds;

        // For use by external functions that only know of a BasicNeedSO.
        [SerializeField, HideInInspector]
        private AYellowpaper.SerializedCollections.SerializedDictionary<BasicNeedSO, BasicNeed> myBasicNeedsMapped;
        [SerializeField, HideInInspector]
        private AYellowpaper.SerializedCollections.SerializedDictionary<SubjectiveNeedSO, SubjectiveNeed> mySubjectiveNeedsMapped;


        // General need-related parameters.



        // PROPERTIES & GETTERS/SETTERS
        public BasicNeed GetBasicNeed(BasicNeedSO basicNeedSO)
        {
            try // including this just to be safe.
            {
                return myBasicNeedsMapped[basicNeedSO];
            }
            catch
            {
                return null;
            }
        }
        public SubjectiveNeed GetSubjectiveNeed(SubjectiveNeedSO subjectiveNeedSO)
        {
            try // including this just to be safe.
            {
                return mySubjectiveNeedsMapped[subjectiveNeedSO];
            }
            catch
            {
                return null;
            }
        }
        
        // Option to add or remove a need?;



        // METHODS

        // Coroutine management.

        // Individual needs.
        private void StartBasicNeedDecay(BasicNeed myNeed)
        {
            //myNeed.CurrentChangeRate = myNeed.BaseChangeRate; // is this safeguard needed?
            StartCoroutine(myNeed.AlterLevelByChangeRate());
        }
        private void StopBasicNeedDecay(BasicNeed myNeed)
        {
            StopCoroutine(myNeed.AlterLevelByChangeRate());
        }
        private void StartSubjectiveNeedDecay(SubjectiveNeed myNeed)
        {
            //myNeed.CurrentChangeRate = myNeed.BaseChangeRate; // is this safeguard needed?
            StartCoroutine(myNeed.AlterLevelByChangeRate());
        }
        private void StopSubjectiveNeedDecay(SubjectiveNeed myNeed)
        {
            StopCoroutine(myNeed.AlterLevelByChangeRate());
        }

        // Array of needs.
        private void StartBasicNeedDecay(BasicNeed[] myBasicNeeds)
        {
            foreach (BasicNeed myNeed in myBasicNeeds)
            {
                StartBasicNeedDecay(myNeed);
            }
        }
        private void StopBasicNeedDecay(BasicNeed[] myBasicNeeds) 
        {
            foreach (BasicNeed myNeed in myBasicNeeds)
            {
                StopBasicNeedDecay(myNeed);
            }
        }
        private void StartSubjectiveNeedDecay(SubjectiveNeed[] myBasicNeeds)
        {
            foreach (SubjectiveNeed myNeed in mySubjectiveNeeds)
            {
                StartSubjectiveNeedDecay(myNeed);
            }
        }
        private void StopSubjectiveNeedDecay(BasicNeed[] SubjectiveNeed)
        {
            foreach (SubjectiveNeed myNeed in mySubjectiveNeeds)
            {
                StopSubjectiveNeedDecay(myNeed);
            }
        }


        // Init.
        private void Init()
        {
            InitBasicNeeds();
            InitSubjectiveNeeds();

            // TO DO: Hook up the modulations here.
        }
        private void InitBasicNeeds()
        {
            myBasicNeeds = new BasicNeed[basicNeedSOs.Length];
            for (int i = 0; i < basicNeedSOs.Length; i++)
            {
                myBasicNeeds[i] = new BasicNeed(basicNeedSOs[i]);
                myBasicNeedsMapped.Add(basicNeedSOs[i], myBasicNeeds[i]);

                myBasicNeeds[i].OnBNReachedWarning += OnBasicNeedWarning;
                myBasicNeeds[i].OnBNReachedCritical += OnBasicNeedCritical;
                myBasicNeeds[i].OnBNFailure += OnBasicNeedFailure;

                Debug.Log(myBasicNeeds[i].ToString());
            }

            StartBasicNeedDecay(myBasicNeeds);
        }
        private void InitSubjectiveNeeds()
        {
            mySubjectiveNeeds = new SubjectiveNeed[subjectiveNeedSOs.Length];
            for (int i = 0; i < subjectiveNeedSOs.Length; i++)
            {
                mySubjectiveNeeds[i] = new SubjectiveNeed(subjectiveNeedSOs[i]);
                mySubjectiveNeedsMapped.Add(subjectiveNeedSOs[i], mySubjectiveNeeds[i]);

                mySubjectiveNeeds[i].OnSNReachedWarning += OnSubjectiveNeedWarning;
                mySubjectiveNeeds[i].OnSNReachedCritical += OnSubjectiveNeedCritical;
                mySubjectiveNeeds[i].OnSNFailure += OnSubjectiveNeedFailure;

                Debug.Log(mySubjectiveNeeds[i].ToString());
            }

            StartSubjectiveNeedDecay(mySubjectiveNeeds);
        }



        // Event handlers.
        // Basic needs.
        private void OnBasicNeedWarning(BasicNeedSO needIdentity, float needLevelCurrent, float needLevelCurrentAsPercentage)
        {
            Debug.Log(string.Format("{0} threw a Warning event.", needIdentity.NeedName));
        }
        private void OnBasicNeedCritical(BasicNeedSO needIdentity, float needLevelCurrent, float needLevelCurrentAsPercentage)
        {
            Debug.Log(string.Format("{0} threw a Critical event.", needIdentity.NeedName));
        }
        private void OnBasicNeedFailure(BasicNeedSO needIdentity)
        {
            Debug.Log(string.Format("{0} threw a Failure event.", needIdentity.NeedName));
        }
        private void OnSubjectiveNeedWarning(SubjectiveNeedSO needIdentity, bool subNeed, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage)
        {
            Debug.Log(string.Format("{0} threw a Warning event.", (subNeed ? needIdentity.NeedNameRight : needIdentity.NeedNameLeft)));
        }
        private void OnSubjectiveNeedCritical(SubjectiveNeedSO needIdentity, bool subNeed, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage)
        {
            Debug.Log(string.Format("{0} threw a Critical event.", (subNeed ? needIdentity.NeedNameRight : needIdentity.NeedNameLeft)));
        }
        private void OnSubjectiveNeedFailure(SubjectiveNeedSO needIdentity, bool subNeed)
        {
            Debug.Log(string.Format("{0} threw a Failure event.", (subNeed ? needIdentity.NeedNameRight : needIdentity.NeedNameLeft)));
        }




        // For use by personAI.

        // Return the neediest need of each type as well as its level.
        public (BasicNeedSO, float) PerformBasicNeedCheck() 
        {
            BasicNeed neediestNeed = BasicNeed.ReturnNeediest(myBasicNeeds);
            float neediestNeedLevel = neediestNeed.LevelCurrentAsPercentage;

            return (neediestNeed.NeedSO, neediestNeedLevel);
        }
        // The returned bool indicates which side is neediest, where Left = 0, Right = 1.
        public (SubjectiveNeedSO, bool, float) PerformSubjectiveNeedCheck()
        {
            (SubjectiveNeed, bool) neediestNeed = SubjectiveNeed.ReturnNeediestbyNeediestDelta(mySubjectiveNeeds);
            float neediestNeedLevel = neediestNeed.Item2 ? neediestNeed.Item1.LevelCurrentRightAsPercentage : neediestNeed.Item1.LevelCurrentLeftAsPercentage;

            return (neediestNeed.Item1.NeedSO, neediestNeed.Item2, neediestNeedLevel);
        }


        // Testing.
        private void RandomizeNeedChangeRates(float min, float max)
        {
            foreach (BasicNeed need in myBasicNeeds)
            {
                float randomRate = Random.Range(min, max);
                need.CurrentChangeRate = randomRate;
            }

            foreach (SubjectiveNeed need in mySubjectiveNeeds)
            {
                float randomRateLeft = Random.Range(min, max);
                float randomRateRight = Random.Range(min, max);
                need.CurrentChangeRate = (randomRateLeft, randomRateRight);

                need.AverageLocalAiPriorityWeighting();
            }
        }
        private void SortAndPrintNeedInfo()
        {
            BasicNeed.SortByFulfillmentDelta(myBasicNeeds, true, true);
            SubjectiveNeed.SortByFulfillmentDeltaofNeediest(mySubjectiveNeeds, true, true);

            foreach (BasicNeed need in myBasicNeeds)
            {
                Debug.Log(need.ToString());
            }

            foreach (SubjectiveNeed need in mySubjectiveNeeds)
            {
                Debug.Log(need.ToString());
            }
        }

        public void ModulateSubjectiveNeeds(BasePerson sourceBasePerson)
        {
            foreach (SubjectiveNeed need in mySubjectiveNeeds)
            {
                need.Init(sourceBasePerson);
            }
        }


        // Built in.

        // Start is called before the first frame update
        void Start()
        {
            Init();
        } 

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                RandomizeNeedChangeRates(-0.005f, 0.001f);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                SortAndPrintNeedInfo();
            }
        }
    }
}
