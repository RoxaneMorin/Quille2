using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    [System.Serializable]
    public class NeedNourishment : BasicNeed
    {
        // VARIABLES
        [SerializeField]
        private string needName = "Nourishment";
        [SerializeField]
        private string needIDName = "basicNeedNourishment";


        // LOCAL DECAY RATE

        private static float defaultDecayRate = -0.001f; // This need's universal default decay rate.

        override public float DefaultChangeRate
        {
            get { return defaultDecayRate; }
        }



        // PROPERTIES
        override public string GetNeedName()
        {
            return needName;
        }

        override public string GetNeedIDName()
        {
            return needIDName;
        }


        // CONSTRUCTOR

        public NeedNourishment() : base()
        {
            AIPriorityWeighting = 3;
        }
    }
}
