using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace proceduralGrid
{
    // These use a larger index buffer format than the default to allow for large grids.
    // I may revert this in the future if large grids prove unecessary.

    // Mesh information in a single stream.
    public struct SingleStream : IMeshStreams
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SingleStreamVertex
        {
            // VARIABLES
            public float3 position;
            public float3 normal;
            public float4 tangent;
            public float2 texCoord0;
        }


        // VARIABLES
        [NativeDisableContainerSafetyRestriction] NativeArray<SingleStreamVertex> stream0;
        [NativeDisableContainerSafetyRestriction] NativeArray<int3> triangles;


        // METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex(int index, Vertex vertex) => stream0[index] = new SingleStreamVertex
        {
            position = vertex.position,
            normal = vertex.normal,
            tangent = vertex.tangent,
            texCoord0 = vertex.texCoord0
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTriangle(int index, int3 triangle)
        {
            triangles[index] = triangle;
        }

        public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount)
        {
            var vertexAttributeDescriptor = new NativeArray<VertexAttributeDescriptor>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            vertexAttributeDescriptor[0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3);
            vertexAttributeDescriptor[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3);
            vertexAttributeDescriptor[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, dimension: 4);
            vertexAttributeDescriptor[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2);

            meshData.SetVertexBufferParams(vertexCount, vertexAttributeDescriptor);
            vertexAttributeDescriptor.Dispose();

            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt32);

            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount) { bounds = bounds, vertexCount = vertexCount }, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

            stream0 = meshData.GetVertexData<SingleStreamVertex>();
            triangles = meshData.GetIndexData<int>().Reinterpret<int3>(4);
        }
    }


    // Mesh information across four streams.
    public struct MultiStream : IMeshStreams
    {
        // VARIABLES
        [NativeDisableContainerSafetyRestriction] NativeArray<float3> stream0, stream1;
        [NativeDisableContainerSafetyRestriction] NativeArray<float4> stream2;
        [NativeDisableContainerSafetyRestriction] NativeArray<float2> stream3;
        [NativeDisableContainerSafetyRestriction] NativeArray<int3> triangles;

        
        // METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex(int index, Vertex vertex)
        {
            stream0[index] = vertex.position;
            stream1[index] = vertex.normal;
            stream2[index] = vertex.tangent;
            stream3[index] = vertex.texCoord0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTriangle(int index, int3 triangle)
        {
            triangles[index] = triangle;
        }

        public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount)
        {
            var vertexAttributeDescriptor = new NativeArray<VertexAttributeDescriptor>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            vertexAttributeDescriptor[0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3, stream: 0);
            vertexAttributeDescriptor[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3, stream: 1);
            vertexAttributeDescriptor[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, dimension: 4, stream: 2);
            vertexAttributeDescriptor[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, stream: 3);

            meshData.SetVertexBufferParams(vertexCount, vertexAttributeDescriptor);
            vertexAttributeDescriptor.Dispose();

            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt32);

            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount) { bounds = bounds, vertexCount = vertexCount }, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

            stream0 = meshData.GetVertexData<float3>(0);
            stream1 = meshData.GetVertexData<float3>(1);
            stream2 = meshData.GetVertexData<float4>(2);
            stream3 = meshData.GetVertexData<float2>(3);
            triangles = meshData.GetIndexData<int>().Reinterpret<int3>(4);
        }
    }
}