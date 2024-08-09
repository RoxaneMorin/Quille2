using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
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



        // METHODS

        // SET UP
        public override void Init(Grid_Base parentGrid, int gridLengthX, int gridLengthZ, float relativeSize, Vector3 offset)
        {
            base.Init(parentGrid, gridLengthX, gridLengthZ, relativeSize, offset);

            // Create internal items.
            CreateInternalItems(parentGrid, gridLengthX, gridLengthZ, relativeSize, offset);

            // Populate boundsGizmoPoints.
            boundsGizmoVertexPos = new Vector3[4];
            PopulateBoundsGizmoPoints();
        }

        // Separated submethods for ease of overriding.
        protected override void CreateInternalItems(Grid_Base parentGrid, int gridLengthX, int gridLengthZ, float relativeSize, Vector3 offset)
        {
            myGridItems = new Grid_ItemGO[gridLengthX + 1, gridLengthZ + 1];
            for (int x = 0; x <= gridLengthX; x++)
            {
                for (int z = 0; z <= gridLengthZ; z++)
                {
                    Vector3 relativePosition = new Vector3(x * relativeSize, 0, z * relativeSize) + offset;
                    relativePosition.y *= relativeSize;

                    myGridItems[x, z] = Instantiate(itemPrefab, transform).GetComponent<Grid_ItemGO>();
                    myGridItems[x, z].SetParameters(parentGrid, this, new CoordPair(x, z), relativePosition, relativeSize);
                }
            }
        }
        protected override void PopulateBoundsGizmoPoints()
        {
            boundsGizmoVertexPos[0] = myGridItems[0, 0].transform.position + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[1] = myGridItems[myLengthX, 0].transform.position + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[2] = myGridItems[myLengthX, myLengthZ].transform.position + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[3] = myGridItems[0, myLengthZ].transform.position + new Vector3(0, boundsGizmoHeight, 0);
        }

        public void CreateHandles(Grid_ItemGO centerItem, int doBeyond)
        {
            myActiveItem = centerItem;

            int fullSpawn = doBeyond * 2 + 1;
            myHandles = new Grid_Handle[fullSpawn, fullSpawn];

            for (int x = -doBeyond; x <= doBeyond; x++)
            {
                for (int z = -doBeyond; z <= doBeyond; z++)
                {

                    int currentXCoord = centerItem.MyGridCoordinates.x + x;
                    int currentZCoord = centerItem.MyGridCoordinates.z + z;

                    //Debug.Log(string.Format("({0}, {1})", currentXCoord, currentZCoord));

                    // TODO: verify the coordinates exist in myGridItems
                    myHandles[x + doBeyond, z + doBeyond] = Instantiate(handlePrefab, transform).GetComponent<Grid_Handle>();
                    myHandles[x + doBeyond, z + doBeyond].SetReferencesAndPosition(myParentGrid, this, myGridItems[currentXCoord, currentZCoord]);
                    myGridItems[currentXCoord, currentZCoord].MyCurrentHandle = myHandles[x + doBeyond, z + doBeyond];
                }
            }
        }


        // UTILITY


        // BUILT IN
    }
}

