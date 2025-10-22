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
    // TODO: Add the option to have multiple submeshes

    // Single submesh streams

    // For a maximum of ~65535 indices.
    public struct SingleStreamUInt16 : IMeshStreams
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
    public struct SingleStreamUInt32 : IMeshStreams
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
}