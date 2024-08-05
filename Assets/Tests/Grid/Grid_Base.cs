using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;
using UnityEngine.Rendering;

namespace proceduralGrid
{
    [System.Serializable]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Grid_Base : MonoBehaviour
    {
        // VARIABLES

        // Grid parameters.
        [Header("Grid Parameters")]
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private int gridLengthX = 10;
        [SerializeField] private int gridLengthZ = 10;

        [Header("Grid Components")]
        [SerializeField] private Material gridMaterial;

        // Mesh stuff
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        // Control stuff
        [SerializeField] private GameObject gridPointPrefab;
        [SerializeField] private GameObject gridSquarePrefab;

        // Grid point information
        private int[,] gridVertexIndexByCoords;
        private Grid_Point[,] gridPointsByCoords;
        private Grid_Square[,] gridSquaresByCoords;

        private bool cursorBusy;
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
        private void Init()
        {
            meshFilter = this.GetComponent<MeshFilter>();
            meshRenderer = this.GetComponent<MeshRenderer>();

            meshRenderer.material = gridMaterial;
        }
        private void Generate()
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
            meshFilter.mesh = mesh;
        }
        private void Populate()
        {
            // Grid Points
            gridPointsByCoords = new Grid_Point[gridLengthX + 1, gridLengthZ + 1];

            for (int i = 0, z = 0; z <= gridLengthZ; z++)
            {
                for (int x = 0; x <= gridLengthX; x++, i++)
                {
                    GameObject gridPointGO = Instantiate(gridPointPrefab, new Vector3(x * tileSize, 0.1f, z * tileSize), new Quaternion(-1f, 0f, 0f, 1));
                    gridPointsByCoords[x, z] = gridPointGO.GetComponent<Grid_Point>();

                    gridPointGO.transform.SetParent(transform);
                    gridPointGO.transform.localScale = new Vector3(gridPointGO.transform.localScale.x * tileSize, gridPointGO.transform.localScale.y * tileSize, gridPointGO.transform.localScale.z * tileSize);

                    gridPointsByCoords[x, z].SetVariables(this, new CoordPair(x, z));
                    gridPointsByCoords[x, z].name = string.Format("Grid Point ({0}, {1})", x, z);
                }
            }

            // Grid Squares.
            gridSquaresByCoords = new Grid_Square[gridLengthX, gridLengthZ];
            float halfTileSize = tileSize / 2f;

            for (int z = 0; z < gridLengthZ; z++)
            {
                for (int x = 0; x < gridLengthX; x++)
                {
                    GameObject gridSquareGO = Instantiate(gridSquarePrefab, new Vector3(x * tileSize + halfTileSize, 0.1f, z * tileSize + halfTileSize), new Quaternion(-1f, 0f, 0f, 1));
                    gridSquaresByCoords[x, z] = gridSquareGO.GetComponent<Grid_Square>();

                    gridSquareGO.transform.SetParent(transform);
                    gridSquareGO.transform.localScale = new Vector3(gridSquareGO.transform.localScale.x * tileSize, gridSquareGO.transform.localScale.y * tileSize, gridSquareGO.transform.localScale.z * tileSize);

                    Grid_Point[,] squaresGridPoints = new Grid_Point[,] { { gridPointsByCoords[x, z], gridPointsByCoords[x + 1, z] }, { gridPointsByCoords[x, z + 1], gridPointsByCoords[x + 1, z + 1] } };
                    gridSquaresByCoords[x, z].SetVariables(this, new CoordPair(x, z), squaresGridPoints);
                    gridSquaresByCoords[x, z].name = string.Format("Grid Square ({0}, {1})", x, z);

                    // Set points' adjacent squares.
                    squaresGridPoints[0, 0].SetAdjacentGridSquare(Adjacencies.tr, gridSquaresByCoords[x, z]);
                    squaresGridPoints[1, 0].SetAdjacentGridSquare(Adjacencies.tl, gridSquaresByCoords[x, z]);
                    squaresGridPoints[0, 1].SetAdjacentGridSquare(Adjacencies.bl, gridSquaresByCoords[x, z]);
                    squaresGridPoints[1, 1].SetAdjacentGridSquare(Adjacencies.br, gridSquaresByCoords[x, z]);
                }
            }

            // Grid Squares Adjacencies.
            for (int z = 0; z < gridLengthZ; z++)
            {
                for (int x = 0; x < gridLengthX; x++)
                {
                    // Bottom row.
                    if (x > 0 && z > 0)
                        gridSquaresByCoords[x, z].SetAdjacentGridSquare(Adjacencies.bl, gridSquaresByCoords[x - 1, z - 1]);
                    if (z > 0)
                        gridSquaresByCoords[x, z].SetAdjacentGridSquare(Adjacencies.b, gridSquaresByCoords[x, z - 1]);
                    if (x < gridLengthX - 1 && z > 0)
                        gridSquaresByCoords[x, z].SetAdjacentGridSquare(Adjacencies.br, gridSquaresByCoords[x + 1, z - 1]);

                    // Med row.
                    if (x > 0)
                        gridSquaresByCoords[x, z].SetAdjacentGridSquare(Adjacencies.l, gridSquaresByCoords[x - 1, z]);
                    if (x < gridLengthX - 1)
                        gridSquaresByCoords[x, z].SetAdjacentGridSquare(Adjacencies.r, gridSquaresByCoords[x + 1, z]);

                    // Top row.
                    if (x > 0 && z < gridLengthZ - 1)
                        gridSquaresByCoords[x, z].SetAdjacentGridSquare(Adjacencies.tl, gridSquaresByCoords[x - 1, z + 1]);
                    if (z < gridLengthZ - 1)
                        gridSquaresByCoords[x, z].SetAdjacentGridSquare(Adjacencies.t, gridSquaresByCoords[x, z + 1]);
                    if (x < gridLengthX - 1 && z < gridLengthZ - 1)
                        gridSquaresByCoords[x, z].SetAdjacentGridSquare(Adjacencies.tr, gridSquaresByCoords[x + 1, z + 1]);
                }
            }
        }


        // MANIPULATIONS
        public void MoveVertex(CoordPair vertexCoords, Vector3 positionDelta)
        {
            var previousVertices = meshFilter.mesh.vertices;
            previousVertices[gridVertexIndexByCoords[vertexCoords.x, vertexCoords.z]] += positionDelta;
            meshFilter.mesh.vertices = previousVertices;
        }
        public void MoveSquare(CoordPair[,] vertexCoords, Vector3 positionDelta)
        {
            var previousVertices = meshFilter.mesh.vertices;
            foreach (CoordPair coordPair in vertexCoords)
            {
                previousVertices[gridVertexIndexByCoords[coordPair.x, coordPair.z]] += positionDelta;
            }
            meshFilter.mesh.vertices = previousVertices;
        }


        // BUILT IN
        private void Awake()
        {
            Init();
            Generate();
            Populate();
        }

        private void Update()
        {
        }

    }

}