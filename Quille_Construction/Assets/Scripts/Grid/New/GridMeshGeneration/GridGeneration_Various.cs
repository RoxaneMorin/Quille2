using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Mathematics;

namespace proceduralGrid
{
    // ENUMS
    public enum GridMeshType
    {
        SeparateQuads,
        LinkedQuads
    };



    // STRUCTS
    public struct Vertex
    {
        // VARIABLES
        public float3 position;
        public float3 normal;
        public float4 tangent;
        public float2 texCoord0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TriangleUInt16
    {
        public ushort a, b, c;

        public static implicit operator TriangleUInt16(int3 t) => new TriangleUInt16 { a = (ushort)t.x, b = (ushort)t.y, c = (ushort)t.z };
    }



    // INTERFACES
    public interface IMeshStreams
    {
        public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount);
        public void SetVertex(int index, Vertex vertex);
        public void SetTriangle(int index, int3 triangle);
    }

    public interface IMeshGenerator
    {
        public int VertexCount { get; }
        public int IndexCount { get; }
        public Bounds Bounds { get; }

        public int JobLength { get; }

        public void Execute<S>(int i, S streams) where S : struct, IMeshStreams;
    }

    public interface IGridMeshGenerator : IMeshGenerator
    {
        public int2 Resolution { get; set; }
        public float TileSize { get; set; }
    }
}