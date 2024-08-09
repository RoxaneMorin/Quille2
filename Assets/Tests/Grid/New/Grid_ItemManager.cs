using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_ItemManager : MonoBehaviour
    {
        // VARIABLES 

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
        [SerializeField] protected Vector3[] boundsGizmoVertexPos;
        [SerializeField] protected Color boundsGizmoColour;
        [SerializeField] protected float boundsGizmoHeight = 0.5f;

        [Header("Config - Cursor")]
        [SerializeField] protected float cursorMovementNoticeDistance = 1f;

        // References
        [Header("References")]
        [SerializeField] protected Grid_Base myParentGrid;
        [SerializeField] protected int myLengthX;
        [SerializeField] protected int myLengthZ;
        [SerializeField] protected float myRelativeSize;
        [SerializeField] protected Vector3 myItemOffset;
        [Space]
        [SerializeField] protected Grid_Handle[,] myHandles;
         


        // METHODS

        // SET UP
        public virtual void Init(Grid_Base parentGrid, int gridLengthX, int gridLengthZ, float relativeSize, Vector3 offset)
        {
            myParentGrid = parentGrid;

            myLengthX = gridLengthX;
            myLengthZ = gridLengthZ;
            myRelativeSize = relativeSize;
            myItemOffset = offset;
        }

        // Separated submethods for ease of overriding.
        protected virtual void CreateInternalItems(Grid_Base parentGrid, int gridLengthX, int gridLengthZ, float relativeSize, Vector3 offset) { }
        protected virtual void PopulateBoundsGizmoPoints() { }


        // UTILITY


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



