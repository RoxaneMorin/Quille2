using MeshGeneration;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using static Unity.Mathematics.math;
using static UnityEngine.GraphicsBuffer;

namespace ProceduralGrid
{
    public struct GridMeshGenerator_SeparateQuads : IGridMeshGenerator
    {
        // PROPERTIES
        public int2 Resolution { get; set; }
        public float TileSize { get; set; }

        public int VertexCount { get { return 4 * Resolution.x * Resolution.y; } }
        public int IndexCount { get { return 6 * Resolution.x * Resolution.y; } }
        public Bounds Bounds { get { return new Bounds(new Vector3(Resolution.x * TileSize / 2f, 0f, Resolution.y * TileSize / 2f), new Vector3(Resolution.x * TileSize, 0f, Resolution.y * TileSize)); } }

        public int JobLength { get { return Resolution.y; } }



        // METHODS
        public void Execute<S> (int z, S streams) where S : struct, IMeshStreams 
        {
            int vi = 4 * Resolution.x * z;
            int ti = 2 * Resolution.x * z;

            half hZero = half(0f);
            half hOne = half(1f);

            for (int x = 0; x < Resolution.x; x++, vi += 4, ti += 2)
            {
                var xCoordinates = float2(x, x + 1f) * TileSize;
                var zCoordinates = float2(z, z + 1f) * TileSize;

                var vertex = new Vertex();
                vertex.normal.y = 1f;
                vertex.tangent.xw = half2(hOne, half(-1f));

                vertex.position.x = xCoordinates.x;
                vertex.position.z = zCoordinates.x;
                streams.SetVertex(vi + 0, vertex);

                vertex.position.x = xCoordinates.y;
                vertex.texCoord0 = half2(hOne, hZero);
                streams.SetVertex(vi + 1, vertex);

                vertex.position.x = xCoordinates.x;
                vertex.position.z = zCoordinates.y;
                vertex.texCoord0 = half2(hZero, hOne);
                streams.SetVertex(vi + 2, vertex);

                vertex.position.x = xCoordinates.y;
                vertex.texCoord0 = hOne;
                streams.SetVertex(vi + 3, vertex);

                streams.SetTriangle(ti + 0, vi + int3(0, 2, 1));
                streams.SetTriangle(ti + 1, vi + int3(1, 2, 3));
            }
        }
    }
}

