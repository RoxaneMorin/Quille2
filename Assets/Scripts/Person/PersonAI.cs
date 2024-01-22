using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    public class PersonAI : MonoBehaviour
    {
        // VARIABLES

        // Should the needNotice stuff be stored here instead of in the needController?

        BasePerson myBasePerson;
        // Do we need additional controller references here?



        // METHODS

        // AI Logic

        /*
         * Even X seconds, verify if:
         * -> Any basic need is at or below notice. If so, do action unless otherwise occupied.
         * -> Any subjective need is at or below notice. If so, do action unless otherwise occupied.
         * Once the action is done, check again in case multiple needs were at or below notice.
         * 
         * Receiving a need Warning, Critical or Failure event can overwrite other actions depending on the importance of said need.
         */


        // Init.
        void Init()
        {
            // Fetch our various components.
            myBasePerson = GetComponent<BasePerson>();
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

        }
    }
}
