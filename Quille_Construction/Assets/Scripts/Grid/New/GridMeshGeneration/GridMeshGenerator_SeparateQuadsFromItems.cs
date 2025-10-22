using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using MeshGeneration;

namespace ProceduralGrid
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct GridMeshGenerator_SeparateQuadsFromItems<S> : IJobParallelFor
        where S : struct, IMeshStreams
    {
        // PROPERTIES
        public NativeArray<GridItem> GridItems;
        public float TileSize { get; set; }
        public float ItemSize { get; set; }

        [WriteOnly] S streams;
        public int VertexCount { get { return GridItems.Length * 4; } }
        public int IndexCount { get { return GridItems.Length * 6; } }
        public Bounds Bounds
        {
            // TODO: find a better way of calculating the bounds. Is it worth doing it here?
            get
            {
                {
                    int lastIndex = GridItems.Length - 1;
                    float highestX = GridItems[lastIndex].GridCoordinates.x + 1;
                    float highestZ = GridItems[lastIndex].GridCoordinates.y + 1;

                    float sizeX = highestX * TileSize;
                    float sizeZ = highestZ * TileSize;

                    float centerX = sizeX / 2f;
                    float centerZ = sizeZ / 2f;

                    if (GridItems[lastIndex].Positioning == GridItemPositioning.AtCorner)
                    {
                        centerX -= (TileSize / 2f);
                        centerZ -= (TileSize / 2f);
                    }

                    return new Bounds(new Vector3(centerX, GridItems[lastIndex].HeightOffset, centerZ), new Vector3(sizeX, 0, sizeZ));
                }
            }
        }

        public int JobLength { get { return GridItems.Length; } }



        // METHODS
        public void Execute(int index)
        {
            float posY = GridItems[index].LocalPosition.y;
            float centeredPosX = GridItems[index].LocalPosition.x;
            float centeredPosZ = GridItems[index].LocalPosition.z;

            float halfWidth = ItemSize / 2f;
            float negPosX = centeredPosX - halfWidth;
            float posPosX = centeredPosX + halfWidth;

            int vi = index * 4;
            int ti = index * 2;

            half hZero = half(0f);
            half hOne = half(1f);

            // Create the base vertex.
            var vertex = new Vertex();
            vertex.position.y = posY;
            vertex.normal.y = 1f;
            vertex.tangent.xw = half2(hOne, half(-1f));

            // Bottom left.
            vertex.position.x = negPosX;
            vertex.position.z = centeredPosZ - halfWidth;
            streams.SetVertex(vi + 0, vertex);

            // Bottom right.
            vertex.position.x = posPosX;
            vertex.texCoord0 = half2(hOne, hZero);
            streams.SetVertex(vi + 1, vertex);

            // Top left.
            vertex.position.x = negPosX;
            vertex.position.z = centeredPosZ + halfWidth;
            vertex.texCoord0 = half2(hZero, hOne);
            streams.SetVertex(vi + 2, vertex);

            // Top right.
            vertex.position.x = posPosX;
            vertex.texCoord0 = hOne;
            streams.SetVertex(vi + 3, vertex);

            // Triangles.
            streams.SetTriangle(ti + 0, vi + int3(0, 2, 1));
            streams.SetTriangle(ti + 1, vi + int3(1, 2, 3));
        }

        public static JobHandle Schedule(Mesh mesh, Mesh.MeshData meshData, NativeArray<GridItem> gridItems, float tileSize, float itemSize, JobHandle dependency)
        {
            var job = new GridMeshGenerator_SeparateQuadsFromItems<S>();
            job.GridItems = gridItems;
            job.TileSize = tileSize;
            job.ItemSize = itemSize;

            job.streams.Setup(meshData, mesh.bounds = job.Bounds, job.VertexCount, job.IndexCount);

            return job.Schedule(job.JobLength, 1, dependency);
        }
    }
}