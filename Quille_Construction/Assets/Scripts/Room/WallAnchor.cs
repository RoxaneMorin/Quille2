using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    public partial class WallAnchor : MonoBehaviour, IComparable, IPointerDownHandler
    {
        // VARIABLES/PARAMETERS
        [SerializeField] private int id; 

        [SerializeField] private float height = 1f;
        // Store height per wall instead?

        [SerializeField] private List<WallConnection> connections;
        [SerializeField] private Dictionary<WallAnchor, WallConnection> connectionsFromAnchorsDict;
        [SerializeField] private Dictionary<WallSegment, WallConnection> connectionsFromSegmentsDict;
        // Idea: since walls physically exist now, can we merge them with connections?
        

        [SerializeField] private Renderer myRenderer;
        [SerializeField] private Material myMaterial;

        [SerializeField] private Color colourDefault = Color.white;
        [SerializeField] private Color colourSelected = Color.blue;



        // PROPERTIES
        public int ID { get { return id; } }

        public Vector3 GroundPosition { get { return gameObject.transform.position; } }
        public Vector3 TopPosition { get { return gameObject.transform.position + new Vector3() { y = height }; } }
        public float3 GroundPositionF { get { return (float3)GroundPosition; } }
        public float3 TopPositionF { get { return (float3)TopPosition; } }

        public float Height
        {
            get { return height; }
            set
            {
                if (value < Constants_Building.MIN_WALL_ANCHOR_HEIGHT)
                {
                    height = Constants_Building.MIN_WALL_ANCHOR_HEIGHT;
                }
                else if(value > Constants_Building.MAX_WALL_ANCHOR_HEIGHT)
                {
                    height = Constants_Building.MAX_WALL_ANCHOR_HEIGHT;
                }
                else
                {
                    height = value;
                }

                OnParameterUpdate();
            }
        }

        public List<WallConnection> Connections { get { return connections; } }






        // EVENTS
        public event WallAnchorClicked OnWallAnchorClicked;



        // METHODS

        // INIT
        public void Init(int id, float height = 1f)
        {
            // Name the game object.
            gameObject.name = string.Format("WallAnchor {0} {1}", id, transform.position);


            // Fetch components.
            myRenderer = gameObject.GetComponent<Renderer>();
            myMaterial = myRenderer.material;


            // Set parameters.
            this.id = id;
            this.height = height;

            this.connections = new List<WallConnection>();
            this.connectionsFromAnchorsDict = new Dictionary<WallAnchor, WallConnection>();
            this.connectionsFromSegmentsDict = new Dictionary<WallSegment, WallConnection>();

            OnParameterUpdate();
        }


        // EVENT LISTENERS
        public void OnWallAnchorSelected(WallAnchor selectedAnchor)
        {
            if (selectedAnchor == this)
            {
                Select();
            }
            else
            {
                Unselect();
            }
        }

        public void OnParameterUpdate()
        {
            // Visual update.
            UpdateGameObjectHeight();

            // Notify the connected wall segments.
            foreach (WallConnection connection in connections)
            {
                WallSegment segment = connection.ConnectedSegment;
                segment.OnParameterUpdate();
            }
        }


        // UTILITY

        // -> CONNECTIONS
        public void AddConnection(WallAnchor connectedAnchor, WallSegment connectedSegment)
        {
            float newConnectionsAngle = MathHelpers.GetNormalizedAngleBetween(transform.position, connectedAnchor.transform.position);
            //Debug.Log(string.Format("{0}'s angle around {1}: {2}.", connectedAnchor, this, angle));

            WallConnection newConnection = new WallConnection(this, connectedAnchor, connectedSegment, newConnectionsAngle);
            connections.SortedInsert(newConnection, (existingConnection, newConnection) => existingConnection.Angle > newConnection.Angle);
            connectionsFromAnchorsDict.Add(connectedAnchor, newConnection);
            connectionsFromSegmentsDict.Add(connectedSegment, newConnection);
        }
        public void AddConnection(WallSegment connectedSegment)
        {
            WallAnchor connectedAnchor = connectedSegment.OtherAnchor(this);
            AddConnection(connectedAnchor, connectedSegment);
        }

        private void DeleteConnection(WallConnection targetConnection)
        {
            connectionsFromAnchorsDict.Remove(targetConnection.ConnectedAnchor);
            connectionsFromSegmentsDict.Remove(targetConnection.ConnectedSegment);
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

        private void EditConnection(WallConnection targetConnection, WallAnchor newAnchor, WallSegment newSegment, bool recalculateAngle = false)
        {
            connectionsFromAnchorsDict.Remove(targetConnection.ConnectedAnchor);
            connectionsFromSegmentsDict.Remove(targetConnection.ConnectedSegment);

            targetConnection.ConnectedAnchor = newAnchor;
            targetConnection.ConnectedSegment = newSegment;

            connectionsFromAnchorsDict.Add(newAnchor, targetConnection);
            connectionsFromSegmentsDict.Add(newSegment, targetConnection);

            if (recalculateAngle)
            {
                connections.Remove(targetConnection);

                float newConnectionsAngle = MathHelpers.GetNormalizedAngleBetween(transform.position, newAnchor.transform.position);
                connections.SortedInsert(targetConnection, (existingConnection, newConnection) => existingConnection.Angle > newConnection.Angle);
            }
        }

        public void ReplaceConnection(WallAnchor targetAnchor, WallAnchor newAnchor, WallSegment newSegment, bool recalculateAngle = false)
        {
            WallConnection targetConnection = connectionsFromAnchorsDict[targetAnchor];
            if (targetConnection != null)
            {
                EditConnection(targetConnection, newAnchor, newSegment, recalculateAngle);
            }
        }
        public void ReplaceConnection(WallSegment targetSegment, WallAnchor newAnchor, WallSegment newSegment, bool recalculateAngle = false)
        {
            WallConnection targetConnection = connectionsFromSegmentsDict[targetSegment];
            if (targetConnection != null)
            {
                EditConnection(targetConnection, newAnchor, newSegment, recalculateAngle);
            }
        }


        

        public bool IsConnectedTo(WallAnchor targetAnchor)
        {
            return connectionsFromAnchorsDict.ContainsKey(targetAnchor);
        }
        public bool IsConnectedTo(WallSegment targetSegment)
        {
            return connectionsFromSegmentsDict.ContainsKey(targetSegment);
        }

        public WallConnection GetConnectionTo(WallAnchor targetAnchor)
        {
            if (IsConnectedTo(targetAnchor))
            {
                return connectionsFromAnchorsDict[targetAnchor];
            }
            else
            {
                return null;
            }
        }
        public WallConnection GetConnectionTo(WallSegment targetSegment)
        {
            if (IsConnectedTo(targetSegment))
            {
                return connectionsFromSegmentsDict[targetSegment];
            }
            else
            {
                return null;
            }
        }

        public WallConnection? GetConnectionPreceding(WallAnchor targetAnchor)
        {
            if (connections.Count > 1)
            {
                WallConnection targetConnection = GetConnectionTo(targetAnchor);

                if (targetConnection != null)
                {
                    int index = connections.FindIndex(x => x == targetConnection);
                    return connections[(index - 1 + connections.Count) % connections.Count];
                }
            }
            // Else,
            return null;
        }
        public WallConnection? GetConnectionFollowing(WallAnchor targetAnchor)
        {
            if (connections.Count > 1)
            {
                WallConnection targetConnection = GetConnectionTo(targetAnchor);

                if (targetConnection != null)
                {
                    int index = connections.FindIndex(x => x == targetConnection);
                    return connections[(index + 1) % connections.Count];
                }
            }
            // Else,
            return null;
        }

        public WallConnection? GetConnectionPreceding(WallSegment targetSegment)
        {
            if (connections.Count > 1)
            {
                WallConnection targetConnection = GetConnectionTo(targetSegment);

                if (targetConnection != null)
                {
                    int index = connections.FindIndex(x => x == targetConnection);
                    return connections[(index - 1 + connections.Count) % connections.Count];
                }
            }
            // Else,
            return null;
        }
        public WallConnection? GetConnectionFollowing(WallSegment targetSegment)
        {
            if (connections.Count > 1)
            {
                WallConnection targetConnection = GetConnectionTo(targetSegment);

                if (targetConnection != null)
                {
                    int index = connections.FindIndex(x => x == targetConnection);
                    return connections[(index + 1) % connections.Count];
                }
            }
            // Else,
            return null;
        }

        public WallAnchor? GetAnchorPreceding(WallAnchor targetAnchor)
        {
            WallConnection? prevConnection = GetConnectionPreceding(targetAnchor);
            if (prevConnection != null)
            {
                return prevConnection.ConnectedAnchor;
            }
            else
            {
                return null;
            }
        }
        public WallAnchor? GetAnchorFollowing(WallAnchor targetAnchor)
        {
            WallConnection? nextConnection = GetConnectionFollowing(targetAnchor);
            if (nextConnection != null)
            {
                return nextConnection.ConnectedAnchor;
            }
            else
            {
                return null;
            }
        }

        public WallSegment? GetSegmentPreceding(WallSegment targetSegment)
        {
            WallConnection? prevConnection = GetConnectionPreceding(targetSegment);
            if (prevConnection != null)
            {
                return prevConnection.ConnectedSegment;
            }
            else
            {
                return null;
            }
        }
        public WallSegment? GetSegmentFollowing(WallSegment targetSegment)
        {
            WallConnection? nextConnection = GetConnectionFollowing(targetSegment);
            if (nextConnection != null)
            {
                return nextConnection.ConnectedSegment;
            }
            else
            {
                return null;
            }
        }



        // -> SELECTION
        public void Select()
        {
            myMaterial.color = colourSelected;
        }
        public void Unselect()
        {
            myMaterial.color = colourDefault;
        }






        // -> OTHER
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


#if DEBUG
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Handles.Label(gameObject.transform.position, ID.ToString());
        }
#endif
    }

}
