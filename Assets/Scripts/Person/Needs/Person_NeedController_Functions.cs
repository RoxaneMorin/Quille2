using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Quille
{
    // The monoBehaviour containing and controlling a person's needs.
    // Need changes are handled via coroutines. Other behaviours may take place on Update.
    // This part of the class hosts events, as well as management and utility methods.
    // Not JSON serialized.


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

        private void OnSubjectiveNeedReachThreshold(SubjectiveNeedSO needIdentity, BasicNeedSO subNeed, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage, NeedStates needState)
        {
            Debug.Log(string.Format("{0} ({1}) threw a ReachedThreshold event ({2}).", subNeed.NeedName, needIdentity.NeedName, needState));

            // Throw the event upwards.
            OnSNReachedThreshold?.Invoke(needIdentity, subNeed, needLevelCurrent, needLevelCurrentAsPercentage, needState);
        }
        private void OnSubjectiveNeedFailure(SubjectiveNeedSO needIdentity, BasicNeedSO subNeed)
        {
            Debug.Log(string.Format("{0} ({1}) threw a Failure event.", subNeed.NeedName, needIdentity.NeedName));

            // Throw the event upwards.
            OnSNFailure?.Invoke(needIdentity, subNeed);
        }
        private void OnSubjectiveNeedLeftThreshold(SubjectiveNeedSO needIdentity, BasicNeedSO subNeed, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage, NeedStates previousNeedState)
        {
            Debug.Log(string.Format("{0} ({1}) threw a LeftThreshold event ({2}).", subNeed.NeedName, needIdentity.NeedName, previousNeedState));
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
            StartBasicNeedDecay(MyBasicNeeds);
            StartSubjectiveNeedDecay(MySubjectiveNeeds);
        }
        private void StopNeedDecay()
        {
            StopBasicNeedDecay(MyBasicNeeds);
            StopSubjectiveNeedDecay(MySubjectiveNeeds);
        }


        // NEED CHECKS (for use by personAI)

        // Return the neediest need of each type as well as its level.
        public (BasicNeedSO, float) PerformBasicNeedCheck()
        {
            BasicNeed neediestNeed = BasicNeed.ReturnNeediest(MyBasicNeeds);
            float neediestNeedLevel = neediestNeed.LevelCurrentAsPercentage;

            return (neediestNeed.NeedSO, neediestNeedLevel);
        }
        public (SubjectiveNeedSO, BasicNeedSO, float) PerformSubjectiveNeedCheck()
        {
            (SubjectiveNeed, BasicNeedSO) neediestNeed = SubjectiveNeed.ReturnNeediestbyNeediestDelta(MySubjectiveNeeds);
            float neediestNeedLevel = neediestNeed.Item1.LevelCurrentAsPercentageFor(neediestNeed.Item2);

            // Returns the neediest subneed's SO.
            return (neediestNeed.Item1.NeedSO, neediestNeed.Item2, neediestNeedLevel);
        }


        // UTILITY

        // Modulation
        public void ModulateBasicNeeds(Person sourceBasePerson)
        {
            foreach (BasicNeed need in MyBasicNeeds)
            {
                need.ModulateNeed(sourceBasePerson);
            }
        }
        public void ModulateSubjectiveNeeds(Person sourceBasePerson)
        {
            foreach (SubjectiveNeed need in MySubjectiveNeeds)
            {
                need.ModulateNeed(sourceBasePerson);
            }
        }
        public void ModulateAllNeeds(Person sourceBasePerson)
        {
            ModulateBasicNeeds(sourceBasePerson);
            ModulateSubjectiveNeeds(sourceBasePerson);
        }


        // TESTING
        private void RandomizeNeedChangeRates(float min, float max)
        {
            foreach (BasicNeed need in MyBasicNeeds)
            {
                float randomRate = Random.Range(min, max);
                need.CurrentChangeRate = randomRate;
            }

            foreach (SubjectiveNeed need in MySubjectiveNeeds)
            {
                float randomRateLeft = Random.Range(min, max);
                float randomRateRight = Random.Range(min, max);
                need.CurrentChangeRate = (randomRateLeft, randomRateRight);

                need.AverageLocalAiPriorityWeighting();
            }
        }
        private void SortAndPrintNeedInfo()
        {
            BasicNeed.SortByFulfillmentDelta(MyBasicNeeds, true, true);
            SubjectiveNeed.SortByFulfillmentDeltaofNeediest(MySubjectiveNeeds, true, true);

            foreach (BasicNeed need in MyBasicNeeds)
            {
                Debug.Log(need.ToString());
            }

            foreach (SubjectiveNeed need in MySubjectiveNeeds)
            {
                Debug.Log(need.ToString());
            }
        }
    }

}

