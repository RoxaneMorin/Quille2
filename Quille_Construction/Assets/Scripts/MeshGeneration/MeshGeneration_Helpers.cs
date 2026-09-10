using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;


namespace MeshGeneration
{
    public static class MeshGenerationHelpers
    {
        // Create the necessary vertices for the creation of a quad with flat/averaged normals and zeroed-out tangents.
        public static Vertex[] MakeVerticesForQuad(float3 posZeroZero, float3 posOneZero, float3 posZeroOne, float3 posOneOne)
        {
            // Calculate face normal.
            float3 normal = MathHelpers.CalculateFaceNormal(posZeroZero, posOneZero, posZeroOne, posOneOne);
            half4 tangent = new half4(0); // junk tangent

            // Calculate UV composants.
            half uvDistanceHorizontal = (half)math.distance(posZeroZero, posOneZero);
            half uvDistanceVerticalZero = (half)math.distance(posZeroZero, posZeroOne);
            half uvDistanceVerticalOne = (half)math.distance(posOneZero, posOneOne);

            // Create the vertices.
            Vertex[] vertices = new Vertex[4];

            vertices[0] = new Vertex
            {
                position = posZeroZero,
                normal = normal,
                tangent = tangent,
                texCoord0 = new half2(half.zero, half.zero)
            };
            vertices[1] = new Vertex
            {
                position = posOneZero,
                normal = normal,
                tangent = tangent,
                texCoord0 = new half2(uvDistanceHorizontal, half.zero)
            };
            vertices[2] = new Vertex
            {
                position = posZeroOne,
                normal = normal,
                tangent = tangent,
                texCoord0 = new half2(half.zero, uvDistanceVerticalZero)
            };
            vertices[3] = new Vertex
            {
                position = posOneOne,
                normal = normal,
                tangent = tangent,
                texCoord0 = new half2(uvDistanceHorizontal, uvDistanceVerticalOne)
            };

            return vertices;
        }


        // Generate a two-sided plane mesh using the given vertex coordinates.
        public static Mesh GenerateTwoSidedPlane(float3 posZeroZero, float3 posOneZero, float3 posZeroOne, float3 posOneOne)
        {
            // Counts
            int vertexCount = 8;
            int triangleCount = 4;
            int indexCount = 4 * 3;

            // Set up the mesh and stream.
            Mesh generatedMesh = new Mesh();
            generatedMesh.Clear();

            Mesh.MeshDataArray generatedMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData generatedMeshData = generatedMeshDataArray[0];

            var stream = new MeshStreamUInt16();
            stream.Setup(generatedMeshData, new Bounds(), vertexCount, indexCount);

            // Create the vertices
            // Main clockwise
            Vertex[] faceVertices = MakeVerticesForQuad(posZeroZero, posOneZero, posZeroOne, posOneOne);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(i, faceVertices[i]); }
            // Main counterclockwise
            faceVertices = MakeVerticesForQuad(posOneZero, posZeroZero, posOneOne, posZeroOne);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(i + 4, faceVertices[i]); }

            // Create the triangles
            for (int t = 0, v = 0; t < triangleCount; t += 2, v += 4)
            {
                stream.SetTriangle(t, new int3(v, v + 3, v + 1));
                stream.SetTriangle(t + 1, new int3(v, v + 2, v + 3));
            }

            // Apply and adjust.
            Mesh.ApplyAndDisposeWritableMeshData(generatedMeshDataArray, generatedMesh);

            return generatedMesh;
        }

        // Generate a two-sided plane mesh using the given vertex coordinates, each quad being its own submesh.
        public static Mesh GenerateTwoSidedPlaneTwoMats(float3 posZeroZero, float3 posOneZero, float3 posZeroOne, float3 posOneOne)
        {
            // Counts
            int submeshCount = 2;

            NativeArray<int> vertexCounts = new NativeArray<int>(2, Allocator.Temp);
            vertexCounts[0] = 4;
            vertexCounts[1] = 4;

            NativeArray<int> triangleCounts = new NativeArray<int>(2, Allocator.Temp);
            triangleCounts[0] = 2;
            triangleCounts[1] = 2;

            // Set up the mesh and stream.
            Mesh generatedMesh = new Mesh();
            generatedMesh.Clear();

            Mesh.MeshDataArray generatedMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData generatedMeshData = generatedMeshDataArray[0];

            var stream = new MultimeshStreamUInt16();
            stream.Setup(generatedMeshData, new Bounds(), submeshCount, vertexCounts, triangleCounts);

            // Create the vertices
            // Main clockwise
            Vertex[] faceVertices = MakeVerticesForQuad(posZeroZero, posOneZero, posZeroOne, posOneOne);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i, faceVertices[i]); }
            // Main counterclockwise
            faceVertices = MakeVerticesForQuad(posOneZero, posZeroZero, posOneOne, posZeroOne);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(1, i, faceVertices[i]); }

            // Create the triangles
            for (int i = 0; i < submeshCount; i++)
            {
                stream.SetTriangle(i, 0, new int3(0, 3, 1));
                stream.SetTriangle(i, 1, new int3(0, 2, 3));
            }

            // Apply and adjust.
            Mesh.ApplyAndDisposeWritableMeshData(generatedMeshDataArray, generatedMesh);

            return generatedMesh;
        }


        public static Mesh GenerateBoxThreeMats(float3 posZeroZeroZero, float3 posOneZeroZero, float3 posZeroOneZero, float3 posOneOneZero,
                                       float3 posZeroZeroOne, float3 posOneZeroOne, float3 posZeroOneOne, float3 posOneOneOne)
        {
            // Counts
            int submeshCount = 3;

            NativeArray<int> vertexCounts = new NativeArray<int>(3, Allocator.Temp);
            vertexCounts[0] = 4;
            vertexCounts[1] = 4;
            vertexCounts[2] = 16;

            NativeArray<int> triangleCounts = new NativeArray<int>(3, Allocator.Temp);
            triangleCounts[0] = 2;
            triangleCounts[1] = 2;
            triangleCounts[2] = 8;

            // Set up the mesh and stream.
            Mesh generatedMesh = new Mesh();
            generatedMesh.Clear();

            Mesh.MeshDataArray generatedMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData generatedMeshData = generatedMeshDataArray[0];

            var stream = new MultimeshStreamUInt16();
            stream.Setup(generatedMeshData, new Bounds(), submeshCount, vertexCounts, triangleCounts);

            // Create the vertices
            // Main clockwise
            Vertex[] faceVertices = MakeVerticesForQuad(posZeroZeroZero, posOneZeroZero, posZeroOneZero, posOneOneZero);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i, faceVertices[i]); }
            // Main counterclockwise
            faceVertices = MakeVerticesForQuad(posOneZeroOne, posZeroZeroOne, posOneOneOne, posZeroOneOne);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(1, i, faceVertices[i]); }
            // Top
            faceVertices = MakeVerticesForQuad(posZeroOneZero, posOneOneZero, posZeroOneOne, posOneOneOne);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(2, i, faceVertices[i]); }
            // Bottom
            faceVertices = MakeVerticesForQuad(posOneZeroZero, posZeroZeroZero, posOneZeroOne, posZeroZeroOne);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(2, i + 4, faceVertices[i]); }
            // AnchorA
            faceVertices = MakeVerticesForQuad(posZeroZeroOne, posZeroZeroZero, posZeroOneOne, posZeroOneZero);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(2, i + 8, faceVertices[i]); }
            // AnchorB
            faceVertices = MakeVerticesForQuad(posOneZeroZero, posOneZeroOne, posOneOneZero, posOneOneOne);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(2, i + 12, faceVertices[i]); }

            // Create the triangles
            for (int i = 0; i < 2; i++)
            {
                stream.SetTriangle(i, 0, new int3(0, 3, 1));
                stream.SetTriangle(i, 1, new int3(0, 2, 3));
            }
            for (int t = 0, v = 0; t < triangleCounts[2]; t += 2, v += 4)
            {
                stream.SetTriangle(2, t, new int3(v, v + 3, v + 1));
                stream.SetTriangle(2, t + 1, new int3(v, v + 2, v + 3));
            }

            // Apply and adjust.
            Mesh.ApplyAndDisposeWritableMeshData(generatedMeshDataArray, generatedMesh);

            return generatedMesh;
        }
    }
}