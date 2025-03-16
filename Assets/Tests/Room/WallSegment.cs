using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    [System.Serializable]
    public class WallSegment
    {
        // VARIABLES/PARAMETERS
        [SerializeField] private WallAnchor anchorA;
        [SerializeField] private WallAnchor anchorB;
        // These should never be null.

        // Wall thickness, height at both anchors?
        // Should those switch if we switch up the anchors?
        // Some info on both 'sides'?
        // Should it depend on the wall direction?


        // PROPERTIES
        public WallAnchor AnchorA
        {
            get { return anchorA; }
            set
            {
                if (value && value != anchorB)
                {
                    anchorA = value;
                }
            }
        }
        public WallAnchor AnchorB
        {
            get { return anchorB; }
            set
            {
                if (value && value != anchorA)
                {
                    anchorB = value;
                }
            }
        }







        // METHODS

        // CONSTRUCTOR
        public WallSegment(WallAnchor anchorA, WallAnchor anchorB)
        {
            this.anchorA = anchorA;
            this.anchorB = anchorB;
        }


        // ADDITIONAL ACCESSORS
        public WallAnchor OtherAnchor(WallAnchor sourceAnchor)
        {
            if (sourceAnchor == anchorA)
            {
                return anchorB;
            }
            else if (sourceAnchor == anchorB)
            {
                return anchorA;
            }
            else
            {
                Debug.LogError(string.Format("{0} is not one of this wall's anchors.", sourceAnchor));
                return null;
            }
        }


        // UTILITY
        public void InverseWallDirection()
        {
            // TODO: should we change smt inside the wall anchors too?

            WallAnchor tempAnchor = anchorA;
            anchorA = anchorB;
            anchorB = tempAnchor;
        }
    }
}
