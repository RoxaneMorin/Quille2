using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_ItemManager : MonoBehaviour
    {
        // VARIABLES 

        // Configurations
        [Header("Config - Source Elements")]
        [SerializeField] protected GameObject itemPrefab;
        //[SerializeField] protected bool useVisuals = true;

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

        //[Header("Config - Cursor")] // Not sure if useful.
        //[SerializeField] protected float cursorMovementNoticeDistance = 1f;
        //[SerializeField] protected float cursorFadeDistance = 100f;

        // References
        [Header("References")]
        [SerializeField] protected Grid_Base myParentGrid;
        [SerializeField] protected int myLengthX;
        [SerializeField] protected int myLengthZ;
        [SerializeField] protected float myRelativeSize;
        [SerializeField] protected Vector3 myOffset;
        [Space]
        [SerializeField] protected Grid_Item[,] myGridItems;
        [SerializeField] protected Grid_Handle[,] myHandles;
        //[Space]
        // Active item(s) info?




        // METHODS

        // SET UP
        public void Init(Grid_Base parentGrid, int gridLengthX, int gridLengthZ, float relativeSize, Vector3 offset)
        {
            myLengthX = gridLengthX;
            myLengthZ = gridLengthZ;
            myRelativeSize = relativeSize;
            myOffset = offset;

            // Create internal items.
            CreateInternalItems(parentGrid, gridLengthX, gridLengthZ, relativeSize, myOffset);

            // Populate boundsGizmoPoints.
            boundsGizmoVertexPos = new Vector3[4];
            PopulateBoundsGizmoPoints();
        }

        // Separated submethods for ease of overriding.
        protected void CreateInternalItems(Grid_Base parentGrid, int gridLengthX, int gridLengthZ, float relativeSize, Vector3 offset)
        {
            myGridItems = new Grid_Item[gridLengthX + 1, gridLengthZ + 1];
            for (int x = 0; x <= gridLengthX; x++)
            {
                for (int z = 0; z <= gridLengthZ; z++)
                {
                    Vector3 relativePosition = new Vector3(x * relativeSize, 0, z * relativeSize) + offset;
                    relativePosition.y *= relativeSize;

                    myGridItems[x, z] = Instantiate(itemPrefab, transform).GetComponent<Grid_Item>();
                    myGridItems[x, z].SetParameters(parentGrid, this, new CoordPair(x, z), relativePosition, relativeSize);
                }
            }
        }
        protected void PopulateBoundsGizmoPoints()
        {
            boundsGizmoVertexPos[0] = myGridItems[0, 0].transform.position + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[1] = myGridItems[myLengthX, 0].transform.position + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[2] = myGridItems[myLengthX, myLengthZ].transform.position + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[3] = myGridItems[0, myLengthZ].transform.position + new Vector3(0, boundsGizmoHeight, 0);
        }

        public void CreateHandles(CoordPair centerPoint, int doBeyond)
        {
            int fullSpawn = doBeyond * 2 + 1;
            myHandles = new Grid_Handle[fullSpawn, fullSpawn];

            for (int x = 0; x < fullSpawn; x++)
            {
                for (int z = 0; z < fullSpawn; z++)
                {
                    //myHandles[x, z] = Instantiate(handlePrefab, transform).GetComponent<Grid_Handle>();
                    //myHandles[x, z].SetReferencesAndPosition(myParentGrid, this, myGridItems[x, z]);
                    //myGridItems[x, z].MyCurrentHandle = myHandles[x, z];
                }
            }
        }


        // UTILITY


        // BUILT IN
        protected void OnDrawGizmos()
        {
            // Draw a square to represent coverage.
            PopulateBoundsGizmoPoints();
            Gizmos.color = boundsGizmoColour;
            Gizmos.DrawLineStrip(boundsGizmoVertexPos, true);
        }

        protected void FixedUpdate()
        {
        }
        protected void Update()
        {
        }
    }
}



