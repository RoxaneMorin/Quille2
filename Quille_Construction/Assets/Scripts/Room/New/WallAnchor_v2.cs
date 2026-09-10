using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    // TODO: should each wall anchor have its own control arrow?
    // TODO: can also set height with the scroll wheel?

    // GameObject representing the start or end point of a segment of wall.
    public partial class WallAnchor_v2 : MonoBehaviour, IComparable, IPointerClickAndHoverHandler, ISelectable, IArrowControllable
    {
        // VARIABLES/PARAMETERS
        [Header("References")]
        [SerializeField] protected MeshFilter myMeshFilter;
        [SerializeField] protected MeshRenderer myMeshRenderer;
        [SerializeField] protected Material myMaterial;
        [SerializeField] protected BoxCollider myCollider;

        [Header("Parameters")]
        [SerializeField] protected Color colourDefault = Color.white;
        [SerializeField] protected Color colourHovered = Color.cyan;
        [SerializeField] protected Color colourSelected = Color.blue;
        [SerializeField] protected float colliderSizePadding = 0.1f;

        [Header("Data")]
        [SerializeField] protected int id;
        [SerializeField] protected float height = 1f;
        [SerializeField] protected List<WallAnchor_v2> connections;
        [SerializeField] protected SerializedDictionary<WallAnchor_v2, float> connectionAngles;
        // TODO: should we also keep track of the corresponding wall segments here?

        [Header("Runtime")]
        [SerializeField] protected bool isSelected;


        // PROPERTIES
        public int ID { get { return id; } }
        public float Height
        {
            get { return height; }
            set
            {
                if (value < Constants_Building.MIN_WALL_ANCHOR_HEIGHT)
                {
                    height = Constants_Building.MIN_WALL_ANCHOR_HEIGHT;
                }
                else if (value > Constants_Building.MAX_WALL_ANCHOR_HEIGHT)
                {
                    height = Constants_Building.MAX_WALL_ANCHOR_HEIGHT;
                }
                else
                {
                    height = value;
                }

                // Propagate this update;
                UpdateGameObjectHeight();
                NotifyParameterUpdated();
            }
        }
        public List<WallAnchor_v2> Connections { get { return connections; } }

        public bool IsSelected { get { return isSelected; } }


        public Vector3 PosAtBase
        {
            get { return transform.position; }
        }
        public Vector3 PosAtTop
        {
            get { return transform.position + new Vector3(0, Height, 0); }
        }



        // EVENTS
        public event ItemClicked<WallAnchor_v2> OnClicked;
        public event ItemParametersUpdated<WallAnchor_v2> OnParameterUpdated;



        // METHODS

        // EVENT LISTENERS
        public void OnWallAnchorSelected(WallAnchor_v2 selectedAnchor)
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

        public void OnControlArrowAdjustment(ControlArrow sourceArrow, Vector2 cursorPosDelta)
        {
            float heightDelta = cursorPosDelta.y * -0.03f;
            Height += heightDelta;

            sourceArrow.SetPositionFromTarget();
        }


        // INIT
        public void Init(int id, float height = 1f)
        {
            // Name the game object.
            gameObject.name = string.Format("WallAnchor {0} {1}", id, transform.position);

            // Fetch components.
            myMeshFilter = gameObject.GetComponent<MeshFilter>();
            myMeshRenderer = gameObject.GetComponent<MeshRenderer>();
            myMaterial = myMeshRenderer.material;
            myCollider = gameObject.GetComponent<BoxCollider>();

            // Set parameters.
            this.id = id;
            this.height = height;
            connections = new List<WallAnchor_v2>();
            connectionAngles = new SerializedDictionary<WallAnchor_v2, float>();

            // Propagate.
            UpdateGameObjectHeight();
            NotifyParameterUpdated();
        }


        // UTILITY

        // -> CONNECTIONS
        public bool IsConnectedTo(WallAnchor_v2 anchor)
        {
            return connectionAngles.ContainsKey(anchor);
        }
        public WallAnchor_v2? GetConnectionPreceding(WallAnchor_v2 targetAnchor)
        {
            if (connections.Count > 1 && connectionAngles.ContainsKey(targetAnchor))
            {
                int index = connections.FindIndex(x => x == targetAnchor);
                return connections[(index - 1 + connections.Count) % connections.Count];
            }
            // Else,
            return null;
        }
        public WallAnchor_v2? GetConnectionFollowing(WallAnchor_v2 targetAnchor)
        {
            if (connections.Count > 1 && connectionAngles.ContainsKey(targetAnchor))
            {
                int index = connections.FindIndex(x => x == targetAnchor);
                return connections[(index + 1) % connections.Count];
            }
            // Else,
            return null;
        }

        public void Connect(WallAnchor_v2 anchor)
        {
            float angle = MathHelpers.GetNormalizedAngleBetween(transform.position, anchor.transform.position);
            connections.SortedInsert(anchor, (existingAnchor, newAnchor) => connectionAngles[existingAnchor] > angle);
            connectionAngles.Add(anchor, angle);
        }
        public bool Disconnect(WallAnchor_v2 anchor)
        {
            return connections.Remove(anchor) & connectionAngles.Remove(anchor);
        }
        public void ReplaceConnection(WallAnchor_v2 existingAnchor, WallAnchor_v2 newAnchor) // Replace the given connection without recalculating its angle.
        {
            if (connectionAngles.ContainsKey(existingAnchor))
            {
                int index = connections.IndexOf(existingAnchor);
                float angle = connectionAngles[existingAnchor];

                connections[index] = newAnchor;
                connectionAngles.Remove(existingAnchor);
                connectionAngles[newAnchor] = angle;
            }
            else
            {
                Connect(newAnchor);
            }
        }


        // -> PARAMETER UPDATES
        protected void UpdateGameObjectHeight()
        {
            // TODO: do this less hackily
            Mesh myMesh = myMeshFilter.mesh;
            List<Vector3> meshVertices = new List<Vector3>();
            myMesh.GetVertices(meshVertices);

            for (int i = 0; i < meshVertices.Count; i++)
            {
                Vector3 vertex = meshVertices[i];

                if (vertex.y != 0)
                {
                    vertex.y = Height;
                    meshVertices[i] = vertex;
                }
            }

            myMesh.SetVertices(meshVertices);
            myMesh.RecalculateBounds();

            // Update collider
            myCollider.size = myMesh.bounds.size + new Vector3(colliderSizePadding, colliderSizePadding, colliderSizePadding);
            myCollider.center = myMesh.bounds.center;
        }

        public void NotifyParameterUpdated()
        {
            // Throw event to notify relevant wall segments.
            OnParameterUpdated?.Invoke(this);
        }


        // INTERFACES

        // -> SELECTION
        public void Select()
        {
            isSelected = true;
            myMaterial.color = colourSelected;
        }
        public void Unselect()
        {
            isSelected = false;
            myMaterial.color = colourDefault;
        }


        // -> POINTER
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isSelected)
            {
                myMaterial.color = colourHovered;
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isSelected)
            {
                myMaterial.color = colourDefault;
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(this, eventData.button);
        }


#if DEBUG
        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            // ID
            Vector3 idLabelPos = PosAtBase;
            idLabelPos.y -= 0.05f;
            Handles.Label(idLabelPos, string.Format("Anchor #{0}", ID));

            // Height.
            Vector3 heightLabelPos = PosAtTop;
            heightLabelPos.y += 0.075f;
            Handles.Label(heightLabelPos, string.Format("Height: {0:0.000}", height));
        }
#endif
    }
}

