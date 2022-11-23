﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class NeedController : MonoBehaviour
    {
        //public BasicNeed myNeed = new BasicNeed("Undefined", 1);

        public BasicNeed[] myBasicNeeds;

        public BasicNeedSO[] basicNeedSOs;

        private void StartBasicNeedDecay(BasicNeed myNeed)
        {
            myNeed.CurrentChangeRate = myNeed.BaseChangeRate; // is this safeguard needed?
            StartCoroutine(myNeed.AlterLevelByChangeRate());
        }
        private void StopBasicNeedDecay(BasicNeed myNeed)
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






        // Start is called before the first frame update
        void Start()
        {
            myBasicNeeds = new BasicNeed[basicNeedSOs.Length];
            
            for (int i = 0; i < basicNeedSOs.Length; i++)
            {
                myBasicNeeds[i] = new BasicNeed(basicNeedSOs[i]);
            }

            StartBasicNeedDecay(myBasicNeeds);



        } 

        // Update is called once per frame
        void Update()
        {
        }
    }
}
