

using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class NeedController : MonoBehaviour
    {
        // TEMP
        // TODO: Have these be loaded automatically from the resource folder instead?
        // Will have to edit the init functions.
        public BasicNeedSO[] basicNeedSOs;
        public SubjectiveNeedSO[] subjectiveNeedSOs;



        // VARIABLES
        [SerializeField] private BasicNeed[] myBasicNeeds;
        [SerializeField] private SubjectiveNeed[] mySubjectiveNeeds;

        // For use by external functions that only know of a BasicNeedSO.
        private Dictionary<BasicNeedSO, BasicNeed> myBasicNeedsMapped;
        private Dictionary<SubjectiveNeedSO, SubjectiveNeed> mySubjectiveNeedsMapped;


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

        public BasicNeed[] MyBasicNeeds
        {
            get { return myBasicNeeds; }
        }
        public SubjectiveNeed[] MySubjectiveNeeds
        {
            get { return mySubjectiveNeeds; }
        }

        // Option to add or remove a need?;



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

            // TODO: Hook up the modulations here?
        }
        private void InitBasicNeeds()
        {
            myBasicNeedsMapped = new Dictionary<BasicNeedSO, BasicNeed>();

            myBasicNeeds = new BasicNeed[basicNeedSOs.Length];
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

            StartBasicNeedDecay(myBasicNeeds);
        }
        private void InitSubjectiveNeeds()
        {
            mySubjectiveNeedsMapped = new Dictionary<SubjectiveNeedSO, SubjectiveNeed>();
            
            mySubjectiveNeeds = new SubjectiveNeed[subjectiveNeedSOs.Length];
            for (int i = 0; i < subjectiveNeedSOs.Length; i++)
            {
                mySubjectiveNeeds[i] = new SubjectiveNeed(subjectiveNeedSOs[i]);
                mySubjectiveNeedsMapped.Add(subjectiveNeedSOs[i], mySubjectiveNeeds[i]);

                mySubjectiveNeeds[i].ONSNReachedThreshold += OnSubjectiveNeedWarning;
                mySubjectiveNeeds[i].OnSNFailure += OnSubjectiveNeedFailure;
                mySubjectiveNeeds[i].OnSNLeftThreshold += OnSubjectiveNeedLeftThreshold;

                //mySubjectiveNeeds[i].Init(myBasePerson);
                // TODO: Init modulators here?

                Debug.Log(mySubjectiveNeeds[i].ToString());
            }

            StartSubjectiveNeedDecay(mySubjectiveNeeds);
        }



        // Event handlers.
        // Basic needs.
        private void OnBasicNeedReachedThreshold(BasicNeedSO needIdentity, float needLevelCurrent, float needLevelCurrentAsPercentage, NeedStates needState)
        {
            if (needState == NeedStates.Warning)
            {
                Debug.Log(string.Format("{0} threw a Warning event.", needIdentity.NeedName));

                // Throw the event upwards.
                OnBNReachedThreshold?.Invoke(needIdentity, needLevelCurrent, needLevelCurrentAsPercentage, NeedStates.Warning);
            }
            else if (needState == NeedStates.Critical)
            {
                Debug.Log(string.Format("{0} threw a Critical event.", needIdentity.NeedName));

                // Throw the event upwards.
                OnBNReachedThreshold?.Invoke(needIdentity, needLevelCurrent, needLevelCurrentAsPercentage, NeedStates.Critical);
            }
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


        private void OnSubjectiveNeedWarning(SubjectiveNeedSO needIdentity, bool subNeed, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage, NeedStates needState)
        {
            if (needState == NeedStates.Warning)
            {
                Debug.Log(string.Format("{0} threw a Warning event.", (subNeed ? needIdentity.NeedNameRight : needIdentity.NeedNameLeft)));

                // Throw the event upwards.
                OnSNReachedThreshold?.Invoke(needIdentity, subNeed, needLevelCurrent, needLevelCurrentAsPercentage, NeedStates.Warning);
            }
            else if (needState == NeedStates.Critical)
            {
                Debug.Log(string.Format("{0} threw a Critical event.", (subNeed ? needIdentity.NeedNameRight : needIdentity.NeedNameLeft)));

                // Throw the event upwards.
                OnSNReachedThreshold?.Invoke(needIdentity, subNeed, needLevelCurrent, needLevelCurrentAsPercentage, NeedStates.Critical);
            }
        }
        private void OnSubjectiveNeedFailure(SubjectiveNeedSO needIdentity, bool subNeed)
        {
            Debug.Log(string.Format("{0} threw a Failure event.", (subNeed ? needIdentity.NeedNameRight : needIdentity.NeedNameLeft)));

            // Throw the event upwards.
            OnSNFailure?.Invoke(needIdentity, subNeed);
        }
        private void OnSubjectiveNeedLeftThreshold(SubjectiveNeedSO needIdentity, bool subNeed, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage, NeedStates previousNeedState)
        {
            Debug.Log(string.Format("{0} threw a LeftThreshold event ({1}).", (subNeed ? needIdentity.NeedNameRight : needIdentity.NeedNameLeft), previousNeedState));
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
        public void ModulateSubjectiveNeeds(Person sourceBasePerson)
        {
            foreach (SubjectiveNeed need in mySubjectiveNeeds)
            {
                need.Init(sourceBasePerson);
            }
        }
        public void ModulateBasicNeeds(Person sourceBasePerson)
        {
            foreach (BasicNeed need in myBasicNeeds)
            {
                need.Init(sourceBasePerson);
            }
        }


        // Built in.

        // Start is called before the first frame update
        void Start()
        {
            //myBasePerson = gameObject.GetComponent<BasePerson>();

            Init();
        } 

        // Update is called once per frame
        void Update()
        {

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
