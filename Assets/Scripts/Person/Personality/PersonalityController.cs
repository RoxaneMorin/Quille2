using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Quille
{
    public class PersonalityController : MonoBehaviour
    {
        [SerializedDictionary("Personality Axe", "Score")]
        public SerializedDictionary<PersonalityAxeSO, float> myPersonalityAxes;
        // Should be private, but is not visible in editor when it is.

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
