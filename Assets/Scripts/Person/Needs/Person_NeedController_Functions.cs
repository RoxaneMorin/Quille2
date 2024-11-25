using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Quille
{
    public partial class Person_NeedController : MonoBehaviour
    {
        // EVENTS
        // Basic needs.
        public event BasicNeedLevelCurrentUpdate OnBNLevelCurrentUpdate;
        public event BasicNeedReachedThreshold OnBNReachedThreshold;
        public event BasicNeedFailure OnBNFailure;

        // Subjective needs.
        public event SubjectiveNeedLevelCurrentUpdate OnSNLevelCurrentUpdate;
        public event SubjectiveNeedReachedThreshold OnSNReachedThreshold;
        public event SubjectiveNeedFailure OnSNFailure;



        // METHODS

        // EVENT HANDLING
        // Basic needs.
        private void OnBasicNeedReachedThreshold(BasicNeedSO needIdentity, float needLevelCurrent, float needLevelCurrentAsPercentage, NeedStates needState)
        {
            Debug.Log(string.Format("{0} threw a ReachedThreshold event ({1}).", needIdentity.NeedName, needState));

            // Throw the event upwards.
            OnBNReachedThreshold?.Invoke(needIdentity, needLevelCurrent, needLevelCurrentAsPercentage, needState);
        }
        private void OnBasicNeedFailure(BasicNeedSO needIdentity)
        {
            Debug.Log(string.Format("{0} threw a Failure event.", needIdentity.NeedName));

            // Throw the event upwards.
            OnBNFailure?.Invoke(needIdentity);
        }
        private void OnBasicNeedLeftThreshold(BasicNeedSO needIdentity, float needLevelCurrent, float needLevelCurrentAsPercentage, NeedStates previousNeedState)
        {
            Debug.Log(string.Format("{0} threw a LeftThreshold event ({1}).", needIdentity.NeedName, previousNeedState));
        }

        private void OnSubjectiveNeedReachThreshold(SubjectiveNeedSO needIdentity, bool subNeed, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage, NeedStates needState)
        {
            Debug.Log(string.Format("{0} ({1}) threw a ReachedThreshold event ({2}).", (subNeed ? needIdentity.NeedNameRight : needIdentity.NeedNameLeft), needIdentity.NeedName, needState));

            // Throw the event upwards.
            OnSNReachedThreshold?.Invoke(needIdentity, subNeed, needLevelCurrent, needLevelCurrentAsPercentage, needState);
        }
        private void OnSubjectiveNeedFailure(SubjectiveNeedSO needIdentity, bool subNeed)
        {
            Debug.Log(string.Format("{0} ({1}) threw a Failure event.", (subNeed ? needIdentity.NeedNameRight : needIdentity.NeedNameLeft), needIdentity.NeedName));

            // Throw the event upwards.
            OnSNFailure?.Invoke(needIdentity, subNeed);
        }
        private void OnSubjectiveNeedLeftThreshold(SubjectiveNeedSO needIdentity, bool subNeed, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage, NeedStates previousNeedState)
        {
            Debug.Log(string.Format("{0} ({1}) threw a LeftThreshold event ({2}).", (subNeed ? needIdentity.NeedNameRight : needIdentity.NeedNameLeft), needIdentity.NeedName, previousNeedState));
        }


        // COROUTINE MANAGEMENT

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
        private void StartSubjectiveNeedDecay(SubjectiveNeed[] mySubjectiveNeeds)
        {
            foreach (SubjectiveNeed myNeed in mySubjectiveNeeds)
            {
                StartSubjectiveNeedDecay(myNeed);
            }
        }
        private void StopSubjectiveNeedDecay(SubjectiveNeed[] mySubjectiveNeeds)
        {
            foreach (SubjectiveNeed myNeed in mySubjectiveNeeds)
            {
                StopSubjectiveNeedDecay(myNeed);
            }
        }

        // All needs in controller.
        private void StartNeedDecay()
        {
            StartBasicNeedDecay(myNeedData.MyBasicNeeds);
            StartSubjectiveNeedDecay(myNeedData.MySubjectiveNeeds);
        }
        private void StopNeedDecay()
        {
            StopBasicNeedDecay(myNeedData.MyBasicNeeds);
            StopSubjectiveNeedDecay(myNeedData.MySubjectiveNeeds);
        }


        // NEED CHECKS (for use by personAI)

        // Return the neediest need of each type as well as its level.
        public (BasicNeedSO, float) PerformBasicNeedCheck()
        {
            BasicNeed neediestNeed = BasicNeed.ReturnNeediest(myNeedData.MyBasicNeeds);
            float neediestNeedLevel = neediestNeed.LevelCurrentAsPercentage;

            return (neediestNeed.NeedSO, neediestNeedLevel);
        }
        public (SubjectiveNeedSO, bool, float) PerformSubjectiveNeedCheck()
        {
            (SubjectiveNeed, bool) neediestNeed = SubjectiveNeed.ReturnNeediestbyNeediestDelta(myNeedData.MySubjectiveNeeds);
            float neediestNeedLevel = neediestNeed.Item2 ? neediestNeed.Item1.LevelCurrentRightAsPercentage : neediestNeed.Item1.LevelCurrentLeftAsPercentage;

            // The returned bool indicates which side is neediest, where Left = 0, Right = 1.
            return (neediestNeed.Item1.NeedSO, neediestNeed.Item2, neediestNeedLevel);
        }



        // BUILT IN

        // Start is called before the first frame update
        void Start()
        {
            // TODO: clear out when testing is done.
            //Init();

            // Testing serialization.
            //string jsonStringSD = JsonConvert.SerializeObject(myNeedData, Formatting.Indented);
            //Debug.Log(jsonStringSD);
        }

        // Update is called once per frame
        void Update()
        {
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




        // TESTING
        private void RandomizeNeedChangeRates(float min, float max)
        {
            foreach (BasicNeed need in myNeedData.MyBasicNeeds)
            {
                float randomRate = Random.Range(min, max);
                need.CurrentChangeRate = randomRate;
            }

            foreach (SubjectiveNeed need in myNeedData.MySubjectiveNeeds)
            {
                float randomRateLeft = Random.Range(min, max);
                float randomRateRight = Random.Range(min, max);
                need.CurrentChangeRate = (randomRateLeft, randomRateRight);

                need.AverageLocalAiPriorityWeighting();
            }
        }
        private void SortAndPrintNeedInfo()
        {
            BasicNeed.SortByFulfillmentDelta(myNeedData.MyBasicNeeds, true, true);
            SubjectiveNeed.SortByFulfillmentDeltaofNeediest(myNeedData.MySubjectiveNeeds, true, true);

            foreach (BasicNeed need in myNeedData.MyBasicNeeds)
            {
                Debug.Log(need.ToString());
            }

            foreach (SubjectiveNeed need in myNeedData.MySubjectiveNeeds)
            {
                Debug.Log(need.ToString());
            }
        }
    }

}

