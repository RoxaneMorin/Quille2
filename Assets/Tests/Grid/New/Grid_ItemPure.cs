using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_ItemPure
    {
        // VARIABLES

        // Coordinates
        [Header("Coordinates")]
        [SerializeField] protected CoordPair myGridCoordinates;
        [SerializeField] protected Matrix4x4 myTransformMatrix;
        [SerializeField] protected float myRelativeSize; // is this necessary?

        // References
        [Header("References")]
        [SerializeField] protected Grid_Base myParentGrid; // is this necessary?
        [SerializeField] protected Grid_ItemManager myParentGridManager;
        [SerializeField] protected Grid_Handle myCurrentHandle;

        // Status
        [Header("Status")]
        [SerializeField] protected bool amIActive = false;
        [SerializeField] protected bool isANeighborActive = false;



        // PROPERTIES
        public CoordPair MyGridCoordinates { get { return myGridCoordinates; } set { myGridCoordinates = value; } }
        public Matrix4x4 MyTransformMatrix { get { return myTransformMatrix; } set { myTransformMatrix = value; } }
        public Vector3 MyPostion { get { return new Vector3(myTransformMatrix.m03, myTransformMatrix.m13, myTransformMatrix.m23); } }
        public float MyRelativeSize { get { return myRelativeSize; } set { myRelativeSize = value; } }

        public Grid_Base MyParentGrid { get { return myParentGrid; } set { myParentGrid = value; } }
        public Grid_ItemManager MyParentGridItemManager { get { return myParentGridManager; } set { myParentGridManager = value; } }
        public Grid_Handle MyCurrentHandle { get { return myCurrentHandle; } set { myCurrentHandle = value; } }
        



        // CONSTRUCTOR
        public Grid_ItemPure() { }
        public Grid_ItemPure(Grid_Base parentGrid, Grid_ItemManager parentGridItemManager, CoordPair gridCoordinates, Matrix4x4 transform, float tileSize = 1f)
        {
            myParentGrid = parentGrid;
            myParentGridManager = parentGridItemManager;

            myGridCoordinates = gridCoordinates;
            myTransformMatrix = transform;
            myRelativeSize = tileSize;

            //Debug.Log("Grid Item created at " + myGridCoordinates);
        }



        // METHODS

        // OVERRIDES
        public override string ToString()
        {
            return string.Format("Grid_ItemPure {0}", myGridCoordinates);
        }
    }
}