using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using MeshGeneration;

namespace Building
{
    public partial class WallSegment : MonoBehaviour
    {
        // STATIC METHODS

        private static Mesh GenerateThickWallMesh(float3 anchorAGroundPosMin, float3 anchorBGroundPosMin, float3 anchorATopPosMin, float3 anchorBTopPosMin, float3 anchorAGroundPosPlus, float3 anchorBGroundPosPlus, float3 anchorATopPosPlus, float3 anchorBTopPosPlus)
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
            Mesh wallMesh = new Mesh { name = "WallMesh" };
            wallMesh.Clear();

            Mesh.MeshDataArray wallMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData wallMeshData = wallMeshDataArray[0];

            var stream = new MultimeshStreamUInt16();
            stream.Setup(wallMeshData, new Bounds(), submeshCount, vertexCounts, triangleCounts);

            // Create the vertices
            // Main clockwise
            Vertex[] faceVertices = CreateFlatFaceVertices(anchorAGroundPosMin, anchorBGroundPosMin, anchorATopPosMin, anchorBTopPosMin);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i, faceVertices[i]); }
            // Main counterclockwise
            faceVertices = CreateFlatFaceVertices(anchorBGroundPosPlus, anchorAGroundPosPlus, anchorBTopPosPlus, anchorATopPosPlus);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(1, i, faceVertices[i]); }
            // Top
            faceVertices = CreateFlatFaceVertices(anchorATopPosMin, anchorBTopPosMin, anchorATopPosPlus, anchorBTopPosPlus);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(2, i, faceVertices[i]); }
            // Bottom
            faceVertices = CreateFlatFaceVertices(anchorBGroundPosMin, anchorAGroundPosMin, anchorBGroundPosPlus, anchorAGroundPosPlus);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(2, i + 4, faceVertices[i]); }
            // AnchorA
            faceVertices = CreateFlatFaceVertices(anchorAGroundPosPlus, anchorAGroundPosMin, anchorATopPosPlus, anchorATopPosMin);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(2, i + 8, faceVertices[i]); }
            // AnchorB
            faceVertices = CreateFlatFaceVertices(anchorBGroundPosMin, anchorBGroundPosPlus, anchorBTopPosMin, anchorBTopPosPlus);
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
            Mesh.ApplyAndDisposeWritableMeshData(wallMeshDataArray, wallMesh);

            return wallMesh;
        }

        private static Mesh GenerateFlatWallMesh(float3 anchorAPos, float3 anchorATopPos, float3 anchorBPos, float3 anchorBTopPos)
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
            Mesh wallMesh = new Mesh { name = "WallMesh" };
            wallMesh.Clear();

            Mesh.MeshDataArray wallMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData wallMeshData = wallMeshDataArray[0];

            var stream = new MultimeshStreamUInt16();
            stream.Setup(wallMeshData, new Bounds(), submeshCount, vertexCounts, triangleCounts);

            // Create the vertices
            // Main clockwise
            Vertex[] faceVertices = CreateFlatFaceVertices(anchorAPos, anchorBPos, anchorATopPos, anchorBTopPos);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i, faceVertices[i]); }
            // Main counterclockwise
            faceVertices = CreateFlatFaceVertices(anchorBPos, anchorAPos, anchorBTopPos, anchorATopPos);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(1, i, faceVertices[i]); }

            // Create the triangles
            for (int i = 0; i < submeshCount; i++)
            {
                stream.SetTriangle(i, 0, new int3(0, 3, 1));
                stream.SetTriangle(i, 1, new int3(0, 2, 3));
            }

            // Apply and adjust.
            Mesh.ApplyAndDisposeWritableMeshData(wallMeshDataArray, wallMesh);

            return wallMesh;
        }

        private static Mesh GenerateWallColliderMesh(float3 anchorAPos, float3 anchorATopPos, float3 anchorBPos, float3 anchorBTopPos)
        {
            // Counts
            int vertexCount = 8;
            int triangleCount = 4;
            int indexCount = 4 * 3;

            // Set up the mesh and stream.
            Mesh colliderMesh = new Mesh { name = "WallColliderMesh" };
            colliderMesh.Clear();

            Mesh.MeshDataArray colliderMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData colliderMeshData = colliderMeshDataArray[0];

            var stream = new MeshStreamUInt16();
            stream.Setup(colliderMeshData, new Bounds(), vertexCount, indexCount);

            // Create the vertices
            // Main clockwise
            Vertex[] faceVertices = CreateFlatFaceVertices(anchorAPos, anchorBPos, anchorATopPos, anchorBTopPos);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(i, faceVertices[i]); }
            // Main counterclockwise
            faceVertices = CreateFlatFaceVertices(anchorBPos, anchorAPos, anchorBTopPos, anchorATopPos);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(i + 4, faceVertices[i]); }

            // Create the triangles
            for (int t = 0, v = 0; t < triangleCount; t += 2, v += 4)
            {
                stream.SetTriangle(t, new int3(v, v + 3, v + 1));
                stream.SetTriangle(t + 1, new int3(v, v + 2, v + 3));
            }

            // Apply and adjust.
            Mesh.ApplyAndDisposeWritableMeshData(colliderMeshDataArray, colliderMesh);

            return colliderMesh;
        }

        private static Bounds GenerateWallMeshBounds(Vector3 anchorAPos, Vector3 anchorATopPos, Vector3 anchorBPos, Vector3 anchorBTopPos)
        {
            // Calculate the bounds' necessary data.
            Vector3 anchorAMidPos = (anchorAPos + anchorATopPos) / 2f;
            Vector3 anchorBMidPos = (anchorBPos + anchorBTopPos) / 2f;

            Vector3 boundsCenter = Vector3.Lerp(anchorAMidPos, anchorBMidPos, 0.5f);
            float boundsSizeX = math.abs(anchorAPos.x - anchorBPos.x);
            float boundsSizeY = math.abs(math.max(anchorATopPos.y, anchorBTopPos.y) - math.min(anchorAPos.y, anchorBPos.y));
            float boundsSizeZ = math.abs(anchorAPos.z - anchorBPos.z);
            Vector3 boundsSize = new Vector3(boundsSizeX, boundsSizeY, boundsSizeZ);

            return new Bounds(boundsCenter, boundsSize);
        }


        // TODO: decide whether to move this for reuse elsewhere.
        private static Vertex[] CreateFlatFaceVertices(float3 posZeroZero, float3 posOneZero, float3 posZeroOne, float3 posOneOne)
        {
            // Calculate normal and tangent.
            (float3, half4) normalAndTangent = MathHelpers.CalculateTrisNormalAndTangent(posZeroZero, posOneZero, posOneOne);
            //Vector3 faceTangent =  

            // Calculate UV composants.
            half uvDistanceHorizontal = (half)math.distance(posZeroZero, posOneZero);
            half uvDistanceVerticalZero = (half)math.distance(posZeroZero, posZeroOne);
            half uvDistanceVerticalOne = (half)math.distance(posOneZero, posOneOne);

            // Create the vertices.
            Vertex[] vertices = new Vertex[4];

            vertices[0] = new Vertex
            {
                position = posZeroZero,
                normal = normalAndTangent.Item1,
                tangent = normalAndTangent.Item2,
                texCoord0 = new half2(half.zero, half.zero)
            };
            vertices[1] = new Vertex
            {
                position = posOneZero,
                normal = normalAndTangent.Item1,
                tangent = normalAndTangent.Item2,
                texCoord0 = new half2(uvDistanceHorizontal, half.zero)
            };
            vertices[2] = new Vertex
            {
                position = posZeroOne,
                normal = normalAndTangent.Item1,
                tangent = normalAndTangent.Item2,
                texCoord0 = new half2(half.zero, uvDistanceVerticalZero)
            };
            vertices[3] = new Vertex
            {
                position = posOneOne,
                normal = normalAndTangent.Item1,
                tangent = normalAndTangent.Item2,
                texCoord0 = new half2(uvDistanceHorizontal, uvDistanceVerticalOne)
            };

            return vertices;
        }
    }
}
