using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class NeedController : MonoBehaviour
    {
        public BasicNeedSO[] basicNeedSOs;
        public SubjectiveNeedSO[] subjectiveNeedSOs;

        public BasicNeed[] myBasicNeeds;
        public SubjectiveNeed[] mySubjectiveNeeds;

        public GameObject needBarPrefab;
        public Canvas needCanvas;

        

        private void StartBasicNeedDecay(BasicNeed myNeed)
        {
            myNeed.CurrentChangeRate = myNeed.BaseChangeRate; // is this safeguard needed?
            StartCoroutine(myNeed.AlterLevelByChangeRate());
        }
        private void StopBasicNeedDecay(BasicNeed myNeed)
        {
            StopCoroutine(myNeed.AlterLevelByChangeRate());
        }

        private void StartSubjectiveNeedDecay(SubjectiveNeed myNeed)
        {
            myNeed.CurrentChangeRate = myNeed.BaseChangeRate; // is this safeguard needed?
            StartCoroutine(myNeed.AlterLevelByChangeRate());
        }
        private void StopSubjectiveNeedDecay(SubjectiveNeed myNeed)
        {
            StopCoroutine(myNeed.AlterLevelByChangeRate());
        }


        private void StartBasicNeedDecay(BasicNeed[] myBasicNeeds) // what would be a better name??
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
        private void StartSubjectiveNeedDecay(SubjectiveNeed[] myBasicNeeds) // what would be a better name??
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



        // Start is called before the first frame update
        void Start()
        {
            myBasicNeeds = new BasicNeed[basicNeedSOs.Length];
            for (int i = 0; i < basicNeedSOs.Length; i++)
            {
                myBasicNeeds[i] = new BasicNeed(basicNeedSOs[i]);
                Debug.Log(myBasicNeeds[i].ToString());

                NeedBar needBar = Instantiate(needBarPrefab, needCanvas.transform).GetComponentInChildren<NeedBar>();
                needBar.associatedBasicNeed = myBasicNeeds[i];
                myBasicNeeds[i].myNeedBar = needBar;

                needBar.Prepare();
                needBar.transform.position = new Vector3(i * 100, needBar.transform.position.y, needBar.transform.position.z);
            }

            StartBasicNeedDecay(myBasicNeeds);

            mySubjectiveNeeds = new SubjectiveNeed[subjectiveNeedSOs.Length];
            for (int i = 0; i < subjectiveNeedSOs.Length; i++)
            {
                mySubjectiveNeeds[i] = new SubjectiveNeed(subjectiveNeedSOs[i]);
                Debug.Log(mySubjectiveNeeds[i].ToString());
            }

            StartSubjectiveNeedDecay(mySubjectiveNeeds);



        } 

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (BasicNeed need in myBasicNeeds)
                {
                    float randomRate = Random.Range(-0.005f, 0.001f);
                    need.CurrentChangeRate = randomRate;
                }

                foreach (SubjectiveNeed need in mySubjectiveNeeds)
                {
                    float randomRateLeft = Random.Range(-0.005f, 0.001f);
                    float randomRateRight = Random.Range(-0.005f, 0.001f);
                    need.CurrentChangeRate = (randomRateLeft, randomRateRight);
                }
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                BasicNeed.SortByFulfillmentDelta(myBasicNeeds, true, true);

                foreach (BasicNeed need in myBasicNeeds)
                {
                    Debug.Log(need.ToString());
                }

                foreach (SubjectiveNeed need in mySubjectiveNeeds)
                {
                    Debug.Log(need.ToString());
                }
            }
        }
    }
}
