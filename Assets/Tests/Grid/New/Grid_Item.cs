using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_Item : MonoBehaviour
    {
        // VARIABLES

        // References
        [Header("References")]
        [SerializeField] protected Grid_Base myParentGrid; // is this necessary?
        [SerializeField] protected Grid_Items myParentGridItems;
        [SerializeField] protected Grid_Handle myCurrentHandle;

        // Coordinates
        [Header("Coordinates")]
        [SerializeField] protected CoordPair myGridCoordinates;
        // Keep track of activity here?
        // Keep track of neighbhors here?


        // PROPERTIES
        public Grid_Base MyParentGrid { get { return myParentGrid; } set { myParentGrid = value; } }
        public Grid_Items MyParentGridItems { get { return myParentGridItems; } set { myParentGridItems = value; } }
        public Grid_Handle MyCurrentHandle { get { return myCurrentHandle; } set { myCurrentHandle = value; } }


        // CONSTRUCTOR
        public Grid_Item() { }
        public Grid_Item(Grid_Base myParentGrid, Grid_Items myParentGridItems, CoordPair myGridCoordinates)
        {
            this.myParentGrid = myParentGrid;
            this.myParentGridItems = myParentGridItems;
            this.myGridCoordinates = myGridCoordinates;

            //Debug.Log("Grid Item created at " + myGridCoordinates);
        }
        public Grid_Item(Grid_Base myParentGrid, Grid_Items myParentGridItems, CoordPair myGridCoordinates, Vector3 myRelativePosition) 
            : this (myParentGrid, myParentGridItems, myGridCoordinates)
        {
            transform.position = myRelativePosition;
        }



        // METHODS


    }
}