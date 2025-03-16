using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    public class WallAnchor : MonoBehaviour, IPointerDownHandler
    {
        // INTERNAL CLASSES
        [System.Serializable]
        private class WallConnection
        {
            // TODO: make a UI for this class.

            // VARIABLES/PARAMETERS
            private WallSegment connectedWallSegment;
            private WallAnchor connectedAnchor;
            private float angle;
            // TODO: should we also track whether this is anchor A or B?

            // PROPERTIES
            public WallSegment ConnectedWallSegment { get { return connectedWallSegment; } set { connectedWallSegment = value; } }
            public WallAnchor ConnectedAnchor { get { return connectedAnchor; } set { connectedAnchor = value; } }
            public float Angle { get { return angle; } set { angle = value; } }


            // CONSTRUCTOR
            public WallConnection(WallSegment connectedWallSegment, WallAnchor connectedAnchor, float angle)
            {
                this.connectedWallSegment = connectedWallSegment;
                this.connectedAnchor = connectedAnchor;
                this.angle = angle;
            }
        }



        // VARIABLES/PARAMETERS
        [SerializeField] private float height = 1f;
        // Store height per wall instead?

        [SerializeField] private List<WallConnection> connections;
        



        [SerializeField] private Renderer myRenderer;
        [SerializeField] private Material myMaterial;

        [SerializeField] private Color colourDefault = Color.black;
        [SerializeField] private Color colourSelected = Color.blue;




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
            // Name the game object.
            gameObject.name = string.Format("WallAnchor {0}", transform.position);


            // Fetch components.
            myRenderer = gameObject.GetComponent<Renderer>();
            myMaterial = myRenderer.material;


            // Set parameters.
            this.height = height;
            UpdateGameObjectHeight();

            this.connections = new List<WallConnection>();
        }


        // EVENT LISTENERS
        public void OnWallAnchorSelected(WallAnchor selectedAnchor)
        {
            if (selectedAnchor == this)
            {
                myMaterial.color = colourSelected;
            }
            else
            {
                myMaterial.color = colourDefault;
            }
        }


        // ADDITIONAL SETTERS AND ACCESSORS
        public void AddConnection(WallSegment connectedSegment)
        {
            WallAnchor connectedAnchor = connectedSegment.OtherAnchor(this);
            float angle = GetNormalizedAngleBetween(transform, connectedAnchor.transform);

            Debug.Log(string.Format("{0}'s angle around {1}: {2}.", connectedAnchor, this, angle));

            WallConnection newConnection = new WallConnection(connectedSegment, connectedAnchor, angle);


            // TODO: move the sorting to its own function?

            for (int i = 0; i < connections.Count; i++)
            {
                Debug.Log(string.Format("Existing connection {0}, angle {1}.", i, connections[i].Angle));

                if (connections[i].Angle > angle)
                {
                    connections.Insert(i, newConnection);
                    return;
                }
            }
            // Else, add to the end of the list.
            connections.Add(newConnection);
        }


        // UTILITY
        private void UpdateGameObjectHeight()
        {
            Vector3 anchorScale = gameObject.transform.localScale;
            anchorScale.y = height;
            gameObject.transform.localScale = anchorScale;
        }









        // TODO: move this to some utility or extention class instead?
        private float GetNormalizedAngleBetween(Transform centerTransform, Transform otherTransform)
        {
            Vector3 otherPoint = otherTransform.position - centerTransform.position;
            return Mathf.Atan2(otherPoint.z, otherPoint.x).NormalizeRadAngle();
        }




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
        public void OnPointerDown(PointerEventData eventData)
        {
            OnWallAnchorClicked?.Invoke(this, eventData.button);
        }


        //private void OnDrawGizmos()
        //{
        //    foreach (WallAnchor anchor in connectedAnchors)
        //    {
                  // TODO: display the number of connections in order around the item?
        //        Gizmos.color = Color.black;
        //        GizmosExtended.DrawArrow(transform.position, anchor.transform.position);
        //    }
        //}
        }

}
