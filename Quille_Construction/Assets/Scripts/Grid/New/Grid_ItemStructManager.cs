using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine;
using MeshGeneration;

namespace ProceduralGrid
{
    [System.Serializable]
    public class Grid_ItemStructManager : Grid_ItemManager
    {
        // VARIABLES & PROPERTIES

        // Components
        [Header("Components & References")]
        protected MeshFilter myMeshFilter;
        protected MeshRenderer myMeshRenderer;
        protected Mesh myMesh;

        public NativeArray<GridItem> myGridItems;
        public GridItem MyGridItem(int xIndex, int zIndex)
        {
            int index = xIndex + (zIndex * myResolutionX);
            return myGridItems[index];
        }

        // Configurations
        [Header("Config - Source Elements")]
        [SerializeField] protected bool generateTileMesh = true;
        [SerializeField] protected Material useThisMaterial;
        
        // TODO: option to render a mesh instead?

        [Header("Config - Gizmos")]
        [SerializeField] protected Color itemGizmoColour;
        [SerializeField] protected float itemGizmoRadius = 0.05f;


        // EVENTS
        public event GridItemPureClicked OnItemClicked;




        // METHODS

        // SET UP
        public override void Init(Grid_Base parentGrid, int gridResolutionX, int gridResolutionZ, float tileSize, float itemSize, Vector3 itemOffset)
        {
            base.Init(parentGrid, gridResolutionX, gridResolutionZ, tileSize, itemSize, itemOffset);

            // Create items.
            GenerateItems();

            // Create tile mesh.
            if (generateTileMesh)
            {
                // TODO: add them instead.

                myMeshRenderer = this.GetComponent<MeshRenderer>();
                myMeshRenderer.material = useThisMaterial;

                myMeshFilter = this.GetComponent<MeshFilter>();
                myMesh = myMeshFilter.mesh;

                GenerateTileMesh();
            }

            // Populate boundsGizmoPoints.
            boundsGizmoVertexPos = new Vector3[4];
            //PopulateBoundsGizmoPoints();
        }

        // Separated submethods for ease of overriding.
        protected override void GenerateItems()
        {
            if (myGridItems.IsCreated)
            {
                myGridItems.Dispose();
            }

            int2 resolution = new int2(myResolutionX, myResolutionZ);
            float4x4 baseTransformMatrix = CalculateMyTransformMatrix();

            myGridItems = new NativeArray<GridItem>(myResolutionX * myResolutionZ, Allocator.Persistent);

            // Do the job proper.
            GridItemGenerationJob.Schedule(myGridItems, myItemPositioning, resolution, myTileSize, myItemSize, (float3)myItemOffset, baseTransformMatrix, default).Complete();
        }

        protected void GenerateTileMesh()
        {
            Mesh.MeshDataArray newMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData newMeshData = newMeshDataArray[0];

            GridMeshGenerator_SeparateQuadsFromItems<MeshStreamUInt32>.Schedule(myMesh, newMeshData, myGridItems, myTileSize, myItemSize, default).Complete();

            Mesh.ApplyAndDisposeWritableMeshData(newMeshDataArray, myMesh);
        }

        protected override void PopulateBoundsGizmoPoints()
        {
            boundsGizmoVertexPos[0] = MyGridItem(0, 0).WorldPosition;
            boundsGizmoVertexPos[1] = MyGridItem(myResolutionX - 1, 0).WorldPosition;
            boundsGizmoVertexPos[2] = MyGridItem(myResolutionX - 1, myResolutionZ - 1).WorldPosition;
            boundsGizmoVertexPos[3] = MyGridItem(0, myResolutionZ - 1).WorldPosition;
        }


        // UTILITY
        protected float4x4 CalculateMyTransformMatrix()
        {
            // Make transform matrices from this manager and the parent grid.
            Matrix4x4 managerTransformMatrix = Matrix4x4.TRS(gameObject.transform.localPosition, gameObject.transform.localRotation, gameObject.transform.localScale);
            Matrix4x4 gridTransformMatrix = Matrix4x4.TRS(myParentGrid.transform.position, myParentGrid.transform.rotation, myParentGrid.transform.lossyScale);
            return (float4x4)(gridTransformMatrix * managerTransformMatrix);
        }

        protected void UpdateItemsTransformMatrices()
        {
            if (myGridItems.IsCreated)
            {
                float4x4 baseTransformMatrix = CalculateMyTransformMatrix();
                GridItemWorldMatrixUpdateJob.Schedule(myGridItems, baseTransformMatrix, default).Complete();
            }
        }


        // ITEM SEARCH
        protected void SearchForClickedItem(Vector3 mousePosition)
        {
            RaycastHit cursorHit;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            // Raycast so we can work with world positions.
            if (Physics.Raycast(ray, out cursorHit))
            {
                Debug.Log(string.Format("The click's world position is {0}.", cursorHit.point));

                NativeArray<int> outputInfo = new NativeArray<int>(3, Allocator.TempJob);
                GridItemSearchJob.Schedule(cursorHit.point, myGridItems, new int2(myGridsResolutionX, myGridsResolutionZ), myTileSize, outputInfo, default).Complete();
                Debug.Log(string.Format("Final closest point: #{0} {1} at {5}. Looked at a total of {2} items out of {3} in {4} step(s).", outputInfo[0], myGridItems[outputInfo[0]].GridCoordinates, outputInfo[1], myResolutionX * myResolutionZ, outputInfo[2], myGridItems[outputInfo[0]].WorldPosition));

                // Send the event and the like

                outputInfo.Dispose();
            }
        }


        // BUILT IN
        void Update()
        {
            if (transform.hasChanged)
            {
                UpdateItemsTransformMatrices();
                transform.hasChanged = false;
            }
        }

        void OnDestroy()
        {
            if (myGridItems.IsCreated)
            {
                myGridItems.Dispose();
            }
        }

        protected override void OnDrawGizmos()
        {
            //base.OnDrawGizmos();

            if (myMesh != null)
            {
                Gizmos.color = boundsGizmoColour;
                Gizmos.DrawWireCube(myMesh.bounds.center, myMesh.bounds.size);
            }

            // Draw the component items.
            if (myGridItems.IsCreated)
            {
                Gizmos.color = itemGizmoColour;
                foreach (GridItem item in myGridItems)
                {
                    Gizmos.DrawSphere(item.WorldPosition, itemGizmoRadius * myItemSize);
                }
            }
        }


        protected void OnMouseDown()
        {
            Debug.Log(string.Format("Mouse down on {0} at {1}.", gameObject, Input.mousePosition));

            SearchForClickedItem(Input.mousePosition);
        }
    }
}

