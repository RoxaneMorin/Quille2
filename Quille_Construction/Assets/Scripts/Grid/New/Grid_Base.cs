using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Grid_Base : MonoBehaviour
    {
        // STATIC
        static GridMeshJobScheduleDelegate[] gridMeshJobs =
        {
            GridGenerationJob<GridMeshGenerator_SeparateQuads, SingleStream>.ScheduleParallel,
            GridGenerationJob<GridMeshGenerator_LinkedQuads, SingleStream>.ScheduleParallel
        };



        // VARIABLES
        // Components
        [Header("Components")]
        protected MeshFilter myMeshFilter;
        protected MeshRenderer myMeshRenderer;
        protected Mesh myMesh;

        // TODO: add collider so we can handle mouse hovering?

        // Grid parameters.
        [Header("Grid Parameters - Type & Dimensions")]
        [SerializeField] GridMeshType gridMeshType = GridMeshType.LinkedQuads;
        [SerializeField, Range(1, 100)] protected int resolutionX = 10;
        [SerializeField, Range(1, 100)] protected int resolutionZ = 10;
        [SerializeField, Range(0.1f, 2f)] protected float tileSize = 1f;

        [Header("Grid Parameters - Components")]
        [SerializeField] protected Material gridMaterial;
        [SerializeField] protected GameObject gridPointManagerPrefab;
        [SerializeField] protected GameObject gridTileManagerPrefab;

        // Grid data.
        [SerializeField] protected int[,] gridVertexIndexByCoords;
        [Header("Grid Data")] // Wouldn't appear if placed above gridVertexIndexByCoords :/
        [SerializeField] protected Grid_ItemManager myGridPointManager;
        [SerializeField] protected Grid_ItemManager myGridTileManager;

        // TODO: have the handles live here instead?

        protected bool cursorBusy;
        public bool CursorBusy { get { return cursorBusy; } set { cursorBusy = value; } }



        // METHODS

        // SET UP
        protected void Init()
        {
            myMeshRenderer = this.GetComponent<MeshRenderer>();
            myMeshRenderer.material = gridMaterial;

            myMeshFilter = this.GetComponent<MeshFilter>();
            myMesh = myMeshFilter.mesh;

            GenerateMesh();
            Populate();
        }
        protected void GenerateMesh()
        {
            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData meshData = meshDataArray[0];

            gridMeshJobs[(int)gridMeshType](myMesh, meshData, new int2(resolutionX, resolutionZ), tileSize, default).Complete();

            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, myMesh);
        }

        protected void Populate()
        {
            // TODO: place these manually instead of instantiating them from prefabs?

            // Create grid points.
            if (gridPointManagerPrefab)
            {
                myGridPointManager = Instantiate(gridPointManagerPrefab, transform).GetComponent<Grid_ItemManager>();
                myGridPointManager.Init(this, resolutionX, resolutionZ, tileSize, tileSize, Vector3.zero);
                myGridPointManager.name = "Grid_PointManager";
            }

            // Create grid tiles.
            if (gridTileManagerPrefab)
            {
                myGridTileManager = Instantiate(gridTileManagerPrefab, transform).GetComponent<Grid_ItemManager>();
                myGridTileManager.Init(this, resolutionX, resolutionZ, tileSize, tileSize * 0.95f, new Vector3(0f, 0.01f, 0f));
                myGridTileManager.name = "Grid_TileManager";
            }

            // TODO: subscribe to events.
        }


        // MANIPULATIONS
        //public void MoveVertex(CoordPair vertexCoords, Vector3 positionDelta)
        //{
        //    // TODO: can this be done via jobs?
        //    var previousVertices = myMeshFilter.mesh.vertices;
        //    previousVertices[gridVertexIndexByCoords[vertexCoords.x, vertexCoords.z]] += positionDelta;
        //    myMeshFilter.mesh.vertices = previousVertices;
        //}
        //public void MoveQuad(CoordPair[,] vertexCoords, Vector3 positionDelta)
        //{
        //    // TODO: can this be done via jobs?
        //    var previousVertices = myMeshFilter.mesh.vertices;
        //    foreach (CoordPair coordPair in vertexCoords)
        //    {
        //        previousVertices[gridVertexIndexByCoords[coordPair.x, coordPair.z]] += positionDelta;
        //    }
        //    myMeshFilter.mesh.vertices = previousVertices;
        //}

        //protected void EditVertices()
        //{
        //    Mesh.MeshDataArray theMeshDataArray = Mesh.AcquireReadOnlyMeshData(myMeshFilter.mesh);
        //    Mesh.MeshData theMeshData = theMeshDataArray[0];

        //    NativeArray<Vector3> theVertices = new NativeArray<Vector3>(myMeshFilter.mesh.vertexCount, Allocator.TempJob);
        //    theMeshData.GetVertices(theVertices);

        //    // Do the manipulation.
        //    //for (int i = 0; i < theVertices.Length; i++)
        //    //{
        //    //    Debug.Log(theVertices[i]);
        //    //}

        //    myMeshFilter.mesh.SetVertices(theVertices);
        //    myMeshFilter.mesh.RecalculateNormals();
        //    myMeshFilter.mesh.RecalculateBounds();

        //    theVertices.Dispose();
        //}


        // BUILT IN
        void Awake()
        {
            Init();
        }
    }

}