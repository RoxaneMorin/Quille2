using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TestGrid : MonoBehaviour
{
    // VARIABLES

    // Grid parameters.
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private int gridLenghtX = 10;
    [SerializeField] private int gridLengthZ = 10;

    public Material gridMaterial;

    // Mesh stuff
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Vector3[] vertices;
    private int[] triangles;
    private Mesh mesh;

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
        //int vertexAttributeCount = 4;

        //Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        //Mesh.MeshData meshData = meshDataArray[0];

        //var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(vertexAttributeCount, Allocator.Temp);

        var mesh = new Mesh { name = "Grid" };
        //Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
        GetComponent<MeshFilter>().mesh = mesh;


        // Handle vertices and their info. Keep track of grid points.
        vertices = new Vector3[(gridLenghtX + 1) * (gridLengthZ + 1)];
        gridPointIndices = new Dictionary<(int, int), int>();

        Vector3[] normals = new Vector3[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector2[] uv = new Vector2[vertices.Length];  

        for (int i = 0, z = 0; z <= gridLengthZ; z++)
        {
            for (int x = 0; x <= gridLenghtX; x++, i++)
            {
                vertices[i] = new Vector3(x * tileSize, 0, z * tileSize);
                gridPointIndices.Add((x, z), i);

                normals[i] = Vector3.up;
                tangents[i] = new Vector4(1f, 0f, 0f, -1f);

                uv[i] = new Vector3(x, z);
            }
        }
        mesh.vertices = vertices;

        // Handle triangles.
        triangles = new int[gridLenghtX * gridLengthZ * 6];
        for (int ti = 0, vi = 0, z = 0; z < gridLengthZ; z++, vi++)
        {
            for (int x = 0; x < gridLenghtX; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + gridLenghtX + 1;
                triangles[ti + 5] = vi + gridLenghtX + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.tangents = tangents;
        mesh.uv = uv;

        mesh.RecalculateNormals();
    }


    // BUILT IN
    private void Awake()
    {
        Init();
        Generate();



        vertices[gridPointIndices[(2, 1)]] = vertices[gridPointIndices[(2, 1)]] + new Vector3(0f, 2f, 0f);

        GetComponent<MeshFilter>().mesh.SetVertices(vertices);
    }

}
