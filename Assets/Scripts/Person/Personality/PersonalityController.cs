using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Quille
{
    public class PersonalityController : MonoBehaviour
    {
        // VARIABLES/PARAMS
        [SerializedDictionary("Personality Axe", "Score")]
        public SerializedDictionary<PersonalityAxeSO, float> myPersonalityAxes;
        // Should be private, but is not visible in editor when it is.

        // PROPERTIES
        public float GetScore(PersonalityAxeSO targetPersonalityAxe)
        {
            return myPersonalityAxes[targetPersonalityAxe];
        }
        public void SetScore(PersonalityAxeSO targetPersonalityAxe, float value)
        {
            if (value < -Constants.AXE_HALF_SPAN)
                myPersonalityAxes[targetPersonalityAxe] = -Constants.AXE_HALF_SPAN;
            else if (value > Constants.AXE_HALF_SPAN)
                myPersonalityAxes[targetPersonalityAxe] = Constants.AXE_HALF_SPAN;
            else
                myPersonalityAxes[targetPersonalityAxe] = value;
        }
        // TO DO: Add checks in case the targeted axe is not present in the dict.






        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {


        }
    }
}
