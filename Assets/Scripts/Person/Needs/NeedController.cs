using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class NeedController : MonoBehaviour
    {
        // TEMP
        public BasicNeedSO[] basicNeedSOs;
        public SubjectiveNeedSO[] subjectiveNeedSOs;



        // VARIABLES
        [SerializeField]
        private BasicNeed[] myBasicNeeds;
        [SerializeField]
        private SubjectiveNeed[] mySubjectiveNeeds;

        // Change to a dict <needSO, need> ?



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
        private void InitBasicNeeds()
        {
            myBasicNeeds = new BasicNeed[basicNeedSOs.Length];
            for (int i = 0; i < basicNeedSOs.Length; i++)
            {
                myBasicNeeds[i] = new BasicNeed(basicNeedSOs[i]);
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
                Debug.Log(mySubjectiveNeeds[i].ToString());
            }

            StartSubjectiveNeedDecay(mySubjectiveNeeds);
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


        // Built in.

        // Start is called before the first frame update
        void Start()
        {
            InitBasicNeeds();
            InitSubjectiveNeeds();
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
