using AYellowpaper.SerializedCollections;
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
        public class WallConnection
        {
            // TODO: move this class to its own file?

            // VARIABLES/PARAMETERS
            [SerializeField] private WallSegment connectedWallSegment;
            [SerializeField] private WallAnchor connectedAnchor;
            [SerializeField] private float angle;
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
        [SerializeField, SerializedDictionary("Other Anchor", "Connection")] private SerializedDictionary<WallAnchor, WallConnection> connectionsFromAnchorsDict;
        [SerializeField, SerializedDictionary("Segment", "Connection")] private SerializedDictionary<WallSegment, WallConnection> connectionsFromSegmentsDict;
        // TODO: these don't need to be serialized, switch that around once testing is done.
        

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


        // UTILITY
        public void AddConnection(WallSegment connectedSegment)
        {
            WallAnchor connectedAnchor = connectedSegment.OtherAnchor(this);
            float newConnectionsAngle = Building_MathHelpers.GetNormalizedAngleBetween(transform.position, connectedAnchor.transform.position);
            //Debug.Log(string.Format("{0}'s angle around {1}: {2}.", connectedAnchor, this, angle));

            WallConnection newConnection = new WallConnection(connectedSegment, connectedAnchor, newConnectionsAngle);

            connections.SortedInsert(newConnection, (existingConnection, newConnection) => existingConnection.Angle > newConnection.Angle);
            connectionsFromAnchorsDict.Add(connectedAnchor, newConnection);
            connectionsFromSegmentsDict.Add(connectedSegment, newConnection);
        }

        private void DeleteConnection(WallConnection targetConnection)
        {
            connectionsFromAnchorsDict.Remove(targetConnection.ConnectedAnchor);
            connectionsFromSegmentsDict.Remove(targetConnection.ConnectedWallSegment);
            connections.Remove(targetConnection);
        }
        public void DeleteConnection(WallAnchor targetAnchor)
        {
            WallConnection targetConnection = connectionsFromAnchorsDict[targetAnchor];
            if (targetConnection != null)
            {
                DeleteConnection(targetConnection);
            }
        }
        public void DeleteConnection(WallSegment targetSegment)
        {
            WallConnection targetConnection = connectionsFromSegmentsDict[targetSegment];
            if (targetConnection != null)
            {
                DeleteConnection(targetConnection);
            }
        }
        

        
        public void UpdateConnection()
        {

        }


        // TODO: replace/update connection

        // TODO: find connection/verify connection exists


        public bool VerifyConnection(WallAnchor targetAnchor)
        {
            return connectionsFromAnchorsDict.ContainsKey(targetAnchor);
        }
        public bool VerifyConnection(WallSegment targetSegment)
        {
            return connectionsFromSegmentsDict. ContainsKey(targetSegment);
        }




        private void UpdateGameObjectHeight()
        {
            Vector3 anchorScale = gameObject.transform.localScale;
            anchorScale.y = height;
            gameObject.transform.localScale = anchorScale;
        }



        // BUILT IN
        public void OnPointerDown(PointerEventData eventData)
        {
            OnWallAnchorClicked?.Invoke(this, eventData.button);
        }
    }

}
