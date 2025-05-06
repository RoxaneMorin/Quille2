using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Building
{
    //[System.Serializable]
    public class WallSegment : MonoBehaviour
    {
        // VARIABLES/PARAMETERS
        [SerializeField] private WallAnchor anchorA;
        [SerializeField] private WallAnchor anchorB;
        // These should never be null.

        // Wall thickness, height at both anchors?
        // Should those switch if we switch up the anchors?
        // Some info on both 'sides'?
        // Should it depend on the wall direction?

        [Header("Resources")]
        [SerializeField] private LineRenderer lineRenderer;



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



        // METHODS

        // CONSTRUCTOR
        public WallSegment(WallAnchor anchorA, WallAnchor anchorB)
        {
            this.anchorA = anchorA;
            this.anchorB = anchorB;
        }

        // INIT
        public void Init(WallAnchor anchorA, WallAnchor anchorB)
        {
            // Name the game object.
            gameObject.name = string.Format("WallSegment ({0} <-> {1})", anchorA, anchorB);

            // Set the anchor references.
            this.anchorA = anchorA;
            this.anchorB = anchorB;

            // Fetch lineRenderer and update its visuals.
            lineRenderer = gameObject.GetComponent<LineRenderer>();
            lineRenderer.widthMultiplier = 0.1f;

            UpdateSegmentVisual();
        }

        public void UpdateSegmentVisual()
        {
            lineRenderer.SetPosition(0, anchorA.transform.position);
            lineRenderer.SetPosition(1, anchorB.transform.position);

            Color randomColour = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            lineRenderer.startColor = randomColour;
            lineRenderer.endColor = randomColour;
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
