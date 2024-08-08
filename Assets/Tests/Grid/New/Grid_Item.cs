using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_Item : MonoBehaviour
    {
        // VARIABLES
        // Components
        [Header("Components")]
        protected MeshCollider myMeshCollider;
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
        [SerializeField] protected Grid_ItemManager myParentGridItems;
        [SerializeField] protected Grid_Handle myCurrentHandle;

        // Keep track of activity here?
        // Keep track of neighbhors here?



        // PROPERTIES
        public CoordPair MyGridCoordinates { get { return myGridCoordinates; } set { myGridCoordinates = value; } }
        public Grid_Base MyParentGrid { get { return myParentGrid; } set { myParentGrid = value; } }
        public Grid_ItemManager MyParentGridItems { get { return myParentGridItems; } set { myParentGridItems = value; } }
        public Grid_Handle MyCurrentHandle { get { return myCurrentHandle; } set { myCurrentHandle = value; } }



        // METHODS

        // SET UP
        protected void Init()
        {
            myMeshCollider = GetComponent<MeshCollider>();
            myMeshRenderer = GetComponent<MeshRenderer>();
        }
        public void SetParameters(Grid_Base parentGrid, Grid_ItemManager parentGridItems, CoordPair gridCoordinates, Vector3 relativePosition, float tileSize = 1f, bool useVisuals = true)
        {
            if (!useVisuals)
                DeactivateVisualsAndCollider();
            else
                ActivateVisualsAndCollider();

            myParentGrid = parentGrid;
            myParentGridItems = parentGridItems;

            myGridCoordinates = gridCoordinates;
            transform.position = relativePosition;
            myRelativeSize = tileSize;

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
            if (myMeshCollider)
            {
                myMeshCollider.enabled = true;
            }
        }
        public void DeactivateCollider()
        {
            if (myMeshCollider)
            {
                myMeshCollider.enabled = false;
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
            Gizmos.DrawSphere(transform.position, itemGizmoRadius * myRelativeSize);
        }

        protected void Awake()
        {
            Init();
        }


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Grid_Item {0}", myGridCoordinates);
        }
    }
}