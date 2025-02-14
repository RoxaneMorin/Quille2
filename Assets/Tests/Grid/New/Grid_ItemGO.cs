using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_ItemGO : MonoBehaviour
    {
        // VARIABLES
        // Components
        [Header("Components")]
        protected Collider myCollider;
        protected MeshRenderer myMeshRenderer;

        // Configuration
        [Header("Config - Gizmos")]
        [SerializeField] protected Color itemGizmoColour;
        [SerializeField] protected float itemGizmoRadius = 0.05f;

        // Coordinates
        [Header("Coordinates")]
        [SerializeField] protected CoordPair myGridCoordinates;
        [SerializeField] protected float myRelativeSize;

        // References
        [Header("References")]
        [SerializeField] protected Grid_Base myParentGrid; // is this necessary?
        [SerializeField] protected Grid_ItemGOManager myParentGridItemManager;
        [SerializeField] protected Grid_Handle myCurrentHandle;

        // Status
        [Header("Status")]
        [SerializeField] protected bool amIActive = false;
        [SerializeField] protected bool isANeighborActive = false;



        // PROPERTIES
        public CoordPair MyGridCoordinates { get { return myGridCoordinates; } set { myGridCoordinates = value; } }
        public float MyRelativeSize { get { return myRelativeSize; } set { myRelativeSize = value; } }

        public Grid_Base MyParentGrid { get { return myParentGrid; } set { myParentGrid = value; } }
        public Grid_ItemGOManager MyParentGridItemManager { get { return myParentGridItemManager; } set { myParentGridItemManager = value; } }
        public Grid_Handle MyCurrentHandle { get { return myCurrentHandle; } set { myCurrentHandle = value; } }



        // EVENTS
        public event GridItemGOClicked OnItemClicked;



        // METHODS

        // SET UP
        protected void Init()
        {
            myCollider = GetComponent<MeshCollider>();
            myMeshRenderer = GetComponent<MeshRenderer>();
        }
        public void SetParameters(Grid_Base parentGrid, Grid_ItemGOManager parentGridItemManager, CoordPair gridCoordinates, Vector3 relativePosition, float relativeSize, bool useVisuals = true)
        {
            if (!useVisuals)
                DeactivateVisualsAndCollider();
            else
                ActivateVisualsAndCollider();

            myParentGrid = parentGrid;
            myParentGridItemManager = parentGridItemManager;

            myGridCoordinates = gridCoordinates;
            transform.localPosition = relativePosition;
            myRelativeSize = relativeSize;
            transform.localScale *= myRelativeSize;

            gameObject.name = this.ToString();

            //Debug.Log("Grid Item created at " + myGridCoordinates);
        }


        // UTILITY
        public void ActivateVisuals()
        {
            if (myMeshRenderer)
            {
                myMeshRenderer.enabled = true;
            }
        }
        public void DeactivateVisuals()
        {
            if (myMeshRenderer)
            {
                myMeshRenderer.enabled = false;
            }
        }

        public void ActivateCollider()
        {
            if (myCollider)
            {
                myCollider.enabled = true;
            }
        }
        public void DeactivateCollider()
        {
            if (myCollider)
            {
                myCollider.enabled = false;
            }
        }

        public void ActivateVisualsAndCollider()
        {
            ActivateVisuals();
            ActivateCollider();
        }
        public void DeactivateVisualsAndCollider()
        {
            DeactivateVisuals();
            DeactivateCollider();
        }


        // BUILT IN
        protected void OnDrawGizmos()
        {
            Gizmos.color = itemGizmoColour;
            Gizmos.DrawSphere(transform.position, itemGizmoRadius * myRelativeSize * transform.localScale.x);
        }

        protected void Awake()
        {
            Init();
        }

        // MOUSE CLICK
        protected void OnMouseDown()
        {
            //amIActive = !amIActive;
            Debug.Log(string.Format("Mouse down on {0}.", gameObject));

            // Do Event.
            OnItemClicked?.Invoke(this);
        }


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Grid_ItemMB {0}", myGridCoordinates);
        }
    }
}