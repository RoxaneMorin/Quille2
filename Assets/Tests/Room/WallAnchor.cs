using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Building
{
    public class WallAnchor : MonoBehaviour
    {
        // VARIABLES/PARAMETERS
        [SerializeField] private float height = 1f;
        // Store height per wall instead?

        [SerializeField] private List<WallSegment> connectedWallSegments;



        // PROPERTIES
        public float Height
        {
            get { return height; }
            set
            {
                if (value < 0)
                {
                    height = 0;
                }
                else
                {
                    height = value;
                }
                UpdateGameObjectHeight();
            }
        }


        // return pos at height?



        // EVENTS
        public event WallAnchorClicked OnWallAnchorClicked;



        // METHODS

        // INIT
        public void Init(float height = 1f)
        {
            this.height = height;
            UpdateGameObjectHeight();

            this.connectedWallSegments = new List<WallSegment>();

            // TODO: name via coordinates
        }


        // ADDITIONAL SETTERS AND ACCESSORS
        public void AddConnection(WallSegment connectedSegment)
        {
            connectedWallSegments.Add(connectedSegment);

            // TODO: the actual sorting, possibly keep track of the wallSegment's target 'side'.
        }



        // UTILITY
        private void UpdateGameObjectHeight()
        {
            Vector3 anchorScale = gameObject.transform.localScale;
            anchorScale.y = height;
            gameObject.transform.localScale = anchorScale;
        }





        




        // TODO: click, select/deselect;
        // Change its material there?



        // TODO: move this to some utility or extention class instead?
        //private float GetNormalizedAngleBetween(Transform centerTransform, Transform otherTransform)
        //{
        //    Vector3 otherPoint = otherTransform.position - centerTransform.position;
        //    return Mathf.Atan2(otherPoint.z, otherPoint.x).NormalizeRadAngle();
        //}




        //private void SortConnectionList()
        //{
        //    connectedAnchors.Sort
        //    (
        //        (a, b) =>
        //        {
        //            float angleA = GetNormalizedAngleBetween(transform, a.transform);
        //            float angleB = GetNormalizedAngleBetween(transform, b.transform);

        //            return angleA.CompareTo(angleB);
        //        }
        //    );
        //}

        //private void InsertIntoConnectionList(WallAnchor newAnchor)
        //{
        //    float newAnchorsAngle = GetNormalizedAngleBetween(transform, newAnchor.transform);

        //    // Look for the right index using a binary search.
        //    int min = 0;
        //    int max = connectedAnchors.Count;

        //    while (min < max)
        //    {
        //        int mid = (min + max) / 2;
        //        float midAngle = GetNormalizedAngleBetween(transform, connectedAnchors[mid].transform);

        //        if (newAnchorsAngle > midAngle)
        //        {
        //            min = mid + 1;
        //        }
        //        else
        //        {
        //            max = mid;
        //        }
        //    }

        //    connectedAnchors.Insert(min, newAnchor);
        //}



        // BUILT IN
        private void Start()
        {
            //foreach (WallAnchor anchor in connectedAnchors)
            //{
            //    Vector3 anchorPoint = anchor.transform.position - transform.position;
            //    Debug.Log(string.Format("{0}'s angle around {1} : {2}.", anchor, this, Mathf.Atan2(anchorPoint.z, anchorPoint.x).NormalizeRadAngle()));
            //}

            //SortConnectionList();
        }

        //private void OnDrawGizmos()
        //{
        //    foreach (WallAnchor anchor in connectedAnchors)
        //    {
        //        Gizmos.color = Color.black;
        //        GizmosExtended.DrawArrow(transform.position, anchor.transform.position);
        //    }
        //}
    }

}
