using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_Items : MonoBehaviour
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
        [SerializeField] protected Color itemGizmoColour;
        [SerializeField] protected float itemGizmoRadius = 0.05f;

        [Header("Config - Cursor")] // Not sure if useful.
        [SerializeField] protected float cursorMovementNoticeDistance = 1f;
        [SerializeField] protected float cursorFadeDistance = 100f;

        // References
        [Header("References")]
        [SerializeField] protected Grid_Base myParentGrid;
        [SerializeField] protected float myTileSize;
        [SerializeField] protected int myLengthX;
        [SerializeField] protected int myLengthZ;
        [SerializeField] protected float myXOffset;
        [SerializeField] protected float myYOffset;
        [SerializeField] protected float myZOffset;
        [Space]
        [SerializeField] protected Grid_Item[,] myGridItems;
        [SerializeField] protected Grid_Handle[,] myHandles;
        //[Space]
        // Active item(s) info?

        // State Tracking
        protected Vector3 previousCursorPosition;
        protected float previousCursorPositionDelta;

        protected Vector3 previousTransformPosition;

        protected Transform previousTransform;




        // METHODS
        public void Init(Grid_Base parentGrid, int gridLengthX, int gridLengthZ, float tileSize, float yOffset, float xOffset = 0, float zOffset = 0)
        {
            myLengthX = gridLengthX;
            myLengthZ = gridLengthZ;
            myTileSize = tileSize;
            myXOffset = xOffset;
            myYOffset = yOffset;
            myZOffset = zOffset;

            // Create internal items.
            CreateInternalItems(parentGrid, gridLengthX, gridLengthZ, tileSize, yOffset, xOffset, zOffset);

            // Populate boundsGizmoPoints.
            boundsGizmoVertexPos = new Vector3[4];
            PopulateBoundsGizmoPoints(gridLengthX, gridLengthZ, tileSize, yOffset, xOffset, zOffset);
            
            // If visuals should be used, prepare them for instanced rendering.
            if (useVisuals && useThisMesh != null)
            {
                instancedRenderParameters = new RenderParams(useThisMaterial);
                instancedMeshData = new Matrix4x4[(gridLengthX + 1) * (gridLengthZ + 1)];
                PopulateInstancedRenderData(tileSize);
            }
        }
        // Separated submethods for ease of overriding.
        protected void CreateInternalItems(Grid_Base parentGrid, int gridLengthX, int gridLengthZ, float tileSize, float yOffset, float xOffset = 0, float zOffset = 0)
        {
            myGridItems = new Grid_Item[gridLengthX + 1, gridLengthZ + 1];
            for (int x = 0; x <= gridLengthX; x++)
            {
                for (int z = 0; z <= gridLengthZ; z++)
                {
                    Vector3 relativePosition = new Vector3(x * tileSize + xOffset, yOffset * tileSize, z * tileSize + zOffset);
                    myGridItems[x, z] = new Grid_Item(parentGrid, this, new CoordPair(x, z), relativePosition);
                }
            }
        }
        protected void PopulateBoundsGizmoPoints(int gridLengthX, int gridLengthZ, float tileSize, float yOffset, float xOffset, float zOffset)
        {
            // TODO: fetch values from the corner items themselves?
            boundsGizmoVertexPos[0] = new Vector3(transform.position.x + xOffset, boundsGizmoHeight * tileSize + yOffset, transform.position.z + zOffset);
            boundsGizmoVertexPos[1] = new Vector3(transform.position.x + gridLengthX * tileSize + xOffset, boundsGizmoHeight * tileSize + yOffset, transform.position.z + zOffset);
            boundsGizmoVertexPos[2] = new Vector3(transform.position.x + gridLengthX * tileSize + xOffset, boundsGizmoHeight * tileSize + yOffset, transform.position.z + gridLengthZ * tileSize + zOffset);
            boundsGizmoVertexPos[3] = new Vector3(transform.position.x + xOffset, boundsGizmoHeight * tileSize + yOffset, transform.position.z + gridLengthZ * tileSize + zOffset);
        }
        protected void PopulateInstancedRenderData(float tileSize)
        {
            int i = 0;
            foreach (Grid_Item item in myGridItems)
            {
                if (i < instancedMeshData.Length)
                {
                    instancedMeshData[i] = Matrix4x4.Translate(item.MyWorldPosition + defaultMeshOffset);
                    instancedMeshData[i] *= Matrix4x4.Rotate(transform.rotation * Quaternion.Euler(defaultMeshOrientation));
                    instancedMeshData[i] *= Matrix4x4.Scale(defaultMeshSize * tileSize);
                    i++;
                }
            }
        }
        public void CreateHandles(int dimensions)
        {
            myHandles = new Grid_Handle[dimensions, dimensions];

            //myGridTiles = Instantiate(gridTilePrefab, transform).GetComponent<Grid_Items>();

            for (int x = 0; x < dimensions; x++)
            {
                for (int z = 0; z < dimensions; z++)
                {
                    myHandles[x, z] = Instantiate(handlePrefab, transform).GetComponent<Grid_Handle>();
                    myHandles[x, z].SetReferencesAndPosition(myParentGrid, this, myGridItems[x, z]);
                    myGridItems[x, z].MyCurrentHandle = myHandles[x, z];
                }
            }
        }


        // UTILITY


        // BUILT IN
        protected void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            // Draw a square to represent coverage.
            PopulateBoundsGizmoPoints(myLengthX, myLengthZ, myTileSize, myYOffset, myXOffset, myZOffset);
            Gizmos.color = boundsGizmoColour;
            Gizmos.DrawLineStrip(boundsGizmoVertexPos, true);

            // Draw the component items.
            if (myGridItems != null)
            {
                Gizmos.color = itemGizmoColour;
                foreach (Grid_Item item in myGridItems)
                {
                    Gizmos.DrawSphere(item.MyWorldPosition, itemGizmoRadius * myTileSize);
                }
            }
        }

        protected void FixedUpdate()
        {
            previousCursorPositionDelta = Vector3.Distance(previousCursorPosition, Input.mousePosition);

            
            previousCursorPosition = Input.mousePosition;

            previousTransformPosition = transform.position;
            previousTransform = transform;
        }
        protected void Update()
        {
            if (useVisuals && useThisMesh != null)
            {
                PopulateInstancedRenderData(myTileSize);
                Graphics.RenderMeshInstanced(instancedRenderParameters, useThisMesh, 0, instancedMeshData);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log(string.Format("{0}'s WorldPosition: {1}.", gameObject.name, transform.position));
                Debug.Log(string.Format("{0}'s ScreenPosition: {1}.", gameObject.name, Camera.main.WorldToScreenPoint(transform.position)));

                Debug.Log(string.Format("{0}[0,0]'s WorldPosition: {1}.", gameObject.name, myGridItems[0, 0].MyRelativeWorldPosition));
                Debug.Log(string.Format("{0}[0,0]'s ScreenPosition: {1}.", gameObject.name, myGridItems[0, 0].MyRelativeScreenPosition));
            }
        }
    }
}



