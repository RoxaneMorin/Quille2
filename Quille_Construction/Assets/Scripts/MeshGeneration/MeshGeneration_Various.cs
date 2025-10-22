using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace MeshGeneration
{
    // STRUCTS
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        // VARIABLES
        public float3 position;
        public float3 normal;
        public half4 tangent;
        public half2 texCoord0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TriangleUInt16
    {
        public ushort a, b, c;

        public static implicit operator TriangleUInt16(int3 t) => new TriangleUInt16 { a = (ushort)t.x, b = (ushort)t.y, c = (ushort)t.z };

        public override string ToString()
        {
            return $"({a}, {b}, {c})";
        }
    }


    // INTERFACES
    public interface IMeshStreams
    {
        public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount);
        public void SetVertex(int index, Vertex vertex);
        public void SetTriangle(int index, int3 triangle);
    }

    public interface IMultiMeshStreams
    {
        // Should the vertexCounts use NativeArrays instead?
        public void Setup(Mesh.MeshData meshData, Bounds bounds, int submeshCount, NativeArray<int> vertexCounts, NativeArray<int> triangleCounts);
        public void SetVertex(int submesh, int index, Vertex vertex);
        public void SetTriangle(int submesh, int index, int3 triangle);
    }

    public interface IMeshGenerator
    {
        public int VertexCount { get; }
        public int IndexCount { get; }
        public Bounds Bounds { get; }

        public int JobLength { get; }

        public void Execute<S>(int i, S streams) where S : struct, IMeshStreams;
    }


}