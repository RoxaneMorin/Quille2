using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TestGrid : MonoBehaviour
{
    // STRUCTS
    [StructLayout(LayoutKind.Sequential)]
    struct Vertex
    {
        public float3 position;
        public float3 normal;
        public half4 tangent;
        public half2 texCoord0;
    }



    // VARIABLES

    // Grid parameters.
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private int gridLengthX = 10;
    [SerializeField] private int gridLengthZ = 10;

    public Material gridMaterial;

    // Mesh stuff
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    // Grid points.
    private Dictionary<(int, int), int> gridPointIndices;

    // 


    



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
        half h0 = half(0f), h1 = half(1f);

        for (int i = 0, z = 0; z <= gridLengthZ; z++)
        {
            for (int x = 0; x <= gridLengthX; x++, i++)
            {
                float y = (sin(x) + cos(z))/4;

                var vertex = new Vertex
                {
                    position = float3(x * tileSize, y, z * tileSize),
                    normal = float3(0, 1, 0),
                    tangent = half4(h1, h0, h0, half(-1f)),
                    texCoord0 = half2(half(x), half(z))
                };
                vertices[i] = vertex;
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
        meshData.SetSubMesh(0, new SubMeshDescriptor(0, triangleCount) { bounds = bounds, vertexCount = vertexCount}, MeshUpdateFlags.DontRecalculateBounds);

        var mesh = new Mesh { bounds = bounds, name = "Grid" };
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
        meshFilter.mesh = mesh;
    }


    // BUILT IN
    private void Awake()
    {
        Init();
        Generate();



        //vertices[gridPointIndices[(2, 1)]] = vertices[gridPointIndices[(2, 1)]] + new Vector3(0f, 2f, 0f);

        //GetComponent<MeshFilter>().mesh.SetVertices(vertices);
    }

}
