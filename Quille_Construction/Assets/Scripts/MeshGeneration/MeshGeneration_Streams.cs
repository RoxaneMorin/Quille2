using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshGeneration
{
    // TODO: see if we should switch from receiving index to triangle counts for the single-mesh streams too.


    // Mesh streams for a single submesh.

    // For a maximum of ~65535 indices.
    public struct MeshStreamUInt16 : IMeshStreams
    {
        // VARIABLES
        [NativeDisableContainerSafetyRestriction] NativeArray<Vertex> stream;
        [NativeDisableContainerSafetyRestriction] NativeArray<TriangleUInt16> triangleIndices;


        // METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex(int index, Vertex vertex) => stream[index] = vertex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTriangle(int index, int3 triangle)
        {
            triangleIndices[index] = triangle;
        }

        public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount)
        {
            var vertexAttributeDescriptor = GetVertexAttributeDescriptor();

            meshData.SetVertexBufferParams(vertexCount, vertexAttributeDescriptor);
            vertexAttributeDescriptor.Dispose();

            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt16);

            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount) { bounds = bounds, vertexCount = vertexCount }, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

            stream = meshData.GetVertexData<Vertex>();
            triangleIndices = meshData.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);
        }

        public static NativeArray<VertexAttributeDescriptor> GetVertexAttributeDescriptor()
        {
            var vertexAttributeDescriptor = new NativeArray<VertexAttributeDescriptor>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            vertexAttributeDescriptor[0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3);
            vertexAttributeDescriptor[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3);
            vertexAttributeDescriptor[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, dimension: 4, format: VertexAttributeFormat.Float16);
            vertexAttributeDescriptor[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, format: VertexAttributeFormat.Float16);

            return vertexAttributeDescriptor;
        }
    }

    // For a maximum of ~4,294,967,295 indices.
    public struct MeshStreamUInt32 : IMeshStreams
    {
        // VARIABLES
        [NativeDisableContainerSafetyRestriction] NativeArray<Vertex> stream;
        [NativeDisableContainerSafetyRestriction] NativeArray<int3> triangleIndices;


        // METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex(int index, Vertex vertex) => stream[index] = vertex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTriangle(int index, int3 triangle)
        {
            triangleIndices[index] = triangle;
        }

        public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount)
        {
            var vertexAttributeDescriptor = GetVertexAttributeDescriptor();

            meshData.SetVertexBufferParams(vertexCount, vertexAttributeDescriptor);
            vertexAttributeDescriptor.Dispose();

            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt32);

            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount) { bounds = bounds, vertexCount = vertexCount }, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

            stream = meshData.GetVertexData<Vertex>();
            triangleIndices = meshData.GetIndexData<int>().Reinterpret<int3>(4);
        }

        public static NativeArray<VertexAttributeDescriptor> GetVertexAttributeDescriptor()
        {
            var vertexAttributeDescriptor = new NativeArray<VertexAttributeDescriptor>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            vertexAttributeDescriptor[0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3);
            vertexAttributeDescriptor[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3);
            vertexAttributeDescriptor[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, dimension: 4, format: VertexAttributeFormat.Float16);
            vertexAttributeDescriptor[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, format: VertexAttributeFormat.Float16);

            return vertexAttributeDescriptor;
        }
    }


    // Mesh streams for multuiple submeshes.
    public struct MultimeshStreamUInt16 : IMultiMeshStreams
    {
        // VARIABLES
        [NativeDisableContainerSafetyRestriction] NativeArray<Vertex> stream;
        [NativeDisableContainerSafetyRestriction] NativeArray<TriangleUInt16> triangleIndices;

        [NativeDisableContainerSafetyRestriction] NativeArray<int> firstVertexForSubmesh;
        [NativeDisableContainerSafetyRestriction] NativeArray<int> firstTriangleForSubmesh;
        [NativeDisableContainerSafetyRestriction] NativeArray<int> firstIndexForSubmesh;

        // TODO: see if we could use NativeSlices instead of doing this math.

        // METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex(int submesh, int index, Vertex vertex)
        {
            int true_index = firstVertexForSubmesh[submesh] + index;
            stream[true_index] = vertex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTriangle(int submesh, int index, int3 triangle)
        {
            int true_index = firstTriangleForSubmesh[submesh] + index;
            int3 true_triangle = triangle + new int3(firstVertexForSubmesh[submesh]);
            triangleIndices[true_index] = true_triangle;
        }

        public void Setup(Mesh.MeshData meshData, Bounds bounds, int submeshCount, NativeArray<int> vertexCounts, NativeArray<int> triangleCounts)
        {
            var vertexAttributeDescriptor = GetVertexAttributeDescriptor();

            NativeArray<int> indexCounts = new NativeArray<int>(submeshCount, Allocator.Temp);

            firstVertexForSubmesh = new NativeArray<int>(submeshCount, Allocator.Temp);
            firstTriangleForSubmesh = new NativeArray<int>(submeshCount, Allocator.Temp);
            firstIndexForSubmesh = new NativeArray<int>(submeshCount, Allocator.Temp);

            int totalVertexCount = 0;
            int totalTriangleCount = 0;
            int totalIndexCount = 0;

            for (int i = 0; i < submeshCount; i++)
            {
                indexCounts[i] = triangleCounts[i] * 3;

                firstVertexForSubmesh[i] = totalVertexCount;
                firstTriangleForSubmesh[i] = totalTriangleCount;
                firstIndexForSubmesh[i] = totalIndexCount;

                totalVertexCount += vertexCounts[i];
                totalTriangleCount += triangleCounts[i];
                totalIndexCount += indexCounts[i];
            }

            meshData.SetVertexBufferParams(totalVertexCount, vertexAttributeDescriptor);
            vertexAttributeDescriptor.Dispose();

            meshData.SetIndexBufferParams(totalIndexCount, IndexFormat.UInt16);

            meshData.subMeshCount = submeshCount;
            for (int i = 0; i < submeshCount; i++)
            {
                meshData.SetSubMesh(i, new SubMeshDescriptor(0, indexCounts[i]) 
                { 
                    bounds = bounds, 
                    firstVertex = firstVertexForSubmesh[i], 
                    vertexCount = vertexCounts[i],
                    indexStart = firstIndexForSubmesh[i],
                    indexCount = indexCounts[i],
                }, 
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
            }

            stream = meshData.GetVertexData<Vertex>();
            triangleIndices = meshData.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);
        }

        public static NativeArray<VertexAttributeDescriptor> GetVertexAttributeDescriptor()
        {
            var vertexAttributeDescriptor = new NativeArray<VertexAttributeDescriptor>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            vertexAttributeDescriptor[0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3);
            vertexAttributeDescriptor[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3);
            vertexAttributeDescriptor[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, dimension: 4, format: VertexAttributeFormat.Float16);
            vertexAttributeDescriptor[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, format: VertexAttributeFormat.Float16);

            return vertexAttributeDescriptor;
        }
    }

    public struct MultimeshStreamUInt32 : IMultiMeshStreams
    {
        // VARIABLES
        [NativeDisableContainerSafetyRestriction] NativeArray<Vertex> stream;
        [NativeDisableContainerSafetyRestriction] NativeArray<int3> triangleIndices;

        [NativeDisableContainerSafetyRestriction] NativeArray<int> firstVertexForSubmesh;
        [NativeDisableContainerSafetyRestriction] NativeArray<int> firstTriangleForSubmesh;
        [NativeDisableContainerSafetyRestriction] NativeArray<int> firstIndexForSubmesh;

        // TODO: see if we could use NativeSlices instead of doing this math.

        // METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex(int submesh, int index, Vertex vertex)
        {
            int true_index = firstVertexForSubmesh[submesh] + index;
            stream[true_index] = vertex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTriangle(int submesh, int index, int3 triangle)
        {
            int true_index = firstTriangleForSubmesh[submesh] + index;
            int3 true_triangle = triangle + new int3(firstVertexForSubmesh[submesh]);
            triangleIndices[true_index] = true_triangle;
        }

        public void Setup(Mesh.MeshData meshData, Bounds bounds, int submeshCount, NativeArray<int> vertexCounts, NativeArray<int> triangleCounts)
        {
            var vertexAttributeDescriptor = GetVertexAttributeDescriptor();

            NativeArray<int> indexCounts = new NativeArray<int>(submeshCount, Allocator.Temp);

            firstVertexForSubmesh = new NativeArray<int>(submeshCount, Allocator.Temp);
            firstTriangleForSubmesh = new NativeArray<int>(submeshCount, Allocator.Temp);
            firstIndexForSubmesh = new NativeArray<int>(submeshCount, Allocator.Temp);

            int totalVertexCount = 0;
            int totalTriangleCount = 0;
            int totalIndexCount = 0;

            for (int i = 0; i < submeshCount; i++)
            {
                indexCounts[i] = triangleCounts[i] * 3;

                firstVertexForSubmesh[i] = totalVertexCount;
                firstTriangleForSubmesh[i] = totalTriangleCount;
                firstIndexForSubmesh[i] = totalIndexCount;

                totalVertexCount += vertexCounts[i];
                totalTriangleCount += triangleCounts[i];
                totalIndexCount += indexCounts[i];
            }

            meshData.SetVertexBufferParams(totalVertexCount, vertexAttributeDescriptor);
            vertexAttributeDescriptor.Dispose();

            meshData.SetIndexBufferParams(totalIndexCount, IndexFormat.UInt32);

            meshData.subMeshCount = submeshCount;
            for (int i = 0; i < submeshCount; i++)
            {
                meshData.SetSubMesh(i, new SubMeshDescriptor(0, indexCounts[i])
                {
                    bounds = bounds,
                    firstVertex = firstVertexForSubmesh[i],
                    vertexCount = vertexCounts[i],
                    indexStart = firstIndexForSubmesh[i],
                    indexCount = indexCounts[i],
                },
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
            }

            stream = meshData.GetVertexData<Vertex>();
            triangleIndices = meshData.GetIndexData<int>().Reinterpret<int3>(4);
        }

        public static NativeArray<VertexAttributeDescriptor> GetVertexAttributeDescriptor()
        {
            var vertexAttributeDescriptor = new NativeArray<VertexAttributeDescriptor>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            vertexAttributeDescriptor[0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3);
            vertexAttributeDescriptor[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3);
            vertexAttributeDescriptor[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, dimension: 4, format: VertexAttributeFormat.Float16);
            vertexAttributeDescriptor[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, format: VertexAttributeFormat.Float16);

            return vertexAttributeDescriptor;
        }
    }
}