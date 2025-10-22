using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGrid
{
    [System.Serializable]
    public class Grid_ItemManager : MonoBehaviour
    {
        // VARIABLES 
        [Header("Config - Collider")]
        [SerializeField] protected bool useCollider = false;
        [SerializeField] protected float colliderHeight = 0.1f;

        [Header("Config - Handles")]
        [SerializeField] protected GameObject handlePrefab;
        [Space]
        [SerializeField] [ColorUsage(showAlpha:false)] protected Color handleDefaultColour;
        [SerializeField] protected float handleDefaultOpacity = 0.5f;
        [SerializeField] [ColorUsage(showAlpha: false)] protected Color handleSelectedColour;
        [SerializeField] protected float handleSelectedOpacity = 1f;
        [SerializeField] [ColorUsage(showAlpha: false)] protected Color handleSecondarySelectionColour;
        [SerializeField] protected float handleSecondarySelectionOpacity = 0.75f;

        [Header("Config - Gizmos")]
        // TODO: replace by actual "Bounds"
        [SerializeField] protected Vector3[] boundsGizmoVertexPos;
        [SerializeField] protected Color boundsGizmoColour;
        [SerializeField] protected float boundsGizmoHeight = 0.5f;

        [Header("Config - Cursor")]
        [SerializeField] protected float cursorMovementNoticeDistance = 1f;

        // References
        [Header("References")]
        [SerializeField] protected Grid_Base myParentGrid;
        [SerializeField] protected int myGridsResolutionX;
        [SerializeField] protected int myGridsResolutionZ;
        [SerializeField] protected float myTileSize;
        [SerializeField] protected int myResolutionX;
        [SerializeField] protected int myResolutionZ;
        [SerializeField] protected float myItemSize;
        [SerializeField] protected GridItemPositioning myItemPositioning;
        [SerializeField] protected Vector3 myItemOffset;
        [Space]
        [SerializeField] protected BoxCollider myCollider;
        [SerializeField] protected Grid_Handle[,] myHandles;



        // METHODS

        // SET UP
        public virtual void Init(Grid_Base parentGrid, int gridResolutionX, int gridResolutionZ, float tileSize, float itemSize, Vector3 itemOffset)
        {
            myParentGrid = parentGrid;

            myGridsResolutionX = gridResolutionX;
            myGridsResolutionZ = gridResolutionZ;

            if (myItemPositioning == GridItemPositioning.AtCorner)
            {
                myResolutionX = gridResolutionX + 1;
                myResolutionZ = gridResolutionZ + 1;
            }
            else
            {
                myResolutionX = gridResolutionX;
                myResolutionZ = gridResolutionZ;
            }

            myTileSize = tileSize;
            myItemSize = itemSize;
            myItemOffset = new Vector3 (itemOffset.x * myTileSize, itemOffset.y, itemOffset.z * myTileSize);

            if (useCollider)
            {
                GenerateCollider();
            }
        }

        protected void GenerateCollider() 
        {
            myCollider = gameObject.AddComponent<BoxCollider>();

            // Calculate and set collider center.
            float centerX = (myGridsResolutionX * myTileSize) /2;
            float centerZ = (myGridsResolutionZ * myTileSize) /2;
            myCollider.center = new Vector3(centerX, 0, centerZ);

            // Calculate and set collider size.
            myCollider.size = new Vector3(myGridsResolutionX * myTileSize, colliderHeight, myGridsResolutionZ * myTileSize);
        }

        protected virtual void GenerateItems() { }


        // UTILITY
        protected virtual void PopulateBoundsGizmoPoints() { }


        // BUILT IN
        protected virtual void OnDrawGizmos()
        {
            if (boundsGizmoVertexPos != null)
            {
                // Draw a square to represent coverage.
                PopulateBoundsGizmoPoints();
                Gizmos.color = boundsGizmoColour;
                Gizmos.DrawLineStrip(boundsGizmoVertexPos, true);
            }
        }
    }
}



