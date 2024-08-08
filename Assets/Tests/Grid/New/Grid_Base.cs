using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.Mathematics.math;

namespace proceduralGrid
{
    [System.Serializable]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Grid_Base : MonoBehaviour
    {
        // VARIABLES
        // Components
        [Header("Components")]
        protected MeshFilter myMeshFilter;
        protected MeshRenderer myMeshRenderer;

        // Grid parameters.
        [Header("Grid Parameters - Dimensions")]
        [SerializeField] protected float tileSize = 1f;
        [SerializeField] protected int gridLengthX = 10;
        [SerializeField] protected int gridLengthZ = 10;

        [Header("Grid Parameters - Components")]
        [SerializeField] protected Material gridMaterial;
        [SerializeField] protected GameObject gridPointManagerPrefab;
        [SerializeField] protected GameObject gridTileManagerPrefab;

        // Grid data.
        [SerializeField] protected int[,] gridVertexIndexByCoords;
        [Header("Grid Data")] // Wouldn't appear if placed above gridVertexIndexByCoords :/
        [SerializeField] protected Grid_ItemManager myGridPointManager;
        [SerializeField] protected Grid_ItemManager myGridTileManager;
        


        protected bool cursorBusy;
        public bool CursorBusy { get { return cursorBusy; } set { cursorBusy = value; } }



        // STRUCTS

        [StructLayout(LayoutKind.Sequential)]
        struct Vertex
        {
            // VARIABLES
            public float3 position;
            public float3 normal;
            public half4 tangent;
            public half2 texCoord0;
        }



        // METHODS

        // SET UP
        protected void Init()
        {
            myMeshFilter = this.GetComponent<MeshFilter>();
            myMeshRenderer = this.GetComponent<MeshRenderer>();

            myMeshRenderer.material = gridMaterial;

            Generate();
            Populate();
        }
        protected void Generate()
        {
            // Handle mesh data
            int vertexAttributeCount = 4;
            int vertexCount = (gridLengthX + 1) * (gridLengthZ + 1);
            int triangleCount = gridLengthX * gridLengthZ * 6;

            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData meshData = meshDataArray[0];

            var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(vertexAttributeCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            vertexAttributes[0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3);
            vertexAttributes[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3);
            vertexAttributes[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float16, dimension: 4);
            vertexAttributes[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, dimension: 2);
            meshData.SetVertexBufferParams(vertexCount, vertexAttributes);
            vertexAttributes.Dispose();

            // Handle vertices and their info.
            NativeArray<Vertex> vertices = meshData.GetVertexData<Vertex>();
            gridVertexIndexByCoords = new int[gridLengthX + 1, gridLengthZ + 1];

            half h0 = half(0f), h1 = half(1f);

            for (int i = 0, z = 0; z <= gridLengthZ; z++)
            {
                for (int x = 0; x <= gridLengthX; x++, i++)
                {
                    var vertex = new Vertex
                    {
                        position = float3(x * tileSize, 0, z * tileSize),
                        normal = float3(0, 1, 0),
                        tangent = half4(h1, h0, h0, half(-1f)),
                        texCoord0 = half2(half(x), half(z))
                    };

                    vertices[i] = vertex;
                    gridVertexIndexByCoords[x, z] = i;
                }
            }

            // Handle triangles.
            meshData.SetIndexBufferParams(triangleCount, IndexFormat.UInt16);
            NativeArray<ushort> triangles = meshData.GetIndexData<ushort>();
            for (ushort ti = 0, vi = 0, z = 0; z < gridLengthZ; z++, vi++)
            {
                for (ushort x = 0; x < gridLengthX; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = triangles[ti + 4] = (ushort)(vi + gridLengthX + 1);
                    triangles[ti + 2] = triangles[ti + 3] = (ushort)(vi + 1);
                    triangles[ti + 5] = (ushort)(vi + gridLengthX + 2);
                }
            }

            // Calculate and set bounds.
            float sizeX = tileSize * gridLengthX;
            float sizeZ = tileSize * gridLengthZ;
            float centerX = sizeX / 2;
            float centerZ = sizeZ / 2;

            var bounds = new Bounds(new Vector3(centerX, 0, centerZ), new Vector3(sizeX, 0, sizeZ));

            // Set mesh and submesh.
            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, triangleCount) { bounds = bounds, vertexCount = vertexCount }, MeshUpdateFlags.DontRecalculateBounds);

            var mesh = new Mesh { bounds = bounds, name = "Grid" };
            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
            myMeshFilter.mesh = mesh;
        }

        protected void Populate()
        {
            // Create grid points.
            if (gridPointManagerPrefab)
            {
                myGridPointManager = Instantiate(gridPointManagerPrefab, transform).GetComponent<Grid_ItemManager>();
                myGridPointManager.Init(this, gridLengthX, gridLengthZ, tileSize, Vector3.zero);
                myGridPointManager.name = "GridPoints";
            }

            // Create grid tiles.
            if (gridTileManagerPrefab)
            {
                myGridTileManager = Instantiate(gridTileManagerPrefab, transform).GetComponent<Grid_ItemManager>();
                myGridTileManager.Init(this, gridLengthX - 1, gridLengthZ - 1, tileSize, new Vector3(0.5f, 0.01f, 0.5f));
                myGridTileManager.name = "GridTiles";
            }
        }


        // MANIPULATIONS
        public void MoveVertex(CoordPair vertexCoords, Vector3 positionDelta)
        {
            // TODO: can this be done via jobs?
            var previousVertices = myMeshFilter.mesh.vertices;
            previousVertices[gridVertexIndexByCoords[vertexCoords.x, vertexCoords.z]] += positionDelta;
            myMeshFilter.mesh.vertices = previousVertices;
        }
        public void MoveQuad(CoordPair[,] vertexCoords, Vector3 positionDelta)
        {
            // TODO: can this be done via jobs?
            var previousVertices = myMeshFilter.mesh.vertices;
            foreach (CoordPair coordPair in vertexCoords)
            {
                previousVertices[gridVertexIndexByCoords[coordPair.x, coordPair.z]] += positionDelta;
            }
            myMeshFilter.mesh.vertices = previousVertices;
        }


        // BUILT IN
        protected void Awake()
        {
            Init();
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log(string.Format("Cursor's ScreenPosition: {0}.", Input.mousePosition));
            }
        }

    }

}