using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGrid
{
    [System.Serializable]
    public class Grid_ItemGOManager : Grid_ItemManager
    {
        // VARIABLES 

        // Configurations
        [Header("Config - Source Elements")]
        // [SerializeField] protected bool useVisuals = true; 
        [SerializeField] protected GameObject itemPrefab;

        // References
        [Header("References")]
        [SerializeField] protected Grid_ItemGO[,] myGridItems;
        [SerializeField] protected Grid_ItemGO myActiveItem;



        // EVENTS
        public event GridItemGOClicked OnItemClicked; // necessary?



        // METHODS

        // SET UP
        public override void Init(Grid_Base parentGrid, int gridLengthX, int gridLengthZ, float tileSize, float itemSize, Vector3 offset)
        {
            base.Init(parentGrid, gridLengthX, gridLengthZ, tileSize, itemSize, offset);

            // Create internal items.
            GenerateItems();

            // Populate boundsGizmoPoints.
            boundsGizmoVertexPos = new Vector3[4];
            PopulateBoundsGizmoPoints();

            // Register to own event(s).
            this.OnItemClicked += OnMyItemClicked; // Necessary?
        }

        // Separated submethods for ease of overriding.
        protected override void GenerateItems()
        {
            myGridItems = new Grid_ItemGO[myResolutionX + 1, myResolutionZ + 1];
            for (int x = 0; x <= myResolutionX; x++)
            {
                for (int z = 0; z <= myResolutionZ; z++)
                {
                    Vector3 relativePosition = new Vector3(x * myTileSize, 0, z * myTileSize) + myItemOffset;
                    relativePosition.y *= myTileSize;

                    myGridItems[x, z] = Instantiate(itemPrefab, transform).GetComponent<Grid_ItemGO>();
                    myGridItems[x, z].SetParameters(myParentGrid, this, new CoordPair(x, z), relativePosition, myItemSize);
                    myGridItems[x, z].OnItemClicked += OnMyItemClicked;
                }
            }
        }
        protected override void PopulateBoundsGizmoPoints()
        {
            boundsGizmoVertexPos[0] = myGridItems[0, 0].transform.position + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[1] = myGridItems[myResolutionX, 0].transform.position + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[2] = myGridItems[myResolutionX, myResolutionZ].transform.position + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[3] = myGridItems[0, myResolutionZ].transform.position + new Vector3(0, boundsGizmoHeight, 0);
        }

        public void CreateHandles(Grid_ItemGO centerItem, int doBeyond)
        {
            myActiveItem = centerItem;

            // TODO: better handle reshuffling.
            // TODO: better array resizing.

            int fullSpawn = doBeyond * 2 + 1;
            if (myHandles == null)
            {
                myHandles = new Grid_Handle[fullSpawn, fullSpawn];
            }

            for (int x = -doBeyond; x <= doBeyond; x++)
            {
                for (int z = -doBeyond; z <= doBeyond; z++)
                {
                    int currentXCoord = centerItem.MyGridCoordinates.x + x;
                    int currentZCoord = centerItem.MyGridCoordinates.z + z;

                    // Don't generate a handle if the target point is out of range.
                    if (currentXCoord >= 0 && currentXCoord <= myResolutionX && currentZCoord >= 0 && currentZCoord <= myResolutionZ)
                    {
                        if (myHandles[x + doBeyond, z + doBeyond] == null)
                        {
                            myHandles[x + doBeyond, z + doBeyond] = Instantiate(handlePrefab, transform).GetComponent<Grid_Handle>();
                            myHandles[x + doBeyond, z + doBeyond].OnHandleClicked += OnMyHandleClicked;
                        }
                        myHandles[x + doBeyond, z + doBeyond].SetReferencesAndPosition(myParentGrid, this, myGridItems[currentXCoord, currentZCoord]);
                        myGridItems[currentXCoord, currentZCoord].MyCurrentHandle = myHandles[x + doBeyond, z + doBeyond];

                        // TODO: deactivate previous item.
                    }
                }
            }
        }


        // EVENTS
        protected void OnMyItemClicked(Grid_ItemGO clickedItem)
        {
            Debug.Log(string.Format("{0} aknowledges that its item {1} was clicked.", gameObject.name, clickedItem.name));

            //CreateHandles(clickedItem, 1);
        }

        protected void OnMyHandleClicked(Grid_Handle clickedHandle)
        {
            Debug.Log(string.Format("{0} aknowledges that its handle {1}, curently attached to {2}, was clicked.", gameObject.name, clickedHandle.name, clickedHandle.MyCurrentItem.name));
        }


        // UTILITY


        // BUILT IN
    }
}

