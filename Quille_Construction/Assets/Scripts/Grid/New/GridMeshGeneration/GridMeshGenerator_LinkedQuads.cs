using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace proceduralGrid
{
    public struct GridMeshGenerator_LinkedQuads : IGridMeshGenerator
    {
        // PROPERTIES
        public int2 Resolution { get; set; }
        public float TileSize { get; set; }

        public int VertexCount { get { return (Resolution.x + 1) * (Resolution.y + 1); } }
        public int IndexCount { get { return 6 * Resolution.x * Resolution.y; } }
        public Bounds Bounds { get { return new Bounds(new Vector3(Resolution.x * TileSize / 2f, 0f, Resolution.y * TileSize / 2f), new Vector3(Resolution.x * TileSize, 0f, Resolution.y * TileSize)); } }

        public int JobLength { get { return Resolution.y + 1; } }



        // METHODS
        public void Execute<S>(int z, S streams) where S : struct, IMeshStreams
        {
            int vi = (Resolution.x + 1) * z;
            int ti = 2 * Resolution.x * (z - 1);

            var vertex = new Vertex();
            vertex.normal.y = 1f;
            vertex.tangent.xw = float2(1f, -1f);

            vertex.position.x = 0f;
            vertex.position.z = z * TileSize;
            vertex.texCoord0.y = z;
            streams.SetVertex(vi, vertex);
            vi += 1;

            for (int x = 1; x <= Resolution.x; x++, vi++, ti += 2)
            {
                vertex.position.x = x * TileSize;
                vertex.texCoord0.x = x;
                streams.SetVertex(vi, vertex);

                if (z > 0)
                {
                    streams.SetTriangle(ti + 0, vi + int3(-Resolution.x - 2, -1, -Resolution.x - 1));
                    streams.SetTriangle(ti + 1, vi + int3(-Resolution.x - 1, -1, 0));
                }
            }
        }
    }
}

