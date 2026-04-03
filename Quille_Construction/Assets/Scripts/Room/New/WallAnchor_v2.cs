using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    public partial class WallAnchor_v2 : MonoBehaviour, IPointerDownHandler, ISelectable
    {
        // VARIABLES
        private Renderer myRenderer;
        private Material myMaterial;
        private ControlArrow myControlArrow;

        [Header("Parameters")]
        [SerializeField] private Color colourDefault = Color.white;
        [SerializeField] private Color colourSelected = Color.blue;

        [Header("Data")]
        [SerializeField] private int id;
        [SerializeField] private float height = 1f;
        [SerializeField] private List<WallAnchor_v2> connections;
        [SerializeField] private SerializedDictionary<WallAnchor_v2, float> connectionAngles;

        [Header("Runtime")]
        [SerializeField] private bool isSelected;


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

                NotifyParameterUpdated();
            }
        }
        public List<WallAnchor_v2> Connections { get { return connections; } }

        public bool IsSelected { get { return isSelected; } }



        // EVENTS
        public event ItemClicked<WallAnchor_v2> OnClicked;
        public event ItemParametersUpdated<WallAnchor_v2> OnParameterUpdated;
        // Also one thrown when parameters are updated?



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

        public void OnControlArrowAdjustment(Vector2 cursorPosDelta)
        {
            float result = cursorPosDelta.y * -0.03f;
            Height += result;
        }


        // INIT
        public void Init(int id, float height = 1f)
        {
            // Name the game object.
            gameObject.name = string.Format("WallAnchor {0} {1}", id, transform.position);

            // Fetch components.
            myRenderer = gameObject.GetComponent<Renderer>();
            myMaterial = myRenderer.material;
            myControlArrow = gameObject.GetComponentInChildren<ControlArrow>();

            myControlArrow.OnDragged += OnControlArrowAdjustment;
            
            // Set parameters.
            this.id = id;
            this.height = height;
            connections = new List<WallAnchor_v2>();
            connectionAngles = new SerializedDictionary<WallAnchor_v2, float>();

            NotifyParameterUpdated();
        }


        // UTILITY

        // -> SELECTION
        public void Select()
        {
            isSelected = true;
            myMaterial.color = colourSelected;
            myControlArrow.gameObject.SetActive(true);
        }
        public void Unselect()
        {
            isSelected = false;
            myMaterial.color = colourDefault;
            myControlArrow.gameObject.SetActive(false);
        }

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
        public void NotifyParameterUpdated()
        {
            // Visual update.
            UpdateGameObjectHeight();

            // Throw event to notify relevant wall segments.
            OnParameterUpdated?.Invoke(this);
        }

        private void UpdateGameObjectHeight()
        {
            Vector3 scale = gameObject.transform.localScale;
            scale.y = height;
            gameObject.transform.localScale = scale;
        }


        // BUILT IN
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClicked?.Invoke(this, eventData.button);
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

